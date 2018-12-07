using Lumos;
using Lumos.BLL;
using Lumos.BLL.Service.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class OperateController : OwnBaseController
    {
        public ActionResult Result()
        {
            return View();
        }

        public CustomJsonResult GetResult(RupOperateGetResult rup)
        {
            return AppServiceFactory.Operate.GetResult(this.CurrentUserId, this.CurrentUserId, rup);
        }
    }
}