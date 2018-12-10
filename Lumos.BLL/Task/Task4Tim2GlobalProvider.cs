using Lumos.BLL.Biz;
using Lumos.BLL.Sdk;
using Lumos.DAL;
using Lumos.Entity;
using Lumos.Redis;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Task
{
    public enum TimerTaskType
    {
        [Remark("未知")]
        Unknow = 0,
        [Remark("检查订单支付状态")]
        CheckOrderPay = 1
    }


    public class Task4Tim2GlobalData
    {
        public string Id { get; set; }
        public TimerTaskType Type { get; set; }
        public DateTime ExpireTime { get; set; }
        public object Data { get; set; }
    }

    public class Task4Tim2GlobalProvider : BaseProvider, IJob
    {
        private static readonly string key = "task4GlobalTimer";

        public void Enter(TimerTaskType type, DateTime expireTime, object data)
        {
            var d = new Task4Tim2GlobalData();
            d.Id = GuidUtil.New();
            d.Type = type;
            d.ExpireTime = expireTime;
            d.Data = data;
            RedisManager.Db.HashSetAsync(key, d.Id, Newtonsoft.Json.JsonConvert.SerializeObject(d), StackExchange.Redis.When.Always);
        }

        private void Exit(string id)
        {
            RedisManager.Db.HashDelete(key, id);
        }

        private static List<Task4Tim2GlobalData> GetList()
        {
            List<Task4Tim2GlobalData> list = new List<Task4Tim2GlobalData>();
            var hs = RedisManager.Db.HashGetAll(key);

            var d = (from i in hs select i).ToList();

            foreach (var item in d)
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Task4Tim2GlobalData>(item.Value);
                list.Add(obj);
            }
            return list;
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var lists = GetList();

                LogUtil.Info(string.Format("共有{0}条记录需要检查状态", lists.Count));

                if (lists.Count > 0)
                {
                    LogUtil.Info(string.Format("开始执行订单查询,时间：{0}", this.DateTime));

                    foreach (var m in lists)
                    {
                        switch (m.Type)
                        {
                            case TimerTaskType.CheckOrderPay:

                                var order = m.Data.ToJsonObject<Order>();
                                #region 检查支付状态
                                if (order.PayExpireTime.Value.AddMinutes(1) >= DateTime.Now)
                                {
                                    string xml = SdkFactory.Wx.Instance().OrderQuery(order.Sn);
                                    LogUtil.Info(string.Format("订单号：{0},结果文件:{1}", order.Sn, xml));
                                    bool isPaySuccessed = false;
                                    BizFactory.Order.PayResultNotify(GuidUtil.Empty(), Entity.Enumeration.OrderNotifyLogNotifyFrom.OrderQuery, xml, order.Sn, out isPaySuccessed);
                                    if (isPaySuccessed)
                                    {
                                        Exit(m.Id);
                                        LogUtil.Info(string.Format("订单号：{0},支付成功,删除缓存", order.Sn));
                                    }
                                }
                                else
                                {
                                    Exit(m.Id);
                                    var rt = BizFactory.Order.Cancle(GuidUtil.Empty(), m.Id, "订单支付有效时间过期");
                                    if (rt.Result == ResultType.Success)
                                    {
                                        Exit(m.Id);
                                        LogUtil.Info(string.Format("订单号：{0},已经过期,删除缓存", order.Sn));
                                    }
                                }
                                #endregion

                                break;
                        }
                    }

                    LogUtil.Info(string.Format("结束执行订单查询,时间:{0}", this.DateTime));
                }


                #region 检查优惠券领取状态

                //var promoteUserCoupons = CurrentDb.PromoteUserCoupon.Where(m => m.IsGet == false).ToList();

                //foreach (var item in promoteUserCoupons)
                //{
                //    var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.UserId == item.UserId).FirstOrDefault();
                //    if (wxUserInfo != null)
                //    {
                //        var cards = SdkFactory.Wx.Instance().GetUserCards(wxUserInfo.OpenId, item.WxCouponId);

                //        if (cards != null)
                //        {
                //            var card = cards.FirstOrDefault();

                //            if (card != null)
                //            {
                //                item.WxCouponDecryptCode = card.code;
                //                item.Mender = GuidUtil.Empty();
                //                item.MendTime = this.DateTime;
                //            }

                //        }
                //    }
                //}

                //CurrentDb.SaveChanges();

                #endregion

                #region 检查提现到期时间后，用户余额归0


                //if (DateTime.Now > DateTime.Parse("2018-11-15"))
                //{

                //    var withdraws = CurrentDb.Withdraw.Where(m => m.Status == Enumeration.WithdrawStatus.Handing).ToList();

                //    foreach (var withdraw in withdraws)
                //    {
                //        var pms = new WithdrawDoTransferPms();
                //        pms.WithdrawId = withdraw.Id;
                //        pms.Operate = WithdrawDoTransferOperate.Pass;
                //        pms.AuditComments = "";
                //        BizFactory.Withdraw.DoTransfer(GuidUtil.New(), pms);
                //    }


                //    var funds = CurrentDb.Fund.Where(m => m.AvailableBalance > 0).ToList();

                //    foreach (var fund in funds)
                //    {

                //        var dAmount = fund.AvailableBalance;
                //        fund.CurrentBalance -= dAmount;
                //        fund.AvailableBalance -= dAmount;
                //        fund.MendTime = DateTime.Now;
                //        fund.Mender = GuidUtil.Empty();


                //        var fundTrans = new FundTrans();
                //        fundTrans.Id = GuidUtil.New();
                //        fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, fund.ClientId);
                //        fundTrans.ClientId = fund.ClientId;
                //        fundTrans.ChangeType = Enumeration.FundTransChangeType.WtihdrawExpire;
                //        fundTrans.ChangeAmount = dAmount;
                //        fundTrans.CurrentBalance = fund.CurrentBalance;
                //        fundTrans.AvailableBalance = fund.AvailableBalance;
                //        fundTrans.LockBalance = fund.LockBalance;
                //        fundTrans.CreateTime = DateTime.Now;
                //        fundTrans.Creator = GuidUtil.Empty();
                //        fundTrans.Description = "提现期限到期，余额清零";
                //        fundTrans.TipsIcon = IconUtil.Withdraw;
                //        fundTrans.IsNoDisplay = false;
                //        CurrentDb.FundTrans.Add(fundTrans);
                //        CurrentDb.SaveChanges();
                //    }

                //}

                #endregion
            }
            catch (Exception ex)
            {
                LogUtil.Error("全局定时任务发生异常", ex);
            }

        }
    }
}
