using Lumos.Entity;
using Lumos.Redis;
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
        private static readonly object lock_UnifiedOrder = new object();
        public CustomJsonResult<Order> UnifiedOrder(string pOperater, UnifiedOrderPms pPayPms)
        {
            CustomJsonResult<Order> result = new CustomJsonResult<Order>();
            lock (lock_UnifiedOrder)
            {
                var strOrderPms = Newtonsoft.Json.JsonConvert.SerializeObject(pPayPms.OrderPms);
                switch (pPayPms.Type)
                {
                    case UnifiedOrderType.BuyPromoteCoupon:
                        var orderPmsByBuyPromoteCoupon = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderPmsByBuyPromoteCoupon>(strOrderPms);
                        result = UnifiedOrderByBuyPromoteCoupon(pOperater, pPayPms.ClientId, pPayPms.RefereeId, orderPmsByBuyPromoteCoupon);
                        break;
                }
            }
            return result;
        }

        private CustomJsonResult<Order> UnifiedOrderByBuyPromoteCoupon(string pOperater, string pClientId, string pRefereeId, OrderPmsByBuyPromoteCoupon pms)
        {
            var result = new CustomJsonResult<Order>();

            try
            {
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


                    var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientId == pClientId).FirstOrDefault();

                    if (wxUserInfo == null)
                    {
                        return new CustomJsonResult<Order>(ResultType.Failure, ResultCode.Failure, "找不到用户微信信息", null);
                    }

                    var orderByBuyed = CurrentDb.Order.Where(m => m.ClientId == pClientId && m.PromoteId == pms.PromoteId && m.Status == Enumeration.OrderStatus.Payed).FirstOrDefault();
                    if (orderByBuyed != null)
                    {
                        return new CustomJsonResult<Order>(ResultType.Success, ResultCode.Success, "您已成功抢购", orderByBuyed);
                    }

                    var order = new Order();
                    order.Id = GuidUtil.New();
                    order.ClientId = pClientId;
                    order.Sn = SnUtil.Build(Enumeration.BizSnType.Order, order.ClientId);
                    order.PromoteId = promote.Id;
                    order.RefereeId = pRefereeId;
                    order.OriginalAmount = productSku.Price;
                    order.DiscountAmount = 0;
                    order.ChargeAmount = order.OriginalAmount - order.DiscountAmount;
                    order.SubmitTime = this.DateTime;
                    order.CreateTime = this.DateTime;
                    order.Creator = pOperater;
                    CurrentDb.Order.Add(order);
                    CurrentDb.SaveChanges();

                    var orderDetails = new OrderDetails();
                    orderDetails.Id = GuidUtil.New();
                    orderDetails.ClientId = pClientId;
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


                    decimal chargeAmount = order.ChargeAmount;

                    if (promote.IsNeedBuy)
                    {
                        order.Status = Enumeration.OrderStatus.WaitPay; //待支付状态
                        order.WxPrepayIdExpireTime = this.DateTime.AddMinutes(5);

                        if (order.ClientId == "62c587c13c124f96b436de9522fb31f0")
                        {
                            chargeAmount = 0.01m;
                        }
                        else if (order.ClientId == "4faecb3507aa48698405cf492dc26916")
                        {
                            chargeAmount = 0.01m;
                        }




                        string goods_tag = "";
                        if (order.ChargeAmount > 0)
                        {
                            string prepayId = SdkFactory.Wx.Instance().GetPrepayId(pOperater, "JSAPI", wxUserInfo.OpenId, order.Sn, chargeAmount, goods_tag, Common.CommonUtils.GetIP(), productSku.Name, order.WxPrepayIdExpireTime);

                            if (string.IsNullOrEmpty(prepayId))
                            {
                                LogUtil.Error("去结算，微信支付中生成预支付订单失败");

                                return new CustomJsonResult<Lumos.Entity.Order>(ResultType.Failure, ResultCode.Failure, "微信支付中生成预支付订单失败", order);
                            }
                            else
                            {
                                order.WxPrepayId = prepayId;
                            }
                        }

                        OrderCacheUtil.EnterQueue4CheckPayStatus(order.Sn, order);
                    }
                    else
                    {
                        BizFactory.Order.PayCompleted(pOperater, order.Sn, this.DateTime);
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();


                    result = new CustomJsonResult<Lumos.Entity.Order>(ResultType.Success, ResultCode.Success, "操作成功", order);
                    LogUtil.Info("去结算结束");
                }

                return result;
            }
            catch (Exception ex)
            {
                LogUtil.Error("检查下单发生异常", ex);

                return new CustomJsonResult<Order>(ResultType.Exception, ResultCode.Exception, "下单发生异常", null);
            }
        }

        private static readonly object lock_PayResultNotify = new object();
        public CustomJsonResult PayResultNotify(string operater, Enumeration.OrderNotifyLogNotifyFrom from, string content, string orderSn, out bool isPaySuccessed)
        {
            lock (lock_PayResultNotify)
            {
                bool m_isPaySuccessed = false;
                var mod_OrderNotifyLog = new OrderNotifyLog();

                switch (from)
                {
                    case Enumeration.OrderNotifyLogNotifyFrom.WebApp:
                        if (content == "chooseWXPay:ok")
                        {
                            // PayCompleted(operater, orderSn, this.DateTime);
                        }
                        break;
                    case Enumeration.OrderNotifyLogNotifyFrom.OrderQuery:
                        var dic1 = WeiXinSdk.CommonUtil.ToDictionary(content);
                        if (dic1.ContainsKey("out_trade_no") && dic1.ContainsKey("trade_state"))
                        {
                            orderSn = dic1["out_trade_no"].ToString();
                            string trade_state = dic1["trade_state"].ToString();
                            if (trade_state == "SUCCESS")
                            {
                                m_isPaySuccessed = true;
                                PayCompleted(operater, orderSn, this.DateTime);
                            }
                        }
                        break;
                    case Enumeration.OrderNotifyLogNotifyFrom.NotifyUrl:
                        var dic2 = WeiXinSdk.CommonUtil.ToDictionary(content);
                        if (dic2.ContainsKey("out_trade_no") && dic2.ContainsKey("result_code"))
                        {
                            orderSn = dic2["out_trade_no"].ToString();
                            string result_code = dic2["result_code"].ToString();

                            if (result_code == "SUCCESS")
                            {
                                m_isPaySuccessed = true;
                                PayCompleted(operater, orderSn, this.DateTime);
                            }
                        }
                        break;
                }

                var order = CurrentDb.Order.Where(m => m.Sn == orderSn).FirstOrDefault();
                if (order != null)
                {
                    mod_OrderNotifyLog.ClientId = order.ClientId;
                    mod_OrderNotifyLog.OrderId = order.Id;
                    mod_OrderNotifyLog.OrderSn = order.Sn;
                }
                mod_OrderNotifyLog.Id = GuidUtil.New();
                mod_OrderNotifyLog.NotifyContent = content;
                mod_OrderNotifyLog.NotifyFrom = from;
                mod_OrderNotifyLog.NotifyType = Enumeration.OrderNotifyLogNotifyType.Pay;
                mod_OrderNotifyLog.CreateTime = this.DateTime;
                mod_OrderNotifyLog.Creator = operater;
                CurrentDb.OrderNotifyLog.Add(mod_OrderNotifyLog);
                CurrentDb.SaveChanges();

                isPaySuccessed = m_isPaySuccessed;

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
            }
        }

        public CustomJsonResult PayCompleted(string pOperater, string pOrderSn, DateTime pCompletedTime)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var order = CurrentDb.Order.Where(m => m.Sn == pOrderSn).FirstOrDefault();

                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("找不到该订单号({0})", pOrderSn));
                }

                if (order.Status == Enumeration.OrderStatus.Payed || order.Status == Enumeration.OrderStatus.Completed)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("订单号({0})已经支付通知成功", pOrderSn));
                }

                if (order.Status != Enumeration.OrderStatus.WaitPay)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("找不到该订单号({0})", pOrderSn));
                }

                order.Status = Enumeration.OrderStatus.Payed;
                order.PayTime = this.DateTime;
                order.MendTime = this.DateTime;
                order.Mender = pOperater;

                ReidsMqByCalProfitModel reidsMqByCalProfitModel = null;

                if (order.PromoteId != null)
                {
                    var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == order.Id).FirstOrDefault();
                    if (orderDetails != null)
                    {
                        var promoteCoupon = CurrentDb.PromoteCoupon.Where(m => m.ProductSkuId == orderDetails.ProductSkuId && m.PromoteId == order.PromoteId).FirstOrDefault();
                        if (promoteCoupon != null)
                        {
                            var clientCoupon = CurrentDb.ClientCoupon.Where(m => m.ClientId == order.ClientId && m.PromoteId == order.PromoteId && m.PromoteCouponId == promoteCoupon.Id).FirstOrDefault();
                            if (clientCoupon == null)
                            {
                                clientCoupon = new ClientCoupon();
                                clientCoupon.Id = GuidUtil.New();
                                clientCoupon.ClientId = order.ClientId;
                                clientCoupon.PromoteId = promoteCoupon.PromoteId;
                                clientCoupon.PromoteCouponId = promoteCoupon.Id;
                                clientCoupon.WxCouponId = promoteCoupon.WxCouponId;
                                clientCoupon.IsBuy = true;
                                clientCoupon.BuyTime = this.DateTime;
                                clientCoupon.IsGet = false;
                                clientCoupon.IsConsume = false;
                                clientCoupon.Creator = pOperater;
                                clientCoupon.CreateTime = this.DateTime;
                                clientCoupon.RefereeId = order.RefereeId;
                                clientCoupon.OrderId = order.Id;
                                clientCoupon.OrderSn = order.Sn;

                                clientCoupon.Name = promoteCoupon.Name;
                                clientCoupon.Number = promoteCoupon.Number;
                                clientCoupon.NumberType = promoteCoupon.NumberType;
                                clientCoupon.NumberUnit = promoteCoupon.NumberUnit;
                                clientCoupon.ValidStartTime = promoteCoupon.ValidStartTime;
                                clientCoupon.ValidEndTime = promoteCoupon.ValidEndTime;
                                clientCoupon.Description = promoteCoupon.Description;
                                clientCoupon.Discounttip = promoteCoupon.Discounttip;
                                CurrentDb.ClientCoupon.Add(clientCoupon);
                                CurrentDb.SaveChanges();

                                reidsMqByCalProfitModel = new ReidsMqByCalProfitModel();
                                reidsMqByCalProfitModel.Type = ReidsMqByCalProfitType.CouponBuy;

                                var reidsMqByCalProfitByCouponBuyModel = new ReidsMqByCalProfitByCouponBuyModel();
                                reidsMqByCalProfitByCouponBuyModel.OrderId = order.Id;
                                reidsMqByCalProfitByCouponBuyModel.ClientId = order.ClientId;
                                reidsMqByCalProfitByCouponBuyModel.PromoteId = order.PromoteId;
                                reidsMqByCalProfitByCouponBuyModel.RefereeId = order.RefereeId;
                                reidsMqByCalProfitByCouponBuyModel.ClientCouponId = clientCoupon.Id;
                                reidsMqByCalProfitModel.Pms = reidsMqByCalProfitByCouponBuyModel;
                            }
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                if (reidsMqByCalProfitModel != null)
                {
                    ReidsMqFactory.CalProfit.Push(reidsMqByCalProfitModel);
                }

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, string.Format("支付完成通知：订单号({0})通知成功", pOrderSn));
            }

            return result;
        }


    }
}
