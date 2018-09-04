using Lumos;
using Lumos.BLL;
using Lumos.Entity;
using Lumos.WeiXinSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{

    public class PromoteController : OwnBaseController
    {
        public ActionResult Coupon(string promoteId, string refereeId)
        {
            return View();
        }

        public ActionResult PayResult(string orderSn, bool isSuccessed = false)
        {
            return View();
        }

        [HttpPost]
        public CustomJsonResult UnifiedOrder(UnifiedOrderPms model)
        {
            LogUtil.Info("进入UnifiedOrder");
            LogUtil.Info("用户.CurrentUserId:" + this.CurrentUserId);

            var result = BizFactory.Order.UnifiedOrder(this.CurrentUserId, this.CurrentUserId, model);

            if (result.Result == ResultType.Success)
            {
                LogUtil.Info("下单成功:" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                return SdkFactory.Wx.Instance().GetJsApiPayParams(result.Data.WxPrepayId, result.Data.Id, result.Data.Sn);
            }
            else
            {
                LogUtil.Info("下单失败");
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, result.Message);
            }
        }

        [HttpPost]
        public CustomJsonResult PayNotifyResult(string orderSn, string res)
        {
            LogUtil.Info(string.Format("用户通知订单号({0})支付结果:{1}", orderSn, res));

            var result = BizFactory.Order.PayResultNotify(this.CurrentUserId, Enumeration.OrderNotifyLogNotifyFrom.WebApp, res, orderSn);

            return result;
        }

        [HttpPost]
        public CustomJsonResult GetCardList(string promoteId)
        {
            List<WxCard> cardList = new List<WxCard>();

            string api_ticket = SdkFactory.Wx.Instance().GetCardApiTicket();

            LogUtil.Info("CardApiTicket:" + api_ticket);

            string timestamp = CommonUtil.GetTimeStamp();
            string nonce_str = CommonUtil.GetNonceStr();
            string card_id = "ptakHv_v1qDj94DkZ21AMuVOt304";
            string code = "";
            string openid = "";

            Dictionary<string, string> sParams = new Dictionary<string, string>();

            sParams.Add("nonce_str", nonce_str);
            sParams.Add("timestamp", timestamp);
            sParams.Add("card_id", card_id);
            sParams.Add("code", "");
            sParams.Add("openid", "");
            sParams.Add("api_ticket", api_ticket);

            string signature = Lumos.WeiXinSdk.CommonUtil.MakeCardSign(sParams);

            WxCardExt cardExt = new WxCardExt();

            cardExt.code = code;
            cardExt.openid = openid;
            cardExt.timestamp = timestamp;
            cardExt.signature = signature;
            cardExt.nonce_str = nonce_str;

            WxCard card = new WxCard();
            card.cardId = card_id;
            card.cardExt = Newtonsoft.Json.JsonConvert.SerializeObject(cardExt);

            cardList.Add(card);

            LogUtil.Info("cardList:" + Newtonsoft.Json.JsonConvert.SerializeObject(cardList));

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", cardList);
        }
    }
}