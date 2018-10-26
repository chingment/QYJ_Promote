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
            ret.ShareImgUrl = "http://qyj.17fanju.com/Content/images/promote20181021/share_icon.png";
            ret.EndDesc = "活动已经结束";
            ret.NoStartDesc = "活动未开始";
            ret.Bg01 = "/Content/images/promote20181021/bg_01.jpg";
            ret.Bg02 = "/Content/images/promote20181021/bg_02.jpg";
            ret.Bg03 = "/Content/images/promote20181021/bg_03.jpg";
            ret.Bg04 = "/Content/images/promote20181021/bg_04.jpg";
            ret.Bg4GoBuy = "/Content/images/promote20180910/btn_buy.png";
            ret.Bg4OpenCoupon = "/Content/images/promote20181021/btn_opencoupon.png";
            ret.Bg4GetCoupon = "/Content/images/promote20181021/btn_getcoupon.png";
            ret.Bg4GoPersonal = "/Content/images/promote20181021/btn_personal.png";
            ret.Bg4GoInvite = "/Content/images/promote20181021/btn_invite.png";


            //var promoteUserCoupon = CurrentDb.PromoteUserCoupon.Where(m => m.ClientId == this.CurrentUserId && m.PromoteId == model.PromoteId && m.PromoteCouponId == model.PromoteCouponId).FirstOrDefault();
            //if (promoteUserCoupon != null)
            //{
            //    if (promoteUserCoupon.IsBuy)
            //    {
            //        if (promoteUserCoupon.IsGet)
            //        {
            //            return Redirect("~/Promoteb/PayResult?promoteId=" + model.PromoteId + "&orderSn=" + promoteUserCoupon.OrderSn + "&isSuccessed=True");
            //        }
            //    }
            //}

            //var promote = CurrentDb.Promote.Where(m => m.Id == model.PromoteId).FirstOrDefault();
            //if (promote != null)
            //{
            //    if (promote.EndTime < DateTime.Now)
            //    {
            //        model.PromoteIsEnd = true;
            //    }
            //}

            var promote = CurrentDb.Promote.Where(m => m.Id == rup.PromoteId).FirstOrDefault();
            if (promote == null)
            {
                ret.IsStart = false;
                ret.IsEnd = false;
            }
            else
            {
                if (promote.StartTime < this.DateTime)
                {
                    ret.IsStart = true;

                    if (promote.EndTime > this.DateTime)
                    {
                        ret.IsEnd = false;
                    }
                    else
                    {
                        ret.IsEnd = true;
                    }
                }
                else
                {
                    ret.IsStart = false;
                    ret.IsEnd = false;
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
