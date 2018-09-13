using Lumos;
using Lumos.BLL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class WithdrawController : OwnBaseController
    {
        // GET: Withdraw
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public CustomJsonResult Apply(WithdrawApplyPms model)
        {
            model.UserId = this.CurrentUserId;
            return BizFactory.Withdraw.Apply(this.CurrentUserId, model);
        }
    }
}