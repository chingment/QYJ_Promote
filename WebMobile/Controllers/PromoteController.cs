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

    public class PromoteController : OwnBaseController
    {
        public ActionResult Coupon(string id)
        {
            return View();
        }


        [HttpPost]
        public CustomJsonResult UnifiedOrder(UnifiedOrderPms model)
        {
            var result = BizFactory.Order.UnifiedOrder(this.CurrentUserId, this.CurrentUserId, model);

            if (result.Result == ResultType.Success)
            {
                return SdkFactory.Wx.Instance().GetJsApiPayParams(result.Data.WxPrepayId, result.Data.Id, result.Data.Sn);
            }
            else
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, result.Message);
            }
        }
    }
}