using Lumos.BLL.Sdk;
using Lumos.BLL.Task;
using Lumos.Entity;
using Lumos.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Biz
{
    public class CouponModel
    {
        public string Name { get; set; }
        public string Discounttip { get; set; }
        public string Description { get; set; }
        public decimal Number { get; set; }
        public string NumberType { get; set; }
        public string NumberUnit { get; set; }
        public DateTime? ValidStartTime { get; set; }
        public DateTime? ValidEndTime { get; set; }
    }

    public class OrderProvider : BaseProvider
    {

        private static readonly object lock_UnifiedOrder = new object();
        public CustomJsonResult<RetOrderUnifiedOrder> UnifiedOrder(string operater, string clientId, RopOrderUnifiedOrder rop)
        {
            CustomJsonResult<RetOrderUnifiedOrder> result = new CustomJsonResult<RetOrderUnifiedOrder>();

            try
            {
                lock (lock_UnifiedOrder)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        LogUtil.Info("用户id:" + clientId);

                        var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientId == clientId).FirstOrDefault();

                        if (wxUserInfo == null)
                        {
                            return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "找不到用户微信信息", null);
                        }

                        var orderByBuyed = CurrentDb.Order.Where(m => m.ClientId == clientId && m.PromoteId == rop.PromoteId && m.Status == Enumeration.OrderStatus.Payed).FirstOrDefault();
                        if (orderByBuyed != null)
                        {
                            return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "您已成功抢购,支付成功", null);
                        }

                        var promoteBlackList = CurrentDb.PromoteBlackList.Where(m => m.PromoteId == rop.PromoteId && m.ClientId == clientId).FirstOrDefault();
                        if (promoteBlackList != null)
                        {
                            return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "谢谢参与，已售罄", null);
                        }

                        if (!string.IsNullOrEmpty(rop.PromoteId))
                        {
                            foreach (var sku in rop.Skus)
                            {
                                var productSku = CurrentDb.ProductSku.Where(m => m.Id == sku.SkuId).FirstOrDefault();

                                if (productSku == null)
                                {
                                    return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "找不到该商品", null);
                                }

                                var promoteSku = CurrentDb.PromoteSku.Where(m => m.PromoteId == rop.PromoteId && m.SkuId == sku.SkuId && m.BuyStartTime <= this.DateTime && m.BuyEndTime >= this.DateTime).FirstOrDefault();

                                if (promoteSku == null)
                                {
                                    return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "谢谢参与，活动已经结束", null);
                                }

                                if (!string.IsNullOrEmpty(promoteSku.RefereePromoteId))
                                {
                                    var clientCoupon = CurrentDb.ClientCoupon.Where(m => m.PromoteId == promoteSku.RefereePromoteId && m.ClientId == clientId && m.IsBuy == true).FirstOrDefault();
                                    if (clientCoupon == null)
                                    {
                                        return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "谢谢参与，您没有资格参与购买", null);
                                    }
                                }

                                if (promoteSku.StockQuantity > -1)
                                {
                                    if (promoteSku.SellQuantity <= 0)
                                    {
                                        return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "谢谢参与，商品已经售罄", null);
                                    }
                                }
                            }
                        }

                        var order = CurrentDb.Order.Where(m => m.PromoteId == rop.PromoteId && m.ClientId == clientId && m.Status == Entity.Enumeration.OrderStatus.WaitPay).FirstOrDefault();

                        if (order == null)
                        {
                            string orderId = GuidUtil.New();

                            var l_orderDetails = new List<OrderDetails>();

                            foreach (var sku in rop.Skus)
                            {
                                var productSku = CurrentDb.ProductSku.Where(m => m.Id == sku.SkuId).FirstOrDefault();

                                var productSkuOriginalPrice = productSku.SalePrice;
                                var productSkuSalePrice = productSku.SalePrice;

                                var promoteSku = CurrentDb.PromoteSku.Where(m => m.PromoteId == rop.PromoteId && m.SkuId == sku.SkuId && m.BuyStartTime <= this.DateTime && m.BuyEndTime >= this.DateTime).FirstOrDefault();
                                if (promoteSku != null)
                                {
                                    productSkuSalePrice = promoteSku.SkuSalePrice;

                                    if (promoteSku.StockQuantity > -1)
                                    {
                                        promoteSku.SellQuantity -= 1;
                                        promoteSku.LockQuantity += 1;
                                    }
                                }

                                var orderDetails = new OrderDetails();
                                orderDetails.Id = GuidUtil.New();
                                orderDetails.ClientId = clientId;
                                orderDetails.OrderId = orderId;
                                orderDetails.PromoteId = rop.PromoteId;
                                orderDetails.SkuId = productSku.Id;
                                orderDetails.SkuName = productSku.Name;
                                orderDetails.SkuImgUrl = ImgSet.GetMain(productSku.DisplayImgUrls);
                                orderDetails.Quantity = 1;
                                orderDetails.SalePrice = productSkuSalePrice;
                                orderDetails.OriginalAmount = productSkuOriginalPrice;
                                orderDetails.DiscountAmount = productSkuOriginalPrice - productSkuSalePrice;
                                orderDetails.ChargeAmount = orderDetails.OriginalAmount - orderDetails.DiscountAmount;
                                orderDetails.CreateTime = this.DateTime;
                                orderDetails.Creator = operater;
                                orderDetails.Status = Enumeration.OrderDetailsStatus.WaitPay;
                                CurrentDb.OrderDetails.Add(orderDetails);
                                l_orderDetails.Add(orderDetails);
                            }


                            order = new Order();
                            order.Id = orderId;
                            order.ClientId = clientId;
                            order.Sn = SnUtil.Build(Enumeration.BizSnType.Order, order.ClientId);
                            order.PromoteId = rop.PromoteId;
                            order.RefereerId = rop.RefereerId;
                            order.OriginalAmount = l_orderDetails.Sum(m => m.OriginalAmount);
                            order.DiscountAmount = l_orderDetails.Sum(m => m.DiscountAmount);
                            order.ChargeAmount = order.OriginalAmount - order.DiscountAmount;
                            order.SubmitTime = this.DateTime;
                            order.CreateTime = this.DateTime;
                            order.Creator = operater;
                            order.IsInVisiable = true;
                            order.Status = Enumeration.OrderStatus.WaitPay; //待支付状态

                            var promoteUser = CurrentDb.PromoteUser.Where(m => m.ClientId == order.ClientId && m.CtPhone != null).OrderByDescending(m => m.CreateTime).FirstOrDefault();
                            if (promoteUser != null)
                            {
                                order.CtName = promoteUser.CtName;
                                order.CtPhone = promoteUser.CtPhone;
                                order.CtIsStudent = promoteUser.CtIsStudent;
                                order.CtSchool = promoteUser.CtSchool;
                            }

                            CurrentDb.Order.Add(order);
                            CurrentDb.SaveChanges();
                        }


                        bool isNeedBuy = true;
                        decimal chargeAmount = order.ChargeAmount;
                        if (chargeAmount <= 0)
                        {
                            isNeedBuy = false;
                        }

                        string[] testClientId = new string[2] { "62c587c13c124f96b436de9522fb31f0", "4faecb3507aa48698405cf492dc26916" };

                        if (isNeedBuy)
                        {
                            if (testClientId.Contains(order.ClientId))
                            {
                                chargeAmount = 0.01m;
                            }

                            string goods_tag = "";
                            string prepayId = SdkFactory.Wx.Instance().GetPrepayId(operater, "JSAPI", wxUserInfo.OpenId, order.Sn, chargeAmount, goods_tag, Common.CommonUtil.GetIP(), "商品购买", order.PayExpireTime);
                            if (string.IsNullOrEmpty(prepayId))
                            {
                                LogUtil.Error("去结算，微信支付中生成预支付订单失败");
                                return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Failure, ResultCode.Failure, "抢购失败，刷新页面再试试", null);
                            }

                            order.WxPrepayId = prepayId;
                            order.PayExpireTime = this.DateTime.AddMinutes(2);

                            Task4Factory.Tim2Global.Enter(TimerTaskType.CheckOrderPay, order.PayExpireTime.Value, order);
                        }
                        else
                        {
                            BizFactory.Order.PayCompleted(operater, order.Sn, this.DateTime);
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
                }

                return result;
            }
            catch (Exception ex)
            {
                LogUtil.Error("检查下单发生异常", ex);

                return new CustomJsonResult<RetOrderUnifiedOrder>(ResultType.Exception, ResultCode.Exception, "下单发生异常", null);
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
                order.IsInVisiable = false;

                var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == order.Id).ToList();
                foreach (var item in orderDetails)
                {
                    item.Status = Enumeration.OrderDetailsStatus.Payed;

                    if (order.PromoteId != null)
                    {
                        var promoteSku = CurrentDb.PromoteSku.Where(m => m.PromoteId == order.PromoteId && m.SkuId == item.SkuId).FirstOrDefault();
                        if (promoteSku != null)
                        {
                            if (promoteSku.StockQuantity > 0)
                            {
                                promoteSku.LockQuantity -= item.Quantity;
                                promoteSku.StockQuantity -= item.Quantity;
                            }

                            promoteSku.SaleQuantity += item.Quantity; ;

                            if (promoteSku.IsCoupon)
                            {
                                var clientCoupon = CurrentDb.ClientCoupon.Where(m => m.ClientId == order.ClientId && m.PromoteId == order.PromoteId && m.SkuId == promoteSku.SkuId).FirstOrDefault();
                                if (clientCoupon == null)
                                {
                                    clientCoupon = new ClientCoupon();
                                    clientCoupon.Id = GuidUtil.New();
                                    clientCoupon.ClientId = order.ClientId;
                                    clientCoupon.PromoteId = order.PromoteId;
                                    clientCoupon.SkuId = promoteSku.SkuId;
                                    clientCoupon.WxCouponId = promoteSku.WxCouponId;
                                    clientCoupon.IsBuy = true;
                                    clientCoupon.BuyTime = this.DateTime;
                                    clientCoupon.IsGet = false;
                                    clientCoupon.IsConsume = false;
                                    clientCoupon.Creator = pOperater;
                                    clientCoupon.CreateTime = this.DateTime;
                                    clientCoupon.RefereerId = order.RefereerId;
                                    clientCoupon.OrderId = order.Id;
                                    clientCoupon.OrderSn = order.Sn;

                                    if (!string.IsNullOrEmpty(promoteSku.ExtAtrrs))
                                    {
                                        CouponModel couponModel = null;
                                        try
                                        {
                                            couponModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CouponModel>(promoteSku.ExtAtrrs);
                                        }
                                        catch
                                        {
                                            couponModel = null;
                                        }

                                        if (couponModel != null)
                                        {
                                            clientCoupon.Name = couponModel.Name;
                                            clientCoupon.Number = couponModel.Number;
                                            clientCoupon.NumberType = couponModel.NumberType;
                                            clientCoupon.NumberUnit = couponModel.NumberUnit;
                                            clientCoupon.ValidStartTime = couponModel.ValidStartTime;
                                            clientCoupon.ValidEndTime = couponModel.ValidEndTime;
                                            clientCoupon.Description = couponModel.Description;
                                            clientCoupon.Discounttip = couponModel.Discounttip;
                                        }
                                    }

                                    CurrentDb.ClientCoupon.Add(clientCoupon);
                                    CurrentDb.SaveChanges();
                                }
                            }
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();


                if (!string.IsNullOrEmpty(order.RefereerId))
                {
                    var promoteRefereerRewardSets = CurrentDb.PromoteRefereerRewardSet.Where(m => m.PromoteId == order.PromoteId && m.Channel == Enumeration.PromoteRefereerRewardSetChannel.BuyerBuyProductSku).ToList();
                    if (promoteRefereerRewardSets.Count > 0)
                    {
                        var handlePms = new PromoteRefereerRewardByBuyerBuyProductSkuModel();
                        handlePms.OrderId = order.Id;
                        handlePms.ClientId = order.ClientId;
                        handlePms.PromoteId = order.PromoteId;
                        handlePms.RefereerId = order.RefereerId;

                        ReidsMqFactory.Global.Push(RedisMqHandleType.PromoteRefereerRewardByBuyerBuyProductSku, handlePms);
                    }
                }

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, string.Format("支付完成通知：订单号({0})通知成功", pOrderSn));
            }

            return result;
        }

        public CustomJsonResult Cancle(string pOperater, string pOrderId, string cancelReason)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var order = CurrentDb.Order.Where(m => m.Id == pOrderId).FirstOrDefault();
                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单");
                }

                if (order.Status == Enumeration.OrderStatus.Payed)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经支付成功");
                }

                if (order.Status == Enumeration.OrderStatus.Completed)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经完成");
                }

                if (order.Status == Enumeration.OrderStatus.WaitPay)
                {
                    order.Status = Enumeration.OrderStatus.Cancled;
                    order.CancledTime = this.DateTime;
                    order.CancelReason = cancelReason;
                    order.Mender = GuidUtil.Empty();
                    order.MendTime = this.DateTime;

                    var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var item in orderDetails)
                    {
                        item.Status = Enumeration.OrderDetailsStatus.Cancled;
                        item.Mender = GuidUtil.Empty();
                        item.MendTime = this.DateTime;

                        if (!string.IsNullOrEmpty(item.PromoteId))
                        {
                            var promoteSku = CurrentDb.PromoteSku.Where(q => q.PromoteId == item.PromoteId && q.SkuId == item.SkuId).FirstOrDefault();
                            if (promoteSku != null)
                            {
                                if (promoteSku.StockQuantity > 0)
                                {
                                    promoteSku.LockQuantity -= item.Quantity;
                                    promoteSku.SellQuantity += item.Quantity;
                                }

                                promoteSku.Mender = GuidUtil.Empty();
                                promoteSku.MendTime = this.DateTime;
                            }
                        }
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                }

                return result;
            }
        }
    }
}
