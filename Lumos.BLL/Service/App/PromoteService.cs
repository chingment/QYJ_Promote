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

            var promote = CurrentDb.Promote.Where(m => m.Id == rup.PromoteId).FirstOrDefault();

            var promoteSku = CurrentDb.PromoteSku.Where(m => m.PromoteId == rup.PromoteId).FirstOrDefault();

            var ret = new RetPromoteGetConfig();
            ret.PromoteId = rup.PromoteId;
            ret.PromoteSkuId = promoteSku.Id;
            ret.ClientId = pClientId;
            ret.RefereeId = rup.RefereeId;

            if (rup.PromoteId == "akkk753c5fe14e26bbecad576b6a6kkk")
            {
                #region akkk753c5fe14e26bbecad576b6a6kkk
                ret.ShareTitle = "全优加周年庆福利，¥1248元优乐享卡";
                ret.ShareDesc = "限量1028张，60校同抢\n免费送！送！送！";
                ret.ShareImgUrl = "http://qyj.17fanju.com/Content/images/promote20181021/share_icon.png";
                ret.EndDesc = "活动已经结束";
                ret.NoStartDesc = "活动未开始";


                //ret.CouponPage.Title = "全优加周年庆福利，¥1248元优乐享卡，免费送！送！送！";
                //ret.CouponPage.Bg01.Src = "/Content/images/promote20181021/bg_01.jpg";
                //ret.CouponPage.Bg02 = "/Content/images/promote20181021/bg_02.jpg";
                //ret.CouponPage.Bg03 = "/Content/images/promote20181021/bg_03.jpg";
                ////ret.CouponPage.Bg04 = "/Content/images/promote20181021/bg_04.jpg";
                //ret.CouponPage.Bg4GoBuy = "/Content/images/promote20180910/btn_buy.png";
                ////ret.CouponPage.Bg4OpenCoupon = "/Content/images/promote20181021/btn_opencoupon.png";
                ////ret.CouponPage.Bg4GetCoupon = "/Content/images/promote20181021/btn_getcoupon.png";
                //ret.CouponPage.Bg4GoPersonal = "/Content/images/promote20181021/btn_personal.png";
                //ret.CouponPage.Bg4GoInvite = "/Content/images/promote20181021/btn_invite.png";

                //ret.PayResultPage.Title = "恭喜您，抢购成功！";
                //ret.PayResultPage.Bg01 = "/Content/images/promote20181021/bg_suc_01.jpg";
                //ret.PayResultPage.Bg02 = "/Content/images/promote20181021/bg_suc_02.jpg";
                //ret.PayResultPage.Bg03 = "/Content/images/promote20181021/bg_suc_03.jpg";
                //ret.PayResultPage.Bg04 = "/Content/images/promote20181021/bg_suc_04.jpg";
                ////ret.PayResultPage.Bg4GoBuy = "/Content/images/promote20180910/btn_buy.png";
                //ret.PayResultPage.Bg4OpenCoupon = "/Content/images/promote20181021/btn_opencoupon.png";
                //ret.PayResultPage.Bg4GetCoupon = "/Content/images/promote20181021/btn_getcoupon.png";
                //ret.PayResultPage.Bg4GoPersonal = "/Content/images/promote20181021/btn_personal.png";
                //ret.PayResultPage.Bg4GoInvite = "/Content/images/promote20181021/btn_invite.png";
                #endregion
            }
            else if (rup.PromoteId == "80c71a0657924059b39895f9e406ef84")
            {
                #region 80c71a0657924059b39895f9e406ef84
                ret.ShareTitle = "超值推荐|¥2480元的早教大课包，双11秒杀仅需¥1111元";
                ret.ShareDesc = "数量有限\n马上预购双11入场券\n获取秒杀资格吧！";
                ret.ShareImgUrl = "http://qyj.17fanju.com/Content/images/promote20181029/share_icon.png";
                ret.EndDesc = "活动已经结束";
                ret.NoStartDesc = "活动未开始";


                int sumHeight = 1920;
                ret.CouponPage.Title = "超值推荐|¥2480元的早教大课包，双11秒杀仅需¥1111元";
                ret.CouponPage.Bg01.Src = "/Content/images/promote20181029/bg_01.png";
                ret.CouponPage.Bg01.Height = GetHeight(sumHeight, rup.ScreenHeight, 1020);
                ret.CouponPage.Bg02.Src = "/Content/images/promote20181029/bg_02.jpg";
                ret.CouponPage.Bg02.Height = GetHeight(sumHeight, rup.ScreenHeight, 200);
                ret.CouponPage.Bg03.Src = "/Content/images/promote20181029/bg_03.png";
                ret.CouponPage.Bg03.Height = GetHeight(sumHeight, rup.ScreenHeight, 700);
                ret.CouponPage.Bg4GoBuy.Src = "/Content/images/promote20181029/bg_btn_buy.png";
                ret.CouponPage.Bg4GoPersonal.Src = "/Content/images/promote20181029/bg_btn_personal.png";
                ret.CouponPage.Bg4GoInvite.Src = "/Content/images/promote20181029/bg_btn_invite.png";

                ret.PayResultPage.Title = "恭喜您，抢购成功！";
                ret.PayResultPage.Bg01.Src = "/Content/images/promote20181029/bg_suc_01.jpg";
                ret.PayResultPage.Bg01.Height = GetHeight(sumHeight, rup.ScreenHeight, 1177);
                ret.PayResultPage.Bg02.Src = "/Content/images/promote20181029/bg_suc_02.jpg";
                ret.PayResultPage.Bg02.Height = GetHeight(sumHeight, rup.ScreenHeight, 200);
                ret.PayResultPage.Bg03.Src = "/Content/images/promote20181029/bg_suc_03.jpg";
                ret.PayResultPage.Bg03.Height = GetHeight(sumHeight, rup.ScreenHeight, 260);
                ret.PayResultPage.Bg04.Src = "/Content/images/promote20181029/bg_suc_04.jpg";
                ret.PayResultPage.Bg04.Height = GetHeight(sumHeight, rup.ScreenHeight, 283);
                ret.PayResultPage.Bg4OpenCoupon.Src = "/Content/images/promote20181029/bg_suc_btn_opencoupon.png";
                ret.PayResultPage.Bg4GetCoupon.Src = "/Content/images/promote20181029/bg_suc_btn_getcoupon.png";
                ret.PayResultPage.Bg4GoPersonal.Src = "/Content/images/promote20181029/bg_suc_btn_personal.png";
                ret.PayResultPage.Bg4GoInvite.Src = "/Content/images/promote20181029/bg_suc_btn_invite.png";
                #endregion
            }

            if (promote == null)
            {
                ret.Status = 1;//活动未开始
            }
            else
            {
                if (promoteSku.BuyStartTime > this.DateTime)
                {
                    ret.Status = 1;//活动未开始
                }
                else if (promoteSku.BuyStartTime <= this.DateTime && promoteSku.BuyEndTime >= this.DateTime)
                {
                    ret.Status = 2;//活动中
                }
                else
                {
                    ret.Status = 3;//活动结束
                }

                var promoteUserCoupon = CurrentDb.ClientCoupon.Where(m => m.ClientId == pClientId && m.PromoteId == rup.PromoteId && m.PromoteSkuId == promoteSku.Id).FirstOrDefault();
                if (promoteUserCoupon == null)
                {
                    ret.IsGet = false;
                    ret.IsBuy = false;
                }
                else
                {
                    ret.IsGet = promoteUserCoupon.IsGet;
                    ret.IsBuy = promoteUserCoupon.IsBuy;
                    ret.OrderSn = promoteUserCoupon.OrderSn;
                }
            }

            ret.IsNeedBuy = promote.IsNeedBuy;

            var promoteUser = CurrentDb.PromoteUser.Where(m => m.ClientId == pClientId && m.CtPhone != null).OrderByDescending(m => m.CreateTime).FirstOrDefault();
            if (promoteUser != null)
            {
                ret.UserInfo.CtName = promoteUser.CtName ?? "";
                ret.UserInfo.CtPhone = promoteUser.CtPhone ?? "";
                ret.UserInfo.CtIsStudent = promoteUser.CtIsStudent ?? "";
                ret.UserInfo.CtSchool = promoteUser.CtSchool ?? "";
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "预定成功", ret);

            return result;
        }

        public static int GetHeight(int sumHeight, int sHeight, int oHeight)
        {
            double scale = Convert.ToDouble(Convert.ToDouble(oHeight) / Convert.ToDouble(sumHeight));
            double a = sHeight * scale;
            return Convert.ToInt32(a);
        }
    }
}
