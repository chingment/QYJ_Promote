using Lumos;
using Lumos.BLL.Service.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class PromotecController : OwnBaseController
    {
        public ActionResult Coupon()
        {
            return View();
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