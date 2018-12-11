using Lumos;
using Lumos.BLL;
using Lumos.BLL.Biz;
using Lumos.BLL.Sdk;
using Lumos.BLL.Service.App;
using Lumos.Entity;
using Lumos.WeiXinSdk;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class PromotecController : OwnBaseController
    {
        public ActionResult Coupon()
        {
            return View();
        }

        public ActionResult CouponGet()
        {
            return View();
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
        public CustomJsonResult AddCouponNotifyResult(RopCouponNotifyResult rop)
        {

            var promoteUserCoupons = CurrentDb.ClientCoupon.Where(m => m.PromoteId == rop.PromoteId && m.ClientId == this.CurrentUserId).ToList();

            foreach (var item in promoteUserCoupons)
            {
                var coupon = rop.Coupons.Where(m => m.WxCouponId == item.WxCouponId).FirstOrDefault();
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

        [HttpGet]
        public CustomJsonResult GetConfig(int screenHeight)
        {
            LogUtil.Info("rup.ScreenHeight" + screenHeight);
            var uri = new Uri(Request.UrlReferrer.AbsoluteUri);

            var rup = new RupPromoteGetConfig();
            string promoteId = HttpUtility.ParseQueryString(uri.Query).Get("promoteId");
            string refereeId = HttpUtility.ParseQueryString(uri.Query).Get("refereeId");

            rup.PromoteId = promoteId;
            rup.RefereeId = refereeId;
            rup.ScreenHeight = screenHeight;
            return AppServiceFactory.Promote.GetConfig(this.CurrentUserId, this.CurrentUserId, rup);
        }

        [HttpGet]
        [AllowAnonymous]

        public CustomJsonResult UpdateBlackList()
        {
            FileStream fsRead = null;
            try
            {
                string promoteId = "c0c71a0657924059b39895f9e406ef26";

                fsRead = new FileStream(Server.MapPath("~/Files/a1.xls"), FileMode.Open);

                HSSFWorkbook workbook = new HSSFWorkbook(fsRead);

                if (workbook == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel读取失败");
                }

                ISheet sheet = workbook.GetSheetAt(0);

                if (sheet == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel工作本为空");
                }

                if (sheet.LastRowNum <= 1)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel工作本数据为空");
                }

                int rowCount = sheet.LastRowNum + 1;


                for (int i = 1; i < rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row != null)
                    {
                        var cell_phoneNumer = row.GetCell(2);
                        if (cell_phoneNumer != null)
                        {
                            string str_phoneNumer = cell_phoneNumer.ToString().Trim();
                            var promoteUser = CurrentDb.PromoteUser.Where(m => m.CtPhone == str_phoneNumer).FirstOrDefault();
                            if (promoteUser != null)
                            {
                                var promoteBlackList = CurrentDb.PromoteBlackList.Where(m => m.ClientId == promoteUser.ClientId && m.PromoteId == promoteId).FirstOrDefault();
                                if (promoteBlackList == null)
                                {
                                    promoteBlackList = new Lumos.Entity.PromoteBlackList();
                                    promoteBlackList.Id = GuidUtil.New();
                                    promoteBlackList.PromoteId = promoteId;
                                    promoteBlackList.ClientId = promoteUser.ClientId;
                                    promoteBlackList.CreateTime = DateTime.Now;
                                    promoteBlackList.Creator = GuidUtil.Empty();
                                    CurrentDb.PromoteBlackList.Add(promoteBlackList);
                                    CurrentDb.SaveChanges();
                                }
                            }
                        }
                    }
                }

                if (fsRead != null)
                {
                    fsRead.Close();
                    fsRead.Dispose();
                }

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "上传成功");
            }
            catch (Exception ex)
            {
                if (fsRead != null)
                {
                    fsRead.Close();
                    fsRead.Dispose();
                }
                LogUtil.Error("读取excel失败", ex);
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel上传失败");
            }

        }
    }
}