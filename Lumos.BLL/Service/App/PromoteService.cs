using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class PromoteService : BaseProvider
    {
        public CustomJsonResult GetConfig(string pOperater, string pClientId, RupPromoteGetConfig rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetPromoteGetConfig();
            ret.PromoteId = rup.PromoteId;
            ret.PromoteCouponId = "00000000000000000000000000000002";
            ret.Title = "全优加周年庆福利，¥1248元优乐享卡，免费送！送！送！";
            ret.ClientId = pClientId;
            ret.RefereeId = rup.RefereeId;
            ret.ShareTitle = "全优加周年庆福利，¥1248元优乐享卡";
            ret.ShareDesc = "限量1028张，60校同抢\n免费送！送！送！";
            ret.ShareImgUrl = "http://qyj.17fanju.com/Content/images/promote20180910/share_icon.png";

            ret.IsEnd = false;
            ret.IsStart = true;

            ret.EndDesc = "活动已经结束";
            ret.NoStartDesc = "活动未开始";
            ret.IsNeedBuy = false;
            ret.Bg01 = "/Content/images/promote20181021/bg_01.jpg";
            ret.Bg02 = "/Content/images/promote20181021/bg_02.jpg";
            ret.Bg03 = "/Content/images/promote20181021/bg_03.jpg";
            ret.Bg04 = "/Content/images/promote20181021/bg_04.jpg";

            ret.Bg4GoBuy = "/Content/images/promote20180910/btn_buy.png";
            ret.Bg4OpenCoupon = "/Content/images/promote20181021/btn_opencoupon.png";
            ret.Bg4GetCoupon = "/Content/images/promote20181021/btn_getcoupon.png";
            ret.Bg4GoPersonal = "/Content/images/promote20181021/btn_personal.png";
            ret.Bg4GoInvite = "/Content/images/promote20181021/btn_invite.png";

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "预定成功", ret);

            return result;
        }
    }
}
