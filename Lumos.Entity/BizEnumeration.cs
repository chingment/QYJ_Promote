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
            Withraw = 3,
            [Remark("赠品派送流水号")]
            GiftGiveTrans = 4
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

        public enum GiftGiveTransType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("参与报名赠送")]
            SignupGift = 1
        }

        public enum PromoteRefereerRewardSetValueType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("按消费金额的固定金额")]
            AmountByFixed = 1,
            [Remark("按消费金额的消费比例")]
            AmountByRate = 2,
            [Remark("商品库的商品")]
            GiveBySku = 3
        }

        public enum PromoteRefereerRewardSetChannel
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("购买者购买商品")]
            BuyerBuyProductSku = 1,
            [Remark("购买者核销商品")]
            BuyerConsumeProductSku = 2
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
            [Remark("抢购卡券")]
            BuyCoupon = 1,
            [Remark("限时抢购")]
            BuyProductSkuByFlashSale = 2,
            [Remark("礼品赠送")]
            GiftGive = 3
        }

        public enum PromoteTargetType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("全部")]
            All = 1,
            [Remark("学员")]
            Student = 2,
            [Remark("非学员")]
            NotStudent = 3
        }

        public enum PromoteSkuGetType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("购买")]
            Buy = 1,
            [Remark("购买赠送")]
            BuyGiveRefereer = 2
        }


    }
}
