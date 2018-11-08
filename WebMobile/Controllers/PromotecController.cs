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

        public ActionResult CouponGet()
        {
            return View();
        }

        [HttpGet]
        public CustomJsonResult GetConfig(int screenHeight)
        {
            LogUtil.Info("rup.ScreenHeight" + screenHeight);
            var uri = new Uri(Request.UrlReferrer.AbsoluteUri);

            var rup = new RupPromoteGetConfig();
            string promoteId = HttpUtility.ParseQueryString(uri.Query).Get("promoteId");
            string refereeId = HttpUtility.ParseQueryString(uri.Query).Get("refereeId");

            rup.PromoteId = promoteId;
            rup.RefereeId = refereeId;
            rup.ScreenHeight = screenHeight;
            return AppServiceFactory.Promote.GetConfig(this.CurrentUserId, this.CurrentUserId, rup);
        }

    }
}