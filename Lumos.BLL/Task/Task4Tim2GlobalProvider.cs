using Lumos.BLL.Biz;
using Lumos.BLL.Sdk;
using Lumos.DAL;
using Lumos.Entity;
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
    public class Task4Tim2GlobalProvider : BaseProvider, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            #region 检查支付状态
            try
            {
                #region 检查支付状态
                var orders = OrderCacheUtil.GetCheckPayStatusQueue();
                LogUtil.Info(string.Format("共有{0}条待支付订单查询状态", orders.Count));
                LogUtil.Info(string.Format("开始执行订单查询,时间：{0}", this.DateTime));
                foreach (var m in orders)
                {
                    LogUtil.Info(string.Format("查询订单号：{0}", m.Sn));

                    if (m.PayExpireTime != null)
                    {
                        if (m.PayExpireTime.Value.AddMinutes(1) >= DateTime.Now)
                        {
                            string xml = SdkFactory.Wx.Instance().OrderQuery(m.Sn);
                            LogUtil.Info(string.Format("订单号：{0},结果文件:{1}", m.Sn, xml));
                            bool isPaySuccessed = false;
                            BizFactory.Order.PayResultNotify(GuidUtil.Empty(), Entity.Enumeration.OrderNotifyLogNotifyFrom.OrderQuery, xml, m.Sn, out isPaySuccessed);
                            if (isPaySuccessed)
                            {
                                OrderCacheUtil.ExitQueue4CheckPayStatus(m.Sn);
                                LogUtil.Info(string.Format("订单号：{0},支付成功,删除缓存", m.Sn));
                            }
                        }
                        else
                        {
                            var rt = BizFactory.Order.Cancle(GuidUtil.Empty(), m.Id, "订单支付有效时间过期");
                            if (rt.Result == ResultType.Success)
                            {
                                OrderCacheUtil.ExitQueue4CheckPayStatus(m.Sn);
                                LogUtil.Info(string.Format("订单号：{0},已经过期,删除缓存", m.Sn));
                            }
                        }
                    }
                }

                LogUtil.Info(string.Format("结束执行订单查询,时间:{0}", this.DateTime));
                #endregion

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
            #endregion
        }
    }
}
