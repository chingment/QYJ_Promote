using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Models.Personal;

namespace WebMobile.Controllers
{
    public class PersonalController : OwnBaseController
    {
        public ActionResult Index()
        {
            ViewBag.ShowHeader = false;

            IndexViewModel model = new IndexViewModel();

            model.LoadData(this.CurrentUserId);

            return View(model);
        }
    }
}