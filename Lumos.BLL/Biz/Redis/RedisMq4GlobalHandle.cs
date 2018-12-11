using Lumos.DAL;
using Lumos.Entity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Biz
{
    public enum RedisMqHandleType
    {
        [Remark("未知")]
        Unknow = 0,
        [Remark("优惠卷核销")]
        CouponConsume = 1,
        [Remark("优惠卷购买")]
        CouponBuy = 2
    }

    public class ReidsMqByCalProfitByCouponConsumeModel
    {
        public string ClientId { get; set; }
        public string WxCouponId { get; set; }
        public string WxCouponDecryptCode { get; set; }

    }

    public class ReidsMqByCalProfitByCouponBuyModel
    {
        public string OrderId { get; set; }
        public string ClientId { get; set; }
        public string PromoteId { get; set; }
        public string RefereeId { get; set; }
        public string ClientCouponId { get; set; }

    }

    public class RedisMq4GlobalHandle
    {
        public RedisMqHandleType Type { get; set; }

        public object Pms { get; set; }

        private static readonly object lock_Handle = new object();

        public void Handle()
        {
            lock (lock_Handle)
            {
                if (this.Pms != null)
                {
                    try
                    {
                        switch (this.Type)
                        {
                            case RedisMqHandleType.CouponConsume:
                                CouponConsume();
                                break;
                            case RedisMqHandleType.CouponBuy:
                                CouponBuy();
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtil.Error("消息队列，处理信息-佣金计算发生异常", ex);
                    }
                }
            }
        }

        private void CouponConsume()
        {
            using (LumosDbContext CurrentDb = new LumosDbContext())
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    #region 核销优惠券
                    var model = ((JObject)this.Pms).ToObject<ReidsMqByCalProfitByCouponConsumeModel>();
                    var strjson_model = Newtonsoft.Json.JsonConvert.SerializeObject(model);

                    string msg = string.Format("正在处理信息，消息类型为佣金计算-{0},具体参数：{1}", this.Type.GetCnName(), strjson_model);
                    LogUtil.Info(msg);
                    Console.WriteLine(msg);

                    var clientCoupon = CurrentDb.ClientCoupon.Where(m => m.ClientId == model.ClientId && m.WxCouponId == model.WxCouponId && m.WxCouponDecryptCode == model.WxCouponDecryptCode).FirstOrDefault();

                    if (clientCoupon == null)
                    {
                        LogUtil.Info("用户:" + model.ClientId + ",找不到卡券");
                        return;
                    }

                    if (clientCoupon.RefereeId == null)
                    {
                        LogUtil.Info("用户:" + model.ClientId + ",推荐人为空");

                        return;
                    }

                    if (clientCoupon.ClientId == clientCoupon.RefereeId)
                    {
                        LogUtil.Info("用户和推荐人是同一个人:" + model.ClientId);
                        return;
                    }

                    if (clientCoupon.IsConsume == true)
                    {
                        LogUtil.Info("用户:" + model.ClientId + ",已核销");
                        return;
                    }

                    clientCoupon.IsConsume = true;
                    clientCoupon.ConsumeTime = DateTime.Now;
                    clientCoupon.Mender = GuidUtil.Empty();
                    clientCoupon.MendTime = DateTime.Now;

                    var fund = CurrentDb.Fund.Where(m => m.ClientId == clientCoupon.RefereeId).FirstOrDefault();
                    if (fund == null)
                    {
                        return;
                    }

                    var promote = CurrentDb.Promote.Where(m => m.Id == clientCoupon.PromoteId).FirstOrDefault();

                    if (promote == null)
                    {
                        return;
                    }

                    var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientId == clientCoupon.ClientId).FirstOrDefault();
                    string nickname = "";
                    string headImgUrl = IconUtil.ConsumeCoupon;
                    if (wxUserInfo != null)
                    {
                        nickname = wxUserInfo.Nickname;

                        if (!string.IsNullOrEmpty(wxUserInfo.HeadImgUrl))
                        {
                            headImgUrl = wxUserInfo.HeadImgUrl;
                        }
                    }

                    decimal profit = promote.RefereeConsumeProfit;

                    fund.CurrentBalance += profit;
                    fund.AvailableBalance += profit;
                    fund.MendTime = DateTime.Now;
                    fund.Mender = GuidUtil.Empty();

                    var fundTrans = new FundTrans();
                    fundTrans.Id = GuidUtil.New();
                    fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, fund.ClientId);
                    fundTrans.ClientId = fund.ClientId;
                    fundTrans.ChangeType = Enumeration.FundTransChangeType.ConsumeCoupon;
                    fundTrans.ChangeAmount = profit;
                    fundTrans.CurrentBalance = fund.CurrentBalance;
                    fundTrans.AvailableBalance = fund.AvailableBalance;
                    fundTrans.LockBalance = fund.LockBalance;
                    fundTrans.CreateTime = DateTime.Now;
                    fundTrans.Creator = GuidUtil.Empty();
                    fundTrans.Description = string.Format("分享给用户({0})核销优惠券", nickname);
                    fundTrans.TipsIcon = headImgUrl;
                    fundTrans.IsNoDisplay = false;
                    CurrentDb.FundTrans.Add(fundTrans);


                    CurrentDb.SaveChanges();
                    ts.Complete();

                    #endregion
                }
            }

        }

        private void CouponBuy()
        {
            using (LumosDbContext CurrentDb = new LumosDbContext())
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    #region 核销优惠券
                    var model = ((JObject)this.Pms).ToObject<ReidsMqByCalProfitByCouponBuyModel>();
                    var strjson_model = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                    Console.WriteLine("11111");
                    string msg = string.Format("正在处理信息，消息类型为佣金计算-{0},具体参数：{1}", this.Type.GetCnName(), strjson_model);
                    LogUtil.Info(msg);
                    Console.WriteLine(msg);
                    Console.WriteLine("22222");
                    var promote = CurrentDb.Promote.Where(m => m.Id == model.PromoteId).FirstOrDefault();
                    if (promote == null)
                    {
                        LogUtil.Info("活动:" + model.PromoteId + ",为空");
                        return;
                    }

                    if (model.ClientId == null)
                    {
                        LogUtil.Info("用户:" + model.ClientId + ",为空");
                        return;
                    }

                    if (model.RefereeId == null)
                    {
                        LogUtil.Info("推荐人:" + model.ClientId + ",为空");
                        return;
                    }

                    if (model.ClientId == model.RefereeId)
                    {
                        LogUtil.Info("用户和推荐人是同一个人:" + model.ClientId);
                        return;
                    }

                    var fund = CurrentDb.Fund.Where(m => m.ClientId == model.RefereeId).FirstOrDefault();

                    if (fund == null)
                    {
                        LogUtil.Info("用户:" + model.ClientId + ",找不到钱包");
                        return;
                    }

                    var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientId == model.ClientId).FirstOrDefault();

                    if (wxUserInfo == null)
                    {
                        LogUtil.Info("用户:" + model.ClientId + ",找不到信息");
                        return;
                    }

                    var order = CurrentDb.Order.Where(m => m.Id == model.OrderId).FirstOrDefault();

                    if (order == null)
                    {
                        LogUtil.Info("订单:" + model.OrderId + ",找不到信息");
                        return;
                    }

                    if (order.BuyProfitIsSettled)
                    {
                        LogUtil.Info("订单:" + model.OrderId + ",已经结算佣金");
                        return;
                    }

                    order.BuyProfitIsSettled = true;
                    order.BuyProfitSettledTime = DateTime.Now;

                    string nickname = "";
                    string headImgUrl = IconUtil.BuyCoupon;
                    if (wxUserInfo != null)
                    {
                        nickname = wxUserInfo.Nickname;

                        if (!string.IsNullOrEmpty(wxUserInfo.HeadImgUrl))
                        {
                            headImgUrl = wxUserInfo.HeadImgUrl;
                        }
                    }

                    decimal profit = promote.RefereeBuyProfit;

                    fund.CurrentBalance += profit;
                    fund.AvailableBalance += profit;
                    fund.MendTime = DateTime.Now;
                    fund.Mender = GuidUtil.Empty();

                    var fundTrans = new FundTrans();
                    fundTrans.Id = GuidUtil.New();
                    fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, fund.ClientId);
                    fundTrans.ClientId = fund.ClientId;
                    fundTrans.ChangeType = Enumeration.FundTransChangeType.BuyCoupon;
                    fundTrans.ChangeAmount = profit;
                    fundTrans.CurrentBalance = fund.CurrentBalance;
                    fundTrans.AvailableBalance = fund.AvailableBalance;
                    fundTrans.LockBalance = fund.LockBalance;
                    fundTrans.CreateTime = DateTime.Now;
                    fundTrans.Creator = GuidUtil.Empty();
                    fundTrans.Description = string.Format("分享给用户({0})购买入场券", nickname);
                    fundTrans.TipsIcon = headImgUrl;
                    fundTrans.IsNoDisplay = false;
                    CurrentDb.FundTrans.Add(fundTrans);
                    CurrentDb.SaveChanges();
                    ts.Complete();

                    #endregion
                }
            }
        }

    }
}


