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


                    var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.UserId == pUserId).FirstOrDefault();

                    if (wxUserInfo == null)
                    {
                        return new CustomJsonResult<Order>(ResultType.Failure, ResultCode.Failure, "找不到用户微信信息", null);
                    }


                    var order = new Order();
                    order.Id = GuidUtil.New();
                    order.UserId = pUserId;
                    order.Sn = SnUtil.Build(Enumeration.BizSnType.Order);
                    order.IsPromoteProfit = true;
                    order.PromoteId = promote.Id;
                    order.OriginalAmount = productSku.Price;
                    order.DiscountAmount = 0;
                    order.ChargeAmount = order.OriginalAmount - order.DiscountAmount;
                    order.Status = Enumeration.OrderStatus.WaitPay; //待支付状态
                    order.SubmitTime = this.DateTime;
                    order.CreateTime = this.DateTime;
                    order.Creator = pOperater;
                    CurrentDb.Order.Add(order);
                    CurrentDb.SaveChanges();

                    LogUtil.Info("步骤2");

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

                    LogUtil.Info("步骤3");

                    string goods_tag = "";
                    if (order.ChargeAmount > 0)
                    {
                        string prepayId = SdkFactory.Wx.Instance().GetPrepayId(pOperater, "JSAPI", wxUserInfo.OpenId, order.Sn, 0.01m, goods_tag, Common.CommonUtils.GetIP(), productSku.Name);

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

                    LogUtil.Info("步骤4");

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
        public CustomJsonResult PayResultNotify(string operater, Enumeration.OrderNotifyLogNotifyFrom from, string content, string orderSn = "")
        {
            lock (lock_PayResultNotify)
            {
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
                                PayCompleted(operater, orderSn, this.DateTime);
                            }
                        }
                        break;
                }

                var order = CurrentDb.Order.Where(m => m.Sn == orderSn).FirstOrDefault();
                if (order != null)
                {
                    mod_OrderNotifyLog.UserId = order.UserId;
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


                if (order.IsPromoteProfit)
                {
                    //var promoteUserRelations = CurrentDb.PromoteUserRelation.Where(m => m.CUserId == order.UserId && m.PromoteId == order.PromoteId).ToList();
                    //var promoteProfitRates = CurrentDb.PromoteProfitRate.Where(m => m.PromoteId == order.PromoteId).ToList();
                    //foreach (var item in promoteUserRelations)
                    //{
                    //    var promoteProfitRate = promoteProfitRates.Where(m => m.Dept == item.Dept).FirstOrDefault();

                    //    var fund = CurrentDb.Fund.Where(m => m.UserId == item.UserId).FirstOrDefault();

                    //    var profit = order.ChargeAmount * (promoteProfitRate.Rate / 100);
                    //    fund.Balance += profit;
                    //    fund.Mender = pOperater;
                    //    fund.MendTime = this.DateTime;

                    //    var fundTrans = new FundTrans();
                    //    fundTrans.Id = GuidUtil.New();
                    //    fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.Order);
                    //    fundTrans.UserId = fund.UserId;
                    //    fundTrans.ChangeType = Enumeration.FundTransChangeType.PromoteProfit;
                    //    fundTrans.ChangeAmount = profit;
                    //    fundTrans.Balance = fund.Balance;
                    //    fundTrans.CreateTime = this.DateTime;
                    //    fundTrans.Creator = pOperater;
                    //    fundTrans.Description = string.Format("下级支付{0}元购买优惠卷，作为上级获得收益{1}元", order.ChargeAmount, profit);
                    //    CurrentDb.FundTrans.Add(fundTrans);
                    //    CurrentDb.SaveChanges();

                    //}

                    var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == order.Id).FirstOrDefault();

                    var promoteCoupon = CurrentDb.PromoteCoupon.Where(m => m.ProductSkuId == orderDetails.ProductSkuId && m.PromoteId == order.PromoteId).FirstOrDefault();

                    var promoteUserCoupon = new PromoteUserCoupon();
                    promoteUserCoupon.Id = GuidUtil.New();
                    promoteUserCoupon.UserId = order.UserId;
                    promoteUserCoupon.PromoteId = promoteCoupon.PromoteId;
                    promoteUserCoupon.PromoteCouponId = promoteCoupon.Id;
                    promoteUserCoupon.WxCouponId = promoteCoupon.WxCouponId;
                    promoteUserCoupon.IsBuy = true;
                    promoteUserCoupon.BuyTime = this.DateTime;
                    promoteUserCoupon.IsGet = false;
                    promoteUserCoupon.IsConsume = false;
                    promoteUserCoupon.Creator = pOperater;
                    promoteUserCoupon.CreateTime = this.DateTime;
                    CurrentDb.PromoteUserCoupon.Add(promoteUserCoupon);
                    CurrentDb.SaveChanges();
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, string.Format("支付完成通知：订单号({0})通知成功", pOrderSn));
            }

            return result;
        }

    }
}
