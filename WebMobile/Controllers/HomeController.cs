﻿using System;
using System.Web;
using System.Web.Mvc;
using Lumos.BLL;
using Lumos.Entity;
using Lumos.WeiXinSdk;
using System.Text;
using System.Web.Security;
using Lumos.WeiXinSdk.MsgPush;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Lumos;
using Lumos.Session;
using ZXing;
using ZXing.QrCode;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using Lumos.Common;
using System.IO;
using Newtonsoft.Json.Linq;
using Lumos.BLL.Service.App;
using Lumos.BLL.Biz;
using Lumos.BLL.Sdk;

namespace WebMobile.Controllers
{
    public class HomeController : OwnBaseController
    {



        public ActionResult Index()
        {
         
            return View();
        }

        //获取JsApiConfig配置参数
        public CustomJsonResult<JsApiConfigParams> GetJsApiConfigParams(string url)
        {
            return SdkFactory.Wx.Instance().GetJsApiConfigParams(url);
        }

        [AllowAnonymous]
        public RedirectResult Oauth2()
        {
            try
            {
                var request = Request;
                var code = request.QueryString["code"];
                var returnUrl = request.QueryString["returnUrl"];

                LogUtil.Info("returnUrl=>" + (returnUrl == null ? "" : returnUrl.ToString()));

                if (string.IsNullOrEmpty(code))
                {
                    var url = SdkFactory.Wx.Instance().GetAuthorizeUrl(returnUrl);

                    LogUtil.Info("待跳转路径2：" + url);

                    return Redirect(url);
                }
                else
                {
                    var oauth2_Result = SdkFactory.Wx.Instance().GetWebOauth2AccessToken(code);
                    if (oauth2_Result.errcode == null)
                    {
                        LogUtil.Info("用户OpenId:" + oauth2_Result.openid);
                        LogUtil.Info("用户AccessToken:" + oauth2_Result.access_token);

                        var snsUserInfo_Result = SdkFactory.Wx.Instance().GetUserInfoByOAuth2Token(oauth2_Result.access_token, oauth2_Result.openid);
                        WxUserInfo wxUserInfo = new WxUserInfo();
                        wxUserInfo.AccessToken = oauth2_Result.access_token;
                        wxUserInfo.OpenId = oauth2_Result.openid;
                        wxUserInfo.ExpiresIn = DateTime.Now.AddSeconds(oauth2_Result.expires_in);
                        wxUserInfo.Nickname = snsUserInfo_Result.nickname;
                        wxUserInfo.Sex = snsUserInfo_Result.sex;
                        wxUserInfo.Province = snsUserInfo_Result.province;
                        wxUserInfo.City = snsUserInfo_Result.city;
                        wxUserInfo.Country = snsUserInfo_Result.country;
                        wxUserInfo.HeadImgUrl = snsUserInfo_Result.headimgurl;
                        wxUserInfo.UnionId = snsUserInfo_Result.unionid;


                        wxUserInfo = BizFactory.WxUser.CheckedUser(GuidUtil.New(), wxUserInfo);
                        if (wxUserInfo != null)
                        {
                            LogUtil.Info("用户Id：" + wxUserInfo.ClientId);

                            string key = GuidUtil.New();

                            UserInfo userInfo = new UserInfo();
                            userInfo.UserId = wxUserInfo.ClientId;
                            userInfo.WxOpenId = oauth2_Result.openid;
                            userInfo.WxAccessToken = oauth2_Result.access_token;
                            SSOUtil.SetUserInfo(key, userInfo);
                            Response.Cookies.Add(new HttpCookie(OwnRequest.SESSION_NAME, key));

                            LogUtil.Info("returnUrl.UrlDecode 前：" + returnUrl);
                            string s_returnUrl = HttpUtility.UrlDecode(returnUrl);
                            LogUtil.Info("returnUrl.UrlDecode 后：" + s_returnUrl);
                            s_returnUrl = s_returnUrl.Replace("|", "&");
                            LogUtil.Info("returnUrl.UrlDecode 替换|，&：" + s_returnUrl);

                            LogUtil.Info("returnUrl 最后返回：" + s_returnUrl);

                            if (!string.IsNullOrEmpty(s_returnUrl))
                            {
                                return Redirect(s_returnUrl);
                            }
                        }

                        LogUtil.Info("用户跳进主页");

                        return Redirect("/Home/Index");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("微信Oauth2授权验证发生异常", ex);
            }

            return Redirect("/Home/Oauth2");
        }

        [AllowAnonymous]
        public ContentResult PayResult()
        {
            Stream stream = Request.InputStream;
            stream.Seek(0, SeekOrigin.Begin);
            string xml = new StreamReader(stream).ReadToEnd();

            if (string.IsNullOrEmpty(xml))
            {
                return Content("");
            }

            LogUtil.Info("接收支付结果:" + xml);

            if (!SdkFactory.Wx.Instance().CheckPayNotifySign(xml))
            {
                LogUtil.Warn("支付通知结果签名验证失败");
                return Content("");
            }

            string orderSn = "";
            var dic2 = Lumos.WeiXinSdk.CommonUtil.ToDictionary(xml);
            if (dic2.ContainsKey("out_trade_no") && dic2.ContainsKey("result_code"))
            {
                orderSn = dic2["out_trade_no"].ToString();
            }

            bool isPaySuccessed = false;
            var result = BizFactory.Order.PayResultNotify(GuidUtil.Empty(), Enumeration.OrderNotifyLogNotifyFrom.NotifyUrl, xml, orderSn, out isPaySuccessed);

            if (result.Result == ResultType.Success)
            {
                Response.Write("<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>");
                Response.End();
            }

            return Content("");
        }

        [AllowAnonymous]
        public ActionResult LogJsError(string errorMessage, string scriptURI, string columnNumber, string errorObj)
        {
            CustomJsonResult res = new CustomJsonResult();
            StringBuilder sb = new StringBuilder();
            sb.Append("前端JS脚本错误：" + (errorMessage == null ? "" : errorMessage) + "\t\n");
            sb.Append("错误信息：" + (errorMessage == null ? "" : errorMessage) + "\t\n");
            sb.Append("出错文件：" + (scriptURI == null ? "" : scriptURI) + "\t\n");
            sb.Append("出错列号：" + (columnNumber == null ? "" : columnNumber) + "\t\n");
            sb.Append("错误详情：" + (errorObj == null ? "" : errorObj) + "\t\n");
            sb.Append("浏览器agent：" + Lumos.Common.CommonUtil.GetBrowserInfo() + "\t\n");
            LogUtil.Error(sb.ToString());
            return res;

        }

        [AllowAnonymous]
        public ActionResult NotifyEvent()
        {
            LogUtil.Info("开始接收事件推送通知");
            string echoStr = "";
            if (Request.HttpMethod == "POST")
            {
                Stream stream = Request.InputStream;
                stream.Seek(0, SeekOrigin.Begin);
                string xml = new StreamReader(stream).ReadToEnd();

                //Int32 intLen = Convert.ToInt32(Request.InputStream.Length);
                //byte[] b = new byte[intLen];
                //Request.InputStream.Read(b, 0, intLen);
                //string xml = System.Text.Encoding.UTF8.GetString(b);
                LogUtil.Info("接收事件推送内容:" + xml);
                if (!string.IsNullOrEmpty(xml))
                {
                    var baseEventMsg = WxMsgFactory.CreateMessage(xml);
                    string eventKey = null;
                    LogUtil.Info("baseEventMsg内容:" + baseEventMsg);
                    if (baseEventMsg != null)
                    {
                        var userInfo_Result = SdkFactory.Wx.Instance().GetUserInfoByApiToken(baseEventMsg.FromUserName);

                        if (userInfo_Result.openid != null)
                        {
                            LogUtil.Info("userInfo_Result:" + JsonConvert.SerializeObject(userInfo_Result));

                            var wxUserInfo = new WxUserInfo();

                            wxUserInfo.OpenId = userInfo_Result.openid;
                            wxUserInfo.Nickname = userInfo_Result.nickname;
                            wxUserInfo.Sex = userInfo_Result.sex.ToString();
                            wxUserInfo.Province = userInfo_Result.province;
                            wxUserInfo.City = userInfo_Result.city;
                            wxUserInfo.Country = userInfo_Result.country;
                            wxUserInfo.HeadImgUrl = userInfo_Result.headimgurl;
                            wxUserInfo.UnionId = userInfo_Result.unionid;

                            wxUserInfo = BizFactory.WxUser.CheckedUser(GuidUtil.New(), wxUserInfo);

                            if (wxUserInfo != null)
                            {
                                var wxAutoReply = new WxAutoReply();
                                switch (baseEventMsg.MsgType)
                                {
                                    case MsgType.TEXT:
                                        #region TEXT

                                        LogUtil.Info("文本消息");

                                        var textMsg = (TextMsg)baseEventMsg;

                                        if (textMsg != null)
                                        {

                                            LogUtil.Info("文本消息:" + textMsg.Content);

                                            if (textMsg.Content == "双11入场券" || textMsg.Content == "双十一入场券" || textMsg.Content == "双11入场卷" || textMsg.Content == "双十一入场卷")
                                            {
                                                string promoteId = "80c71a0657924059b39895f9e406ef84";
                                                string media_Id = GetWxPromoteImgMediaId(promoteId, wxUserInfo.ClientId);
                                                echoStr = WxMsgFactory.CreateReplyImage(baseEventMsg.FromUserName, baseEventMsg.ToUserName, media_Id);
                                            }

                                        }


                                        #endregion
                                        break;
                                    case MsgType.EVENT:
                                        #region EVENT
                                        switch (baseEventMsg.Event)
                                        {
                                            case EventType.SUBSCRIBE://订阅
                                                break;
                                            case EventType.UNSUBSCRIBE://取消订阅
                                                break;
                                            case EventType.SCAN://扫描二维码
                                            case EventType.CLICK://单击按钮
                                            case EventType.VIEW://链接按钮
                                                break;
                                            case EventType.USER_GET_CARD://领取卡卷
                                                #region  USER_GET_CARD
                                                var userGetCardMsg = (UserGetCardMsg)baseEventMsg;

                                                var promoteUserCoupon = CurrentDb.ClientCoupon.Where(m => m.ClientId == wxUserInfo.ClientId && m.WxCouponId == userGetCardMsg.CardId).FirstOrDefault();

                                                if (promoteUserCoupon != null)
                                                {
                                                    promoteUserCoupon.IsGet = true;
                                                    promoteUserCoupon.GetTime = DateTime.Now;
                                                    promoteUserCoupon.Mender = GuidUtil.Empty();
                                                    promoteUserCoupon.MendTime = DateTime.Now;
                                                    promoteUserCoupon.WxCouponDecryptCode = userGetCardMsg.UserCardCode;
                                                    CurrentDb.SaveChanges();
                                                }

                                                #endregion
                                                break;
                                            case EventType.USER_CONSUME_CARD://核销卡卷
                                                #region USER_CONSUME_CARD

                                                //var userConsumeCardMsg = (UserConsumeCardMsg)baseEventMsg;

                                                //if (userConsumeCardMsg != null)
                                                //{
                                                //    var reidsMqByCalProfitModel = new RedisMq4GlobalHandle();
                                                //    var reidsMqByCalProfitByCouponConsumeModel = new ReidsMqByCalProfitByCouponConsumeModel();
                                                //    reidsMqByCalProfitByCouponConsumeModel.ClientId = wxUserInfo.ClientId;
                                                //    reidsMqByCalProfitByCouponConsumeModel.WxCouponDecryptCode = userConsumeCardMsg.UserCardCode;
                                                //    reidsMqByCalProfitByCouponConsumeModel.WxCouponId = userConsumeCardMsg.CardId;

                                                //    reidsMqByCalProfitModel.Pms = reidsMqByCalProfitByCouponConsumeModel;

                                                //    ReidsMqFactory.Global.Push(RedisMqHandleType.CouponConsume,reidsMqByCalProfitModel);
                                                //}


                                                #endregion
                                                break;
                                        }
                                        #endregion
                                        break;
                                }

                                var wxMsgPushLog = new WxMsgPushLog();
                                wxMsgPushLog.Id = GuidUtil.New();
                                wxMsgPushLog.ClientId = wxUserInfo.ClientId;
                                wxMsgPushLog.ToUserName = baseEventMsg.ToUserName;
                                wxMsgPushLog.FromUserName = baseEventMsg.FromUserName;
                                wxMsgPushLog.CreateTime = DateTime.Now;
                                wxMsgPushLog.ContentXml = xml;
                                wxMsgPushLog.MsgId = baseEventMsg.MsgId;
                                wxMsgPushLog.MsgType = baseEventMsg.MsgType.ToString();
                                wxMsgPushLog.Event = baseEventMsg.Event.ToString();
                                wxMsgPushLog.EventKey = eventKey;

                                WxMsgPushLog(wxMsgPushLog);
                            }
                        }
                    }
                }
            }
            else if (Request.HttpMethod == "GET") //微信服务器在首次验证时，需要进行一些验证，但。。。。  
            {
                if (string.IsNullOrEmpty(Request["echostr"]))
                {
                    echoStr = "无法获取微信接入信息，仅供测试！";

                }
                else
                {
                    echoStr = Request["echostr"].ToString();
                }

            }
            else
            {
                echoStr = "wrong";
            }

            LogUtil.Info(string.Format("接收事件推送之后回复内容:{0}", echoStr));
            Response.Write(echoStr);
            Response.End();
            return View();
        }

        public Task<bool> WxMsgPushLog(WxMsgPushLog wxMsgPushLog)
        {
            return Task.Run(() =>
            {

                CurrentDb.WxMsgPushLog.Add(wxMsgPushLog);
                CurrentDb.SaveChanges();


                return true;

            });
        }

        private bool CheckSignature()
        {
            string signature = Request.QueryString["signature"].ToString();
            string timestamp = Request.QueryString["timestamp"].ToString();
            string nonce = Request.QueryString["nonce"].ToString();
            string[] ArrTmp = { SdkFactory.Wx.Instance().GetNotifyEventUrlToken(), timestamp, nonce };
            Array.Sort(ArrTmp);
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();
            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetWxPromoteImgMediaId(string promoteId, string clientId)
        {
            try
            {
                var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientId == clientId).FirstOrDefault();
                if (wxUserInfo == null)
                    return null;
                var promote = CurrentDb.Promote.Where(m => m.Id == promoteId).FirstOrDefault();

                if (promote == null)
                    return null;

                var promoteUser = CurrentDb.PromoteUser.Where(m => m.PromoteId == promoteId && m.ClientId == clientId).FirstOrDefault();
                if (promoteUser == null)
                {
                    promoteUser = new PromoteUser();
                    promoteUser.Id = GuidUtil.New();
                    promoteUser.PromoteId = promoteId;
                    promoteUser.ClientId = clientId;
                    promoteUser.RefereerId = null;
                    promoteUser.CreateTime = DateTime.Now;
                    promoteUser.Creator = clientId;
                    CurrentDb.PromoteUser.Add(promoteUser);
                    CurrentDb.SaveChanges();
                }

                if (string.IsNullOrEmpty(promoteUser.WxPromoteImgMediaId))
                {
                    System.Drawing.Image oImg = System.Drawing.Image.FromFile(Server.MapPath("~/Content/images/promote20181029/referee_bg.png"));
                    BarcodeWriter writer = new BarcodeWriter();
                    writer.Format = BarcodeFormat.QR_CODE;
                    QrCodeEncodingOptions options = new QrCodeEncodingOptions();
                    options.DisableECI = true;
                    //设置内容编码
                    options.CharacterSet = "UTF-8";
                    //设置二维码的宽度和高度
                    options.Width = 500;
                    options.Height = 500;
                    //设置二维码的边距,单位不是固定像素
                    options.Margin = 1;
                    writer.Options = options;
                    System.Drawing.Image oImg1 = writer.Write(string.Format("http://qyj.17fanju.com/Promotec/Coupon?promoteId={0}&refereerId={1}", promoteId, clientId));
                    System.Drawing.Bitmap map = new Bitmap(oImg);
                    oImg.Dispose();
                    Graphics g = Graphics.FromImage(map);
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    SolidBrush brush = new SolidBrush(Color.Green);
                    PointF P = new PointF(100, 100);
                    Font f = new Font("Arial", 20);
                    g.DrawImage(oImg1, 320, 1655, 150, 150);//画二维码图片      


                    if (wxUserInfo != null)
                    {
                        if (!string.IsNullOrEmpty(wxUserInfo.HeadImgUrl))
                        {
                            var oImg4 = CirclePhoto(wxUserInfo.HeadImgUrl, 100);

                            g.DrawImage(oImg4, 620, 1655, 150, 150);
                        }
                    }

                    string key = GuidUtil.New();
                    string path = Server.MapPath("~/Static/Promote/User/") + key + ".jpg";

                    map.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                    f.Dispose();
                    g.Dispose();

                    string media_Id = SdkFactory.Wx.Instance().UploadMultimediaImage(path);
                    promoteUser.PromoteImgUrl = string.Format("http://qyj.17fanju.com/Static/Promote/User/{0}.jpg", key);
                    promoteUser.WxPromoteImgMediaId = media_Id;
                    CurrentDb.SaveChanges();

                    return media_Id;
                }
                else
                {
                    return promoteUser.WxPromoteImgMediaId;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("", ex);
                return null;
            }
        }


        public static Bitmap CirclePhoto(string urlPath, int size)
        {

            try
            {
                System.Net.WebRequest webreq = System.Net.WebRequest.Create(urlPath);
                System.Net.WebResponse webres = webreq.GetResponse();
                System.IO.Stream stream = webres.GetResponseStream();
                Image img1 = System.Drawing.Image.FromStream(stream);
                stream.Dispose();

                Bitmap b = new Bitmap(size, size);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.DrawImage(img1, 0, 0, b.Width, b.Height);
                    int r = Math.Min(b.Width, b.Height) / 2;
                    PointF c = new PointF(b.Width / 2.0F, b.Height / 2.0F);
                    for (int h = 0; h < b.Height; h++)
                        for (int w = 0; w < b.Width; w++)
                            if ((int)Math.Pow(r, 2) < ((int)Math.Pow(w * 1.0 - c.X, 2) + (int)Math.Pow(h * 1.0 - c.Y, 2)))
                            {
                                b.SetPixel(w, h, Color.Transparent);
                            }
                    //画背景色圆
                    using (Pen p = new Pen(System.Drawing.SystemColors.Control))
                        g.DrawEllipse(p, 0, 0, b.Width, b.Height);
                }
                return b;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        [HttpPost]
        [AllowAnonymous]
        public CustomJsonResult ShareLog(RopShareLog rop)
        {

            var clientShareLog = new ClientShareLog();
            clientShareLog.Id = GuidUtil.New();
            clientShareLog.ClientId = this.CurrentUserId;
            clientShareLog.ShareLink = rop.ShareLink;
            clientShareLog.RefereerId = rop.RefereerId;
            clientShareLog.PromoteId = rop.PromoteId;
            clientShareLog.Type = rop.Type;
            clientShareLog.CreateTime = DateTime.Now;
            clientShareLog.Creator = this.CurrentUserId;

            CurrentDb.ClientShareLog.Add(clientShareLog);
            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

        }

        [HttpPost]
        [AllowAnonymous]
        public CustomJsonResult AccessLog()
        {
            var uri = new Uri(Request.UrlReferrer.AbsoluteUri);
            string promoteId = HttpUtility.ParseQueryString(uri.Query).Get("promoteId");
            string refereerId = HttpUtility.ParseQueryString(uri.Query).Get("refereerId");
            var clientAccessLog = new ClientAccessLog();
            clientAccessLog.Id = GuidUtil.New();
            clientAccessLog.ClientId = this.CurrentUserId;
            clientAccessLog.AccessUrl = Request.UrlReferrer.AbsoluteUri;
            clientAccessLog.RefereerId = refereerId;
            clientAccessLog.PromoteId = promoteId;
            clientAccessLog.CreateTime = DateTime.Now;
            clientAccessLog.Creator = this.CurrentUserId;

            CurrentDb.ClientAccessLog.Add(clientAccessLog);
            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

        }

    }
}