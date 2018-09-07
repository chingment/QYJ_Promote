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
                        OrderCacheUtil.ExitQueue4CheckPayStatus(m.Sn);

                        LogUtil.Info(string.Format("订单号：{0},已经过期,删除缓存", m.Sn));
                    }
                }
            }

            LogUtil.Info(string.Format("结束执行订单查询,时间:{0}", this.DateTime));


            return result;
        }
    }
}
