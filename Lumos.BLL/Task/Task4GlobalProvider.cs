using Lumos.DAL;
using Lumos.Entity;
using Lumos.Mvc;
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
    public class Task4GlobalProvider : BaseProvider, ITask
    {
        public CustomJsonResult Run()
        {
            CustomJsonResult result = new CustomJsonResult();

            #region 检查支付状态
            var orders = OrderCacheUtil.GetCheckPayStatusQueue();
            LogUtil.Info(string.Format("共有{0}条待支付订单查询状态", orders.Count));

            LogUtil.Info(string.Format("开始执行订单查询,时间：{0}", this.DateTime));
            foreach (var m in orders)
            {
                LogUtil.Info(string.Format("查询订单号：{0}", m.Sn));

                if (m.WxPrepayIdExpireTime != null)
                {
                    if (m.WxPrepayIdExpireTime.Value.AddMinutes(1) >= DateTime.Now)
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
                        var order = CurrentDb.Order.Where(q => q.Sn == m.Sn).FirstOrDefault();
                        if (order != null)
                        {
                            order.Status = Enumeration.OrderStatus.Cancled;
                            order.Mender = GuidUtil.Empty();
                            order.MendTime = this.DateTime;
                            order.CancelReason = "订单支付有效时间过期";
                            CurrentDb.SaveChanges();
                            OrderCacheUtil.ExitQueue4CheckPayStatus(m.Sn);
                            LogUtil.Info(string.Format("订单号：{0},已经过期,删除缓存", m.Sn));
                        }
                    }
                }
            }

            LogUtil.Info(string.Format("结束执行订单查询,时间:{0}", this.DateTime));
            #endregion


            #region 检查优惠卷核销状态


            var promoteUserCoupons = CurrentDb.PromoteUserCoupon.Where(m => m.IsConsume == true).ToList();

            foreach (var item in promoteUserCoupons)
            {
                var profit = 500m;
                var fund = CurrentDb.Fund.Where(m => m.UserId == item.RefereeId).FirstOrDefault();
                fund.CurrentBalance += profit;
                fund.AvailableBalance += profit;
                fund.Mender = GuidUtil.New();
                fund.MendTime = this.DateTime;

                var fundTrans = new FundTrans();
                fundTrans.Id = GuidUtil.New();
                fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, item.RefereeId);
                fundTrans.UserId = item.UserId;
                fundTrans.CurrentBalance = fund.CurrentBalance;
                fundTrans.ChangeAmount = profit;
                fundTrans.ChangeType = Enumeration.FundTransChangeType.ConsumeCoupon;
                fundTrans.CreateTime = this.DateTime;
                fundTrans.Creator = GuidUtil.New();
                fundTrans.Description = string.Format("您推荐的用户核销优惠券，得到奖金:{0}元", profit.ToF2Price());
                CurrentDb.FundTrans.Add(fundTrans);


            }


            #endregion

            return result;
        }
    }
}
