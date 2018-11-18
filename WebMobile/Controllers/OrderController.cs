using Lumos;
using Lumos.BLL;
using Lumos.BLL.Biz;
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

        [HttpPost]
        public CustomJsonResult UnifiedOrder(RopOrderUnifiedOrder rop)
        {
            LogUtil.Info("进入UnifiedOrder");
            LogUtil.Info("用户.CurrentUserId:" + this.CurrentUserId);


            var result = BizFactory.Order.UnifiedOrder(this.CurrentUserId, this.CurrentUserId, rop);

            if (result.Result == ResultType.Success)
            {
                LogUtil.Info("下单成功:" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                return SdkFactory.Wx.Instance().GetJsApiPayParams(result.Data.WxPrepayId, result.Data.OrderId, result.Data.OrderSn, result.Data.IsBuy);
            }
            else
            {
                LogUtil.Info("下单失败");
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, result.Message);
            }
        }

        [HttpPost]
        public CustomJsonResult Cancle(RopOrderCancleOrder rop)
        {
            var result = BizFactory.Order.Cancle(this.CurrentUserId, rop.OrderId, "用户取消支付");
            return result;
        }
    }
}