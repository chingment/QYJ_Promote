using Lumos.DAL;
using Lumos.Entity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL
{
    public enum ReidsMqByCalProfitType
    {
        [Remark("未知")]
        Unknow = 0,
        [Remark("优惠卷核销")]
        CouponConsume = 1
    }

    public class ReidsMqByCalProfitByCouponConsumeModel
    {
        public string ClientId { get; set; }
        public string WxCouponId { get; set; }
        public string WxCouponDecryptCode { get; set; }

    }

    public class ReidsMqByCalProfitModel
    {
        public ReidsMqByCalProfitType Type { get; set; }

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
                        using (LumosDbContext CurrentDb = new LumosDbContext())
                        {
                            using (TransactionScope ts = new TransactionScope())
                            {
                                switch (this.Type)
                                {
                                    case ReidsMqByCalProfitType.CouponConsume:

                                        var model = ((JObject)this.Pms).ToObject<ReidsMqByCalProfitByCouponConsumeModel>();
                                        var strjson_model = Newtonsoft.Json.JsonConvert.SerializeObject(model);

                                        string msg = string.Format("正在处理信息，消息类型为佣金计算-{0},具体参数：{1}", this.Type.GetCnName(), strjson_model);
                                        LogUtil.Info(msg);
                                        Console.WriteLine(msg);

                                        var promoteUserCoupon = CurrentDb.PromoteUserCoupon.Where(m => m.ClientId == model.ClientId && m.WxCouponId == model.WxCouponId && m.WxCouponDecryptCode == model.WxCouponDecryptCode).FirstOrDefault();

                                        if (promoteUserCoupon == null)
                                        {
                                            LogUtil.Info("用户:" + model.ClientId + ",找不到卡券");
                                        }

                                        if (promoteUserCoupon.RefereeId == null)
                                        {
                                            LogUtil.Info("用户:" + model.ClientId + ",推荐人为空");

                                            return;
                                        }

                                        if (promoteUserCoupon.ClientId == promoteUserCoupon.RefereeId)
                                        {
                                            LogUtil.Info("用户和推荐人是同一个人:" + model.ClientId );
                                            return;
                                        }

                                        if (promoteUserCoupon.IsConsume == true)
                                        {
                                            LogUtil.Info("用户:" + model.ClientId + ",已核销");
                                            return;
                                        }

                                        promoteUserCoupon.IsConsume = true;
                                        promoteUserCoupon.ConsumeTime = DateTime.Now;
                                        promoteUserCoupon.Mender = GuidUtil.Empty();
                                        promoteUserCoupon.MendTime = DateTime.Now;

                                        var fund = CurrentDb.Fund.Where(m => m.ClientId == promoteUserCoupon.RefereeId).FirstOrDefault();
                                        if (fund == null)
                                        {
                                            return;
                                        }

                                        var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientId == promoteUserCoupon.ClientId).FirstOrDefault();
                                        string nickname = "";
                                        string headImgUrl = IconUtil.ConsumeCoupon;
                                        if (wxUserInfo != null)
                                        {
                                            nickname = wxUserInfo.Nickname;

                                            if(!string.IsNullOrEmpty(wxUserInfo.HeadImgUrl))
                                            {
                                                headImgUrl = wxUserInfo.HeadImgUrl;
                                            }
                                        }

                                        decimal profit = 500m;
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


                                        break;

                                }

                                CurrentDb.SaveChanges();
                                ts.Complete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtil.Error("消息队列，处理信息-佣金计算发生异常", ex);
                    }
                }
            }
        }
    }
}
