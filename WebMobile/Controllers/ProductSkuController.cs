using Lumos;
using Lumos.BLL.Service.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class ProductSkuController : OwnBaseController
    {
        public ActionResult Details()
        {
            return View();
        }


        [HttpGet]
        public CustomJsonResult GetDetails()
        {
            var uri = new Uri(Request.UrlReferrer.AbsoluteUri);

            var rup = new RupProductSkuGetDetails();
            string promoteId = HttpUtility.ParseQueryString(uri.Query).Get("promoteId");
            string skuId = HttpUtility.ParseQueryString(uri.Query).Get("skuId");
            rup.PromoteId = promoteId;
            rup.SkuId = skuId;
            return AppServiceFactory.ProductSku.GetDetails(this.CurrentUserId, this.CurrentUserId, rup);
        }
    }
}