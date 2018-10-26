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


            ret.ClientId = pClientId;
            ret.RefereeId = rup.RefereeId;
            ret.ShareTitle = "全优加周年庆福利，¥1248元优乐享卡";
            ret.ShareDesc = "限量1028张，60校同抢\n免费送！送！送！";
            ret.ShareImgUrl = "http://qyj.17fanju.com/Content/images/promote20181021/share_icon.png";
            ret.EndDesc = "活动已经结束";
            ret.NoStartDesc = "活动未开始";

            ret.CouponPage.Title = "全优加周年庆福利，¥1248元优乐享卡，免费送！送！送！";
            ret.CouponPage.Bg01 = "/Content/images/promote20181021/bg_01.jpg";
            ret.CouponPage.Bg02 = "/Content/images/promote20181021/bg_02.jpg";
            ret.CouponPage.Bg03 = "/Content/images/promote20181021/bg_03.jpg";
            ret.CouponPage.Bg04 = "/Content/images/promote20181021/bg_04.jpg";
            ret.CouponPage.Bg4GoBuy = "/Content/images/promote20180910/btn_buy.png";
            ret.CouponPage.Bg4OpenCoupon = "/Content/images/promote20181021/btn_opencoupon.png";
            ret.CouponPage.Bg4GetCoupon = "/Content/images/promote20181021/btn_getcoupon.png";
            ret.CouponPage.Bg4GoPersonal = "/Content/images/promote20181021/btn_personal.png";
            ret.CouponPage.Bg4GoInvite = "/Content/images/promote20181021/btn_invite.png";

            ret.PayResultPage.Title = "恭喜您，抢购成功！";
            ret.PayResultPage.Bg01 = "/Content/images/promote20181021/bg_suc_01.jpg";
            ret.PayResultPage.Bg02 = "/Content/images/promote20181021/bg_suc_02.jpg";
            ret.PayResultPage.Bg03 = "/Content/images/promote20181021/bg_suc_03.jpg";
            ret.PayResultPage.Bg04 = "/Content/images/promote20181021/bg_suc_04.jpg";
            ret.PayResultPage.Bg4GoBuy = "/Content/images/promote20180910/btn_buy.png";
            ret.PayResultPage.Bg4OpenCoupon = "/Content/images/promote20181021/btn_opencoupon.png";
            ret.PayResultPage.Bg4GetCoupon = "/Content/images/promote20181021/btn_getcoupon.png";
            ret.PayResultPage.Bg4GoPersonal = "/Content/images/promote20181021/btn_personal.png";
            ret.PayResultPage.Bg4GoInvite = "/Content/images/promote20181021/btn_invite.png";

            var promote = CurrentDb.Promote.Where(m => m.Id == rup.PromoteId).FirstOrDefault();
            if (promote == null)
            {
                ret.Status = 1;//活动未开始
            }
            else
            {
                if (promote.StartTime > this.DateTime)
                {
                    ret.Status = 1;//活动未开始
                }
                else if (promote.StartTime <= this.DateTime && promote.EndTime >= this.DateTime)
                {
                    ret.Status = 2;//活动中
                }
                else
                {
                    ret.Status = 3;//活动结束
                }

                var promoteUserCoupon = CurrentDb.PromoteUserCoupon.Where(m => m.ClientId == pClientId && m.PromoteId == rup.PromoteId && m.PromoteCouponId == ret.PromoteCouponId).FirstOrDefault();
                if (promoteUserCoupon == null)
                {
                    ret.IsGet = false;
                }
                else
                {
                    ret.IsGet = promoteUserCoupon.IsGet;
                }
            }

            ret.IsNeedBuy = false;

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "预定成功", ret);

            return result;
        }
    }
}
