using Lumos;
using Lumos.BLL.Service.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class OrderController : OwnBaseController
    {
        public ActionResult Details()
        {
            return View();
        }

        public CustomJsonResult GetDetails(string id)
        {
            return AppServiceFactory.Order.GetDetails(this.CurrentUserId, this.CurrentUserId, id);
        }
    }
}