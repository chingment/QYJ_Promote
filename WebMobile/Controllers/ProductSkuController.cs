﻿using Lumos;
using Lumos.BLL;
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


        [HttpPost]
        public CustomJsonResult UnifiedOrder(RopOrderUnifiedOrder rop)
        {
            LogUtil.Info("进入UnifiedOrder");
            LogUtil.Info("用户.CurrentUserId:" + this.CurrentUserId);


            var result = AppServiceFactory.Order.UnifiedOrder(this.CurrentUserId, this.CurrentUserId, rop);

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
    }
}