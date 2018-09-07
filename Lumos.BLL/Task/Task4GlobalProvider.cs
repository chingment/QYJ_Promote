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


            var orders = CurrentDb.Order.Where(m => m.Status == Enumeration.OrderStatus.WaitPay && m.WxPrepayIdExpireTime >= DateTime.Now).ToList();

            LogUtil.Info(string.Format("共有{0}条待支付", orders.Count));
            LogUtil.Info(string.Format("开始执行订单查询,时间:{0}", this.DateTime));
            foreach (var m in orders)
            {
                LogUtil.Info(string.Format("开始执行订单号{0}，时间:{1}", m.Sn, this.DateTime));

                string xml = SdkFactory.Wx.Instance().OrderQuery(m.Sn);

                LogUtil.Info(string.Format("查询订单号（{0}）的结果文件:{1}", m.Sn, xml));

                BizFactory.Order.PayResultNotify(GuidUtil.Empty(), Entity.Enumeration.OrderNotifyLogNotifyFrom.OrderQuery, xml, m.Sn);

                LogUtil.Info(string.Format("结束执行订单号{0}，时间:{1}", m.Sn, this.DateTime));
            }

            LogUtil.Info(string.Format("结束执行订单查询,时间:{0}", this.DateTime));


            CurrentDb.SaveChanges();


            return result;
        }
    }
}
