using Lumos;
using Lumos.BLL;
using Lumos.Entity;
using Lumos.WeiXinSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Models.Promote;

namespace WebMobile.Controllers
{
    public class PromotebController : OwnBaseController
    {
        public ActionResult Coupon(string promoteId, string refereeId)
        {
            //refereeId =00000000000000000000000000000000
            var model = new CouponViewModel();
            model.PromoteId = "akkk753c5fe14e26bbecad576b6a6kkk";
            model.PromoteCouponId = "00000000000000000000000000000001";
            model.RefereeId = refereeId;

            var promoteUserCoupon = CurrentDb.PromoteUserCoupon.Where(m => m.UserId == this.CurrentUserId && m.PromoteId == model.PromoteId && m.PromoteCouponId == model.PromoteCouponId).FirstOrDefault();
            if (promoteUserCoupon != null)
            {
                if (promoteUserCoupon.IsBuy)
                {
                    return Redirect("~/Promoteb/PayResult?promoteId=" + model.PromoteId + "&orderSn=" + promoteUserCoupon.OrderSn + "&isSuccessed=True");
                }
            }

            var promote = CurrentDb.Promote.Where(m => m.Id == model.PromoteId).FirstOrDefault();
            if (promote != null)
            {
                if (promote.EndTime < DateTime.Now)
                {
                    model.PromoteIsEnd = true;
                }
            }

            return View(model);
        }

        public ActionResult PayResult(string promoteId, string orderSn, bool isSuccessed = false)
        {
            var model = new PayResultViewModel();


            model.PromoteId = promoteId;
            model.OrderSn = orderSn;
            model.IsSuccessed = isSuccessed;


            var promote = CurrentDb.Promote.Where(m => m.Id == model.PromoteId).FirstOrDefault();
            if (promote != null)
            {
                if (promote.EndTime < DateTime.Now)
                {
                    model.PromoteIsEnd = true;
                }
            }

            bool isGoBuy = false;

            var promoteUserCoupon = CurrentDb.PromoteUserCoupon.Where(m => m.PromoteId == model.PromoteId && m.UserId == this.CurrentUserId).FirstOrDefault();

            string refereeId = GuidUtil.Empty();
            if (promoteUserCoupon == null)
            {
                isGoBuy = true;
            }
            else
            {
                refereeId = promoteUserCoupon.RefereeId;

                model.IsGetCoupon = promoteUserCoupon.IsGet;

                if (!promoteUserCoupon.IsBuy)
                {
                    isGoBuy = true;
                }
            }

            if (isGoBuy)
            {
                return Redirect("~/Promoteb/Coupon?promoteId=" + model.PromoteId + "&refereeId=" + refereeId);
            }

            return View(model);
        }
    }
}