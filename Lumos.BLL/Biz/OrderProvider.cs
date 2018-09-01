using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL
{
    public class OrderProvider : BaseProvider
    {
        public CustomJsonResult<Order> UnifiedOrder(string pOperater, string pUserId, UnifiedOrderPms pPayPms)
        {
            CustomJsonResult<Order> result = new CustomJsonResult<Order>();
            var strOrderPms = Newtonsoft.Json.JsonConvert.SerializeObject(pPayPms.OrderPms);
            switch (pPayPms.Type)
            {
                case UnifiedOrderType.BuyPromoteCoupon:
                    var orderPmsByBuyPromoteCoupon = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderPmsByBuyPromoteCoupon>(strOrderPms);
                    result = UnifiedOrderByBuyPromoteCoupon(pOperater, pUserId, orderPmsByBuyPromoteCoupon);
                    break;
            }
            return result;
        }

        private CustomJsonResult<Order> UnifiedOrderByBuyPromoteCoupon(string pOperater, string pUserId, OrderPmsByBuyPromoteCoupon pms)
        {
            var result = new CustomJsonResult<Order>();

            using (TransactionScope ts = new TransactionScope())
            {
                var promote = CurrentDb.Promote.Where(m => m.Id == pms.PromoteId).FirstOrDefault();

                if (promote == null)
                {
                    return new CustomJsonResult<Order>(ResultType.Failure, ResultCode.Failure, "找不到该活动", null);
                }

                if (promote.StartTime > this.DateTime)
                {
                    return new CustomJsonResult<Order>(ResultType.Failure, ResultCode.Failure, "该活动未开始", null);
                }
                if (promote.EndTime < this.DateTime)
                {
                    return new CustomJsonResult<Order>(ResultType.Failure, ResultCode.Failure, "该活动已结束", null);
                }


                var promoteCoupon = CurrentDb.PromoteCoupon.Where(m => m.Id == pms.PromoteCouponId && m.PromoteId == pms.PromoteId).FirstOrDefault();

                if (promoteCoupon == null)
                {
                    return new CustomJsonResult<Order>(ResultType.Failure, ResultCode.Failure, "找不到该优惠卷", null);
                }

                var productSku = CurrentDb.ProductSku.Where(m => m.Id == promoteCoupon.ProductSkuId).FirstOrDefault();

                if (productSku == null)
                {
                    return new CustomJsonResult<Order>(ResultType.Failure, ResultCode.Failure, "找不到该商品优惠卷", null);
                }

                var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.UserId == pUserId).FirstOrDefault();

                if (wxUserInfo == null)
                {
                    return new CustomJsonResult<Order>(ResultType.Failure, ResultCode.Failure, "找不到授权信息", null);
                }

                var order = new Order();
                order.Id = GuidUtil.New();
                order.UserId = pUserId;
                order.Sn = SnUtil.Build(Enumeration.BizSnType.Order);
                order.OriginalAmount = productSku.Price;
                order.DiscountAmount = 0;
                order.ChargeAmount = order.OriginalAmount - order.DiscountAmount;
                order.Status = Enumeration.OrderStatus.WaitPay; //待支付状态
                order.SubmitTime = this.DateTime;
                order.CreateTime = this.DateTime;
                order.Creator = pOperater;
                CurrentDb.Order.Add(order);
                CurrentDb.SaveChanges();


                var orderDetails = new OrderDetails();
                orderDetails.Id = GuidUtil.New();
                orderDetails.UserId = pUserId;
                orderDetails.OrderId = order.Id;
                orderDetails.Quantity = 1;
                orderDetails.UnitPrice = productSku.Price;
                orderDetails.ProductSkuId = productSku.Id;
                orderDetails.ProductSkuName = productSku.Name;
                orderDetails.OriginalAmount = order.OriginalAmount;
                orderDetails.DiscountAmount = order.DiscountAmount;
                orderDetails.ChargeAmount = order.ChargeAmount;
                orderDetails.CreateTime = order.CreateTime;
                orderDetails.Creator = order.Creator;
                CurrentDb.OrderDetails.Add(orderDetails);
                CurrentDb.SaveChanges();

                string goods_tag = "";
                if (order.ChargeAmount > 0)
                {
                    string prepayId = SdkFactory.Wx.Instance().GetPrepayId(pOperater, "JSAPI", wxUserInfo.OpenId, order.Sn, order.ChargeAmount, goods_tag, Common.CommonUtils.GetIP(), productSku.Name);

                    if (string.IsNullOrEmpty(prepayId))
                    {
                        //LogUtil.ErrorFormat("去结算，订单号（{0}）生成在微信支付中生成不到预支付订单号", mod_Order.Sn);
                    }
                    else
                    {
                        order.WxPrepayId = prepayId;
                        order.WxPrepayIdExpireTime = this.DateTime.AddMinutes(119);
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult<Lumos.Entity.Order>(ResultType.Success, ResultCode.Success, "操作成功", order);
                LogUtil.Info("去结算结束");
            }

            return result;
        }
    }
}
