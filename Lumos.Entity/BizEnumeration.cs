using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{
    /// <summary>
    /// 业务的枚举
    /// </summary>
    public partial class Enumeration
    {


        public enum BizSnType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("入库单号")]
            Order2StockIn = 1
        }

        public enum OrderNotifyLogNotifyFrom
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("网站")]
            WebApp = 1,
            [Remark("微信支付配置通知链接")]
            NotifyUrl = 2,
            [Remark("微信订单查询接口")]
            OrderQuery = 3
        }

        public enum WxUserInfoFrom
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("授权")]
            Authorize = 1,
            [Remark("订阅")]
            Subscribe = 2
        }

        public enum WxAutoReplyType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("订阅")]
            Subscribe = 1,
            [Remark("关键字")]
            Keyword = 2,
            [Remark("菜单点击")]
            MenuClick = 3,
            [Remark("不是关键字")]
            NotKeyword = 4
        }
    }
}
