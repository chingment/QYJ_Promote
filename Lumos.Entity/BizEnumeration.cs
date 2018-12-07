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
            [Remark("订单号")]
            Order = 1,
            [Remark("资金流水号")]
            FundTrans = 2,
            [Remark("提现单号")]
            Withraw = 3
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

        public enum OrderNotifyLogNotifyType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("支付通知")]
            Pay = 1,
            [Remark("退款通知")]
            ReFund = 2
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

        public enum OrderStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("已提交")]
            Submitted = 1,
            [Remark("待支付")]
            WaitPay = 2,
            [Remark("已支付")]
            Payed = 3,
            [Remark("已完成")]
            Completed = 4,
            [Remark("已失效")]
            Cancled = 5

        }

        public enum OrderDetailsStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("已提交")]
            Submitted = 1,
            [Remark("待支付")]
            WaitPay = 2,
            [Remark("已支付")]
            Payed = 3,
            [Remark("已完成")]
            Completed = 4,
            [Remark("已失效")]
            Cancled = 5

        }

        public enum FundTransChangeType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("购买商品")]
            PurchaseGoods = 1,
            [Remark("核销优惠卷")]
            ConsumeCoupon = 2,
            [Remark("提现申请")]
            WtihdrawApply = 3,
            [Remark("提现成功")]
            WtihdrawSuccess = 4,
            [Remark("提现失败")]
            WtihdrawFailure = 5,
            [Remark("购买优惠卷")]
            BuyCoupon = 6,
            [Remark("提现过期")]
            WtihdrawExpire = 7
        }

        public enum PromoteProfitSetValueType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("固定金额")]
            FixedAmount = 1,
            [Remark("比例")]
            RateAmount = 2
        }

        public enum WithdrawStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("申请")]
            Apply = 1,
            [Remark("处理中")]
            Handing = 2,
            [Remark("成功")]
            Success = 3,
            [Remark("失败")]
            Failure = 4
        }

        public enum PromoteClass
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("卡券")]
            Coupon = 1,
            [Remark("商品")]
            ProductSku = 2,
        }
    }
}
