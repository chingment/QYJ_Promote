using System;
using System.Web;
using System.Web.Mvc;
using Lumos.BLL;
using Lumos.Entity;
using Lumos.Mvc;
using Lumos.WeiXinSdk;
using System.Text;
using System.Web.Security;
using WebMobile.Models.Home;
using Lumos.WeiXinSdk.MsgPush;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Lumos;
using Lumos.Session;

namespace WebMobile.Controllers
{
    public class HomeController : OwnBaseController
    {
        public ActionResult Index(int merchantId = 0)
        {
            return View();
        }

        //获取JsApiConfig配置参数
        public CustomJsonResult<JsApiConfigParams> GetJsApiConfigParams(string url)
        {

            return SdkFactory.Wx.Instance(0).GetJsApiConfigParams(url);
        }

        [AllowAnonymous]
        public RedirectResult Oauth2()
        {
            try
            {
                var request = Request;
                var code = request.QueryString["code"];
                int merchantId = 0;
                string returnUrl = request.QueryString["returnUrl"];
                LogUtil.Info("待跳转路径：{0}" + returnUrl);

                if (string.IsNullOrEmpty(code))
                {
                    var url = SdkFactory.Wx.Instance(merchantId).GetAuthorizeUrl(returnUrl);
                    return Redirect(url);
                }
                else
                {
                    var oauth2_Result = SdkFactory.Wx.Instance(merchantId).GetWebOauth2AccessToken(code);
                    if (oauth2_Result.errcode == null)
                    {
                        LogUtil.Info("用户OpenId:" + oauth2_Result.openid);
                        LogUtil.Info("用户AccessToken:" + oauth2_Result.access_token);

                        var snsUserInfo_Result = SdkFactory.Wx.Instance(merchantId).GetUserInfo(oauth2_Result.access_token, oauth2_Result.openid);
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
                        wxUserInfo.InfoFrom = Enumeration.WxUserInfoFrom.Authorize;

                        wxUserInfo = BizFactory.WxUser.CheckedUser(0, wxUserInfo, true);
                        if (wxUserInfo != null)
                        {
                            LogUtil.Info("用户Id：" + wxUserInfo.UserId);

                            UserInfo userInfo = new UserInfo();                      
                            userInfo.UserId = wxUserInfo.UserId;
                            userInfo.WxOpenId = oauth2_Result.openid;
                            userInfo.WxAccessToken = oauth2_Result.access_token;
                            SSOUtil.SetUserInfo(userInfo);

                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                return Redirect(returnUrl);
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
        public ActionResult PayResult()
        {

            Int32 intLen = Convert.ToInt32(Request.InputStream.Length);
            byte[] b = new byte[intLen];
            Request.InputStream.Read(b, 0, intLen);

            string xml = System.Text.Encoding.UTF8.GetString(b);

            int merchantId = 1;
            if (xml.IndexOf("[CDATA[1396636802]") > -1)
            {
                merchantId = 2;
            }

            LogUtil.Info("接受到结果:" + xml);
            //todo
            if (SdkFactory.Wx.Instance(merchantId).CheckPayNotifySign(xml))
            {
                var result = BizFactory.Pay.ResultNotify(0, Lumos.Entity.Enumeration.OrderNotifyLogNotifyFrom.NotifyUrl, xml);


                if (result.Result == ResultType.Success)
                {
                    Response.Write("<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>");
                    Response.End();
                }
            }
            else
            {
                LogUtil.Warn("支付通知结果签名验证失败");

                Response.End();
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult LogJsError(string errorMessage, string scriptURI, string columnNumber, string errorObj)
        {
            CustomJsonResult res = new CustomJsonResult();
            StringBuilder sb = new StringBuilder();
            sb.Append("前端JS脚本错误：" + errorMessage + "\t\n");
            sb.Append("错误信息：" + errorMessage + "\t\n");
            sb.Append("出错文件：" + scriptURI + "\t\n");
            sb.Append("出错列号：" + columnNumber + "\t\n");
            sb.Append("错误详情：" + errorObj + "\t\n");
            sb.Append("浏览器agent：" + Lumos.Common.CommonUtils.GetBrowserInfo() + "\t\n");
            LogUtil.Error(sb.ToString());
            return res;

        }

        [AllowAnonymous]
        public ActionResult NotifyEvent()
        {
            LogUtil.Info("开始接收事件推送通知");



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

        const string Token = "f06d280ddf1e49378ab338fc3124cf40";
        private bool CheckSignature()
        {
            string signature = Request.QueryString["signature"].ToString();
            string timestamp = Request.QueryString["timestamp"].ToString();
            string nonce = Request.QueryString["nonce"].ToString();
            string[] ArrTmp = { Token, timestamp, nonce };
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
    }
}