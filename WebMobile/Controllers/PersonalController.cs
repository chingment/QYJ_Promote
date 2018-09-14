using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class PersonalController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ShowHeader = false;

            return View();
        }
    }
}