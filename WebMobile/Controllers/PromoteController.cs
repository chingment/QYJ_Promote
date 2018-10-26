using Lumos;
using Lumos.BLL;
using Lumos.Entity;
using Lumos.WeiXinSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Models.Promote;

namespace WebMobile.Controllers
{

    public class PromoteController : OwnBaseController
    {
        public ActionResult Coupon(string promoteId, string refereeId)
        {


            //refereeId =00000000000000000000000000000000
            var model = new CouponViewModel();
            model.PromoteId = "a999753c5fe14e26bbecad576b6a6909";
            model.PromoteCouponId = "00000000000000000000000000000001";
            model.RefereeId = refereeId;

            var promoteUserCoupon = CurrentDb.PromoteUserCoupon.Where(m => m.ClientId == this.CurrentUserId && m.PromoteId == model.PromoteId && m.PromoteCouponId == model.PromoteCouponId).FirstOrDefault();
            if (promoteUserCoupon != null)
            {
                if (promoteUserCoupon.IsBuy)
                {
                    return Redirect("~/Promote/PayResult?promoteId=" + model.PromoteId + "&orderSn=" + promoteUserCoupon.OrderSn + "&isSuccessed=True");
                }
            }

            var promote = CurrentDb.Promote.Where(m => m.Id == model.PromoteId).FirstOrDefault();
            if (promote != null)
            {
                if (promote.EndTime < DateTime.Now)
                {
                    model.PromoteIsEnd = true;
                }
            }

            return View(model);
        }

        public ActionResult PayResult(string promoteId, string orderSn, bool isSuccessed = false)
        {
            var model = new PayResultViewModel();


            model.PromoteId = promoteId;
            model.OrderSn = orderSn;
            model.IsSuccessed = isSuccessed;


            var promote = CurrentDb.Promote.Where(m => m.Id == model.PromoteId).FirstOrDefault();
            if (promote != null)
            {
                if (promote.EndTime < DateTime.Now)
                {
                    model.PromoteIsEnd = true;
                }
            }

            bool isGoBuy = false;

            var promoteUserCoupon = CurrentDb.PromoteUserCoupon.Where(m => m.PromoteId == model.PromoteId && m.ClientId == this.CurrentUserId).FirstOrDefault();

            string refereeId = GuidUtil.Empty();
            if (promoteUserCoupon == null)
            {
                isGoBuy = true;
            }
            else
            {
                refereeId = promoteUserCoupon.RefereeId;

                model.IsGetCoupon = promoteUserCoupon.IsGet;

                if (!promoteUserCoupon.IsBuy)
                {
                    isGoBuy = true;
                }
            }

            if (isGoBuy)
            {
                return Redirect("~/Promote/Coupon?promoteId=" + model.PromoteId + "&refereeId=" + refereeId);
            }

            return View(model);
        }

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
                return SdkFactory.Wx.Instance().GetJsApiPayParams(result.Data.WxPrepayId, result.Data.Id, result.Data.Sn);
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
            var promoteUserCoupons = CurrentDb.PromoteUserCoupon.Where(m => m.PromoteId == promoteId && m.ClientId == this.CurrentUserId).ToList();

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
        public CustomJsonResult EditPromoteUserInfo(EditPromoteUserInfoViewModel model)
        {
            LogUtil.Info("用户:" + this.CurrentUserId + ",提交卡券信息");

            var promoteUser = CurrentDb.PromoteUser.Where(m => m.ClientId == this.CurrentUserId && m.PromoteId == model.PromoteId).FirstOrDefault();

            if (promoteUser == null)
            {
                promoteUser = new PromoteUser();
                promoteUser.Id = GuidUtil.New();
                promoteUser.PromoteId = model.PromoteId;
                promoteUser.ClientId = this.CurrentUserId;
                promoteUser.IsAgent = true;
                promoteUser.CtName = model.CtName;
                promoteUser.CtPhone = model.CtPhone;
                promoteUser.CtIsStudent = model.CtIsStudent;
                promoteUser.CtSchool = model.CtSchool;
                promoteUser.CreateTime = DateTime.Now;
                promoteUser.Creator = this.CurrentUserId;
                CurrentDb.PromoteUser.Add(promoteUser);
            }
            else
            {
                promoteUser.CtName = model.CtName;
                promoteUser.CtPhone = model.CtPhone;
                promoteUser.CtIsStudent = model.CtIsStudent;
                promoteUser.CtSchool = model.CtSchool;
            }

            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
        }

        [HttpPost]
        public CustomJsonResult AddCouponNotifyResult(AddCouponNotifyResultModel model)
        {

            var promoteUserCoupons = CurrentDb.PromoteUserCoupon.Where(m => m.PromoteId == model.PromoteId && m.ClientId == this.CurrentUserId).ToList();

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

        [HttpPost]
        public CustomJsonResult ShareLog(ShareLogModel model)
        {

            var promoteShareLog = new PromoteShareLog();
            promoteShareLog.Id = GuidUtil.New();
            promoteShareLog.ClientId = this.CurrentUserId;
            promoteShareLog.ShareLink = model.ShareLink;
            promoteShareLog.RefereeId = model.RefereeId;
            promoteShareLog.PromoteId = model.PromoteId;
            promoteShareLog.Type = model.Type;
            promoteShareLog.CreateTime = DateTime.Now;
            promoteShareLog.Creator = this.CurrentUserId;

            CurrentDb.PromoteShareLog.Add(promoteShareLog);
            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

        }

        [HttpPost]
        public CustomJsonResult AccessLog()
        {
            var uri = new Uri(Request.UrlReferrer.AbsoluteUri);
            string promoteId = HttpUtility.ParseQueryString(uri.Query).Get("promoteId");
            string refereeId = HttpUtility.ParseQueryString(uri.Query).Get("refereeId");
            var promoteAccessLog = new PromoteAccessLog();
            promoteAccessLog.Id = GuidUtil.New();
            promoteAccessLog.ClientId = this.CurrentUserId;
            promoteAccessLog.AccessUrl = Request.UrlReferrer.AbsoluteUri;
            promoteAccessLog.RefereeId = refereeId;
            promoteAccessLog.PromoteId = promoteId;
            promoteAccessLog.CreateTime = DateTime.Now;
            promoteAccessLog.Creator = this.CurrentUserId;

            CurrentDb.PromoteAccessLog.Add(promoteAccessLog);
            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

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