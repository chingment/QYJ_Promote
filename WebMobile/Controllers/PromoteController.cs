using Lumos;
using Lumos.BLL;
using Lumos.BLL.Service.App;
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
        [HttpPost]
        public CustomJsonResult UnifiedOrder(UnifiedOrderPms model)
        {
            LogUtil.Info("进入UnifiedOrder");
            LogUtil.Info("用户.CurrentUserId:" + this.CurrentUserId);

            model.ClientId = this.CurrentUserId;
            var result = BizFactory.Order.UnifiedOrder(this.CurrentUserId, model);

            if (result.Result == ResultType.Success)
            {
                LogUtil.Info("下单成功:" + Newtonsoft.Json.JsonConvert.SerializeObject(result));

                bool isBuy = false;

                if (result.Data.Status == Enumeration.OrderStatus.Payed || result.Data.Status == Enumeration.OrderStatus.Completed)
                {
                    isBuy = true;
                }

                return SdkFactory.Wx.Instance().GetJsApiPayParams(result.Data.WxPrepayId, result.Data.Id, result.Data.Sn, isBuy);
            }
            else
            {
                LogUtil.Info("下单失败");
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, result.Message);
            }
        }

        [HttpPost]
        public CustomJsonResult CancleOrder(string orderSn, string res)
        {
            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
        }

        [HttpPost]
        public CustomJsonResult PayNotifyResult(string orderSn, string res)
        {
            LogUtil.Info(string.Format("用户通知订单号({0})支付结果:{1}", orderSn, res));

            bool isPaySuccessed = false;
            var result = BizFactory.Order.PayResultNotify(this.CurrentUserId, Enumeration.OrderNotifyLogNotifyFrom.WebApp, res, orderSn, out isPaySuccessed);

            return result;
        }

        [HttpPost]
        public CustomJsonResult GetCardList(string promoteId)
        {
            LogUtil.Info("用户:" + this.CurrentUserId + ",领取卡券");

            List<WxCard> cardList = new List<WxCard>();

            string userId = this.CurrentUserId;
            var promoteUserCoupons = CurrentDb.ClientCoupon.Where(m => m.PromoteId == promoteId && m.ClientId == this.CurrentUserId).ToList();

            var promoteUserCouponsByBuyCount = promoteUserCoupons.Where(m => m.IsBuy == true).Count();

            if (promoteUserCouponsByBuyCount == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未购买卡券，不能操作！", cardList);
            }

            var promoteUser = CurrentDb.PromoteUser.Where(m => m.PromoteId == promoteId && m.ClientId == this.CurrentUserId).FirstOrDefault();

            if (promoteUser == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.FormImperfect, "请先完成信息", cardList);
            }

            if (string.IsNullOrEmpty(promoteUser.CtName))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.FormImperfect, "请先完成信息", cardList);
            }

            if (promoteUserCoupons.Count > 0)
            {
                string api_ticket = SdkFactory.Wx.Instance().GetCardApiTicket();

                LogUtil.Info("CardApiTicket:" + api_ticket);

                foreach (var item in promoteUserCoupons)
                {
                    string timestamp = CommonUtil.GetTimeStamp();
                    string nonce_str = CommonUtil.GetNonceStr();
                    string card_id = item.WxCouponId;
                    string code = "";
                    string openid = "";

                    Dictionary<string, string> sParams = new Dictionary<string, string>();

                    sParams.Add("nonce_str", nonce_str);
                    sParams.Add("timestamp", timestamp);
                    sParams.Add("card_id", card_id);
                    sParams.Add("code", "");
                    sParams.Add("openid", "");
                    //sParams.Add("outer_id", outer_id);
                    sParams.Add("api_ticket", api_ticket);


                    LogUtil.Info("api_ticket:" + api_ticket);
                    LogUtil.Info("timestamp:" + timestamp);
                    LogUtil.Info("nonce_str:" + nonce_str);
                    LogUtil.Info("card_id:" + card_id);

                    string signature = Lumos.WeiXinSdk.CommonUtil.MakeCardSign(sParams);

                    LogUtil.Info("signature:" + signature);

                    WxCardExt cardExt = new WxCardExt();

                    cardExt.code = code;
                    cardExt.openid = openid;
                    cardExt.timestamp = timestamp;
                    cardExt.signature = signature;
                    cardExt.nonce_str = nonce_str;
                    WxCard card = new WxCard();
                    card.cardId = card_id;
                    card.cardExt = Newtonsoft.Json.JsonConvert.SerializeObject(cardExt);
                    string decryptCode = item.WxCouponDecryptCode;
                    if (string.IsNullOrEmpty(item.WxCouponDecryptCode))
                    {
                        if (!string.IsNullOrEmpty(item.WxCouponEncryptCode))
                        {
                            decryptCode = SdkFactory.Wx.Instance().CardCodeDecrypt(item.WxCouponEncryptCode);
                        }
                    }

                    card.code = decryptCode;

                    cardList.Add(card);
                }

                LogUtil.Info("cardList:" + Newtonsoft.Json.JsonConvert.SerializeObject(cardList));

            }

            cardList = cardList.Where(m => m.cardId != null).ToList();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", cardList);
        }

        [HttpPost]
        public CustomJsonResult EditPromoteUserInfo(RopPromoteUserInfoEdit rop)
        {
            LogUtil.Info("用户:" + this.CurrentUserId + ",提交卡券信息");

            var promoteUser = CurrentDb.PromoteUser.Where(m => m.ClientId == this.CurrentUserId && m.PromoteId == rop.PromoteId).FirstOrDefault();

            if (promoteUser == null)
            {
                promoteUser = new PromoteUser();
                promoteUser.Id = GuidUtil.New();
                promoteUser.PromoteId = rop.PromoteId;
                promoteUser.ClientId = this.CurrentUserId;
                promoteUser.CtName = rop.CtName;
                promoteUser.CtPhone = rop.CtPhone;
                promoteUser.CtIsStudent = rop.CtIsStudent;
                promoteUser.CtSchool = rop.CtSchool;
                promoteUser.CreateTime = DateTime.Now;
                promoteUser.Creator = this.CurrentUserId;
                CurrentDb.PromoteUser.Add(promoteUser);
            }
            else
            {
                promoteUser.CtName = rop.CtName;
                promoteUser.CtPhone = rop.CtPhone;
                promoteUser.CtIsStudent = rop.CtIsStudent;
                promoteUser.CtSchool = rop.CtSchool;
            }

            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
        }

        [HttpPost]
        public CustomJsonResult AddCouponNotifyResult(AddCouponNotifyResultModel model)
        {

            var promoteUserCoupons = CurrentDb.ClientCoupon.Where(m => m.PromoteId == model.PromoteId && m.ClientId == this.CurrentUserId).ToList();

            foreach (var item in promoteUserCoupons)
            {
                var coupon = model.Coupons.Where(m => m.WxCouponId == item.WxCouponId).FirstOrDefault();
                if (coupon != null)
                {
                    item.WxCouponEncryptCode = coupon.WxCouponEncryptCode;

                    if (string.IsNullOrEmpty(item.WxCouponDecryptCode))
                    {
                        string decryptCode = SdkFactory.Wx.Instance().CardCodeDecrypt(coupon.WxCouponEncryptCode);
                        LogUtil.Info("解密CODE:" + decryptCode);
                        if (string.IsNullOrEmpty(decryptCode))
                        {
                            item.WxCouponDecryptCode = decryptCode;
                        }
                    }
                    item.Mender = this.CurrentUserId;
                    item.MendTime = DateTime.Now;
                }
            }

            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
        }


    }

    public class AddCouponNotifyResultModel
    {
        public string PromoteId { get; set; }

        public List<CouponResult> Coupons { get; set; }
    }

    public class CouponResult
    {
        public string WxCouponId { get; set; }

        public string WxCouponEncryptCode { get; set; }
    }

}