using log4net;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lumos.WeiXinSdk;
using System.Net;

namespace Lumos.BLL
{
    public sealed class WxUntil
    {
        private static WxUntil instance = null;
        private static readonly object padlock = new object();

        public static WxUntil GetInstance()
        {

            if (instance == null)
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new WxUntil();
                    }
                }
            }

            return instance;

        }



        public string GetAccessToken(string appId, string appSecret)
        {
            //用于测试配置
            string wxAccessToken = System.Configuration.ConfigurationManager.AppSettings["custom:WxTestAccessToken"];
            if (wxAccessToken != null)
            {
                return wxAccessToken;
            }

            string key = string.Format("Wx_AppId_{0}_AccessToken", appId);

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，重新获取", key));

                WxApi c = new WxApi();

                WxApiAccessToken apiAccessToken = new WxApiAccessToken("client_credential", appId, appSecret);

                var apiAccessTokenResult = c.DoGet(apiAccessToken);

                if (string.IsNullOrEmpty(apiAccessTokenResult.access_token))
                {
                    LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，Api重新获取失败", key));
                }
                else
                {
                    LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}，已过期，重新获取成功", key, apiAccessTokenResult.access_token));

                    accessToken = apiAccessTokenResult.access_token;

                    redis.KSet(key, accessToken, new TimeSpan(0, 30, 0));
                }

            }
            else
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}", key, accessToken));
            }

            return accessToken;
        }


        public string GetJsApiTicket(string appId, string access_token)
        {

            string key = string.Format("Wx_AppId_{0}_JsApiTicket", appId);

            var redis = new RedisClient<string>();
            var jsApiTicket = redis.KGetString(key);

            if (jsApiTicket == null)
            {
                WxApi c = new WxApi();

                var wxApiJsApiTicket = new WxApiJsApiTicket(access_token);

                var wxApiJsApiTicketResult = c.DoGet(wxApiJsApiTicket);
                if (string.IsNullOrEmpty(wxApiJsApiTicketResult.ticket))
                {
                    LogUtil.Info(string.Format("获取微信JsApiTicket，key：{0}，已过期，Api重新获取失败", key));
                }
                else
                {
                    LogUtil.Info(string.Format("获取微信JsApiTicket，key：{0}，value：{1}，已过期，重新获取成功", key, wxApiJsApiTicketResult.ticket));

                    jsApiTicket = wxApiJsApiTicketResult.ticket;

                    redis.KSet(key, jsApiTicket, new TimeSpan(0, 30, 0));
                }
            }
            else
            {
                LogUtil.Info(string.Format("获取微信JsApiTicket，key：{0}，value：{1}", key, jsApiTicket));
            }

            return jsApiTicket;

        }


        public string GetCardApiTicket(string appId,string access_token)
        {

            string key = string.Format("Wx_AppId_{0}_CardApiTicket", appId);

            var redis = new RedisClient<string>();
            var jsApiTicket = redis.KGetString(key);

            if (jsApiTicket == null)
            {
                WxApi c = new WxApi();

                var wxApiGetCardApiTicket = new WxApiGetCardApiTicket(access_token);

                var wxApiGetCardApiTicketResult = c.DoGet(wxApiGetCardApiTicket);
                if (string.IsNullOrEmpty(wxApiGetCardApiTicketResult.ticket))
                {
                    LogUtil.Info(string.Format("获取微信CardApiTicket，key：{0}，已过期，Api重新获取失败", key));
                }
                else
                {
                    LogUtil.Info(string.Format("获取微信CardApiTicket，key：{0}，value：{1}，已过期，重新获取成功", key, wxApiGetCardApiTicketResult.ticket));

                    jsApiTicket = wxApiGetCardApiTicketResult.ticket;

                    redis.KSet(key, jsApiTicket, new TimeSpan(0, 30, 0));
                }
            }
            else
            {
                LogUtil.Info(string.Format("获取微信CardApiTicket，key：{0}，value：{1}", key, jsApiTicket));
            }

            return jsApiTicket;

        }


        public  string UploadMultimediaImage(string access_token, string imageUrl)
        {
            string mediaId = "";
            string wxurl = "http://file.api.weixin.qq.com/cgi-bin/media/upload?access_token=" + access_token + "&type=image";
            WebClient myWebClient = new WebClient();
            myWebClient.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                byte[] responseArray = myWebClient.UploadFile(wxurl, "POST", imageUrl);
                string str_result = System.Text.Encoding.Default.GetString(responseArray, 0, responseArray.Length);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<UploadMultimediaResult>(str_result);

                if (result != null)
                {
                    mediaId = result.media_id;
                }
            }
            catch (Exception ex)
            {
                mediaId = "Error:" + ex.Message;
            }
            return mediaId;

        }


        public string CardCodeDecrypt(string access_token, string encrypt_code)
        {
            var api = new WxApi();
            var wxApiCardCodeDecrpt = new WxApiCardCodeDecrpt(access_token, WxPostDataType.Text, "{\"encrypt_code\":\"" + encrypt_code + "\"}");
            var wxApiCardCodeDecrpt_Result = api.DoPost(wxApiCardCodeDecrpt);
            return wxApiCardCodeDecrpt_Result.code;
        }

        public WxApiUserInfoResult GetUserInfoByApiToken(string accessToken, string openId)
        {
            WxApi api = new WxApi();
            WxApiUserInfo snsUserInfo = new WxApiUserInfo(accessToken, openId);
            var userInfo_Result = api.DoGet(snsUserInfo);
            return userInfo_Result;
        }

    }
}
