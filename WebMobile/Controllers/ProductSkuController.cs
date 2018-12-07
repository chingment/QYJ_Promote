using Lumos;
using Lumos.BLL;
using Lumos.BLL.Biz;
using Lumos.BLL.Service.App;
using Lumos.Entity;
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
        public CustomJsonResult GetDetails(RupProductSkuGetDetails rup)
        {
            return AppServiceFactory.ProductSku.GetDetails(this.CurrentUserId, this.CurrentUserId, rup);
        }
    }
}