using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.App
{
    public class OrderService : BaseProvider
    {
        public CustomJsonResult<RetOrderUnifiedOrder> UnifiedOrder(string pOperater, string pClientId, RopOrderUnifiedOrder rop)
        {
            CustomJsonResult<RetOrderUnifiedOrder> result = new CustomJsonResult<RetOrderUnifiedOrder>();

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    var productSku = CurrentDb.ProductSku.Where(m => m.Id == rop.SkuId).FirstOrDefault();

                    if (productSku == null)
                    {
                        return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "该商品不存在", null);
                    }

                    var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientId == pClientId).FirstOrDefault();

                    if (wxUserInfo == null)
                    {
                        return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "找不到用户微信信息", null);
                    }

                    //if(rop)
                    //var orderByBuyed = CurrentDb.OrderDetails.Where(m => m.ClientId == pClientId && rop.SkuId && m.Status == Enumeration.OrderStatus.Payed).FirstOrDefault();
                    //if (orderByBuyed != null)
                    //{
                    //    return new CustomJsonResult<Order>(ResultType.Success, ResultCode.Success, "您已成功抢购", orderByBuyed);
                    //}


                    decimal salePrice = productSku.SalePrice;

                    if (!string.IsNullOrEmpty(rop.PromoteId))
                    {
                        var clientCoupon = CurrentDb.ClientCoupon.Where(m => m.PromoteId == rop.PromoteId && m.ClientId == pClientId).FirstOrDefault();
                        if (clientCoupon != null)
                        {
                            if (clientCoupon.IsBuy)
                            {
                                var promoteSku = CurrentDb.PromoteSku.Where(m => m.PromoteId == rop.PromoteId && m.SkuId == rop.SkuId).FirstOrDefault();
                                if (promoteSku != null)
                                {
                                    if (promoteSku.BuyStartTime <= this.DateTime && promoteSku.BuyEndTime >= this.DateTime)
                                    {
                                        salePrice = promoteSku.SkuSalePrice;
                                    }
                                }
                            }
                        }
                    }

                    var order = new Order();
                    order.Id = GuidUtil.New();
                    order.ClientId = pClientId;
                    order.Sn = SnUtil.Build(Enumeration.BizSnType.Order, order.ClientId);
                    order.PromoteId = rop.PromoteId;
                    order.RefereeId = rop.RefereeId;
                    order.OriginalAmount = salePrice;
                    order.DiscountAmount = 0;
                    order.ChargeAmount = order.OriginalAmount - order.DiscountAmount;
                    order.SubmitTime = this.DateTime;
                    order.CreateTime = this.DateTime;
                    order.Creator = pOperater;
                    CurrentDb.Order.Add(order);
                    CurrentDb.SaveChanges();

                    var orderDetails = new OrderDetails();
                    orderDetails.Id = GuidUtil.New();
                    orderDetails.PromoteId = rop.PromoteId;
                    orderDetails.ClientId = pClientId;
                    orderDetails.OrderId = order.Id;
                    orderDetails.Quantity = 1;
                    orderDetails.SalePrice = salePrice;
                    orderDetails.ProductSkuId = productSku.Id;
                    orderDetails.ProductSkuName = productSku.Name;
                    orderDetails.OriginalAmount = order.OriginalAmount;
                    orderDetails.DiscountAmount = order.DiscountAmount;
                    orderDetails.ChargeAmount = order.ChargeAmount;
                    orderDetails.CreateTime = order.CreateTime;
                    orderDetails.Creator = order.Creator;
                    CurrentDb.OrderDetails.Add(orderDetails);
                    CurrentDb.SaveChanges();

                    bool isNeedBuy = true;
                    decimal chargeAmount = order.ChargeAmount;

                    if (chargeAmount <= 0)
                    {
                        isNeedBuy = false;
                    }


                    string[] testClientId = new string[2] { "62c587c13c124f96b436de9522fb31f0", "4faecb3507aa48698405cf492dc26916" };

                    if (isNeedBuy)
                    {
                        order.Status = Enumeration.OrderStatus.WaitPay; //待支付状态
                        order.WxPrepayIdExpireTime = this.DateTime.AddMinutes(5);

                        if (testClientId.Contains(order.ClientId))
                        {
                            chargeAmount = 0.01m;
                        }
                        else
                        {
                            chargeAmount = salePrice;
                        }

                        string goods_tag = "";
                        string prepayId = SdkFactory.Wx.Instance().GetPrepayId(pOperater, "JSAPI", wxUserInfo.OpenId, order.Sn, chargeAmount, goods_tag, Common.CommonUtils.GetIP(), productSku.Name, order.WxPrepayIdExpireTime);
                        if (string.IsNullOrEmpty(prepayId))
                        {
                            LogUtil.Error("去结算，微信支付中生成预支付订单失败");
                            return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "微信支付中生成预支付订单失败", null);
                        }

                        order.WxPrepayId = prepayId;

                        OrderCacheUtil.EnterQueue4CheckPayStatus(order.Sn, order);
                    }
                    else
                    {
                        BizFactory.Order.PayCompleted(pOperater, order.Sn, this.DateTime);
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();

                    var ret = new RetOrderUnifiedOrder();
                    ret.WxPrepayId = order.WxPrepayId;
                    ret.OrderId = order.Id;
                    ret.OrderSn = order.Sn;
                    ret.IsBuy = false;

                    result = new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Success, ResultCode.Success, "操作成功", ret);
                    LogUtil.Info("去结算结束");
                }

                return result;
            }
            catch (Exception ex)
            {
                LogUtil.Error("检查下单发生异常", ex);

                return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Exception, ResultCode.Exception, "下单发生异常", null);
            }





            return result;
        }
    }
}
