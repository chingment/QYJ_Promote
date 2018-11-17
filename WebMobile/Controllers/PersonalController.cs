using Lumos;
using Lumos.BLL.Service.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class PersonalController : OwnBaseController
    {
        public ActionResult Index()
        {
            ViewBag.ShowHeader = false;
            return View();
        }

        public CustomJsonResult GetIndexPageData()
        {
            return AppServiceFactory.Personal.GetIndexPageData(this.CurrentUserId, this.CurrentUserId);
        }

    }
}