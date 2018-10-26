using Lumos;
using Lumos.BLL.Service.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Models.Promote;

namespace WebMobile.Controllers
{
    public class PromotecController : OwnBaseController
    {
        public ActionResult Coupon()
        {
            return View();
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

            var promoteUserCoupon = CurrentDb.PromoteUserCoupon.Where(m => m.PromoteId == model.PromoteId && m.ClientId == this.CurrentUserId).FirstOrDefault();

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
                return Redirect("~/Promotec/Coupon?promoteId=" + model.PromoteId + "&refereeId=" + refereeId);
            }

            model.RefereeId = refereeId;
            return View(model);
        }

        [HttpGet]
        public CustomJsonResult GetConfig(RupPromoteGetConfig rup)
        {
            if (Request.UrlReferrer == null)
            {

            }
            var uri = new Uri(Request.UrlReferrer.AbsoluteUri);
            string promoteId = HttpUtility.ParseQueryString(uri.Query).Get("promoteId");
            string refereeId = HttpUtility.ParseQueryString(uri.Query).Get("refereeId");

            rup.PromoteId= promoteId;
            rup.RefereeId = refereeId;
            return AppServiceFactory.Promote.GetConfig(this.CurrentUserId, this.CurrentUserId, rup);
        }

    }
}