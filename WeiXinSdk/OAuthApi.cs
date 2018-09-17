using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lumos.WeiXinSdk
{
    public static class OAuthApi
    {
        public static string GetAuthorizeUrl(string appId, string redirectUrl)
        {
            redirectUrl = redirectUrl.Replace("&", "|");
            redirectUrl = HttpUtility.UrlEncode(redirectUrl);
            LogUtil.Info("redirectUrl:" + redirectUrl);
            var url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=" + redirectUrl + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect", appId);

            return url;
        }

        public static WxApiSnsOauth2AccessTokenResult GetWebOauth2AccessToken(string appId, string secret, string code)
        {
            WxApi api = new WxApi();
            WxApiSnsOauth2AccessToken oauth2AccessToken = new WxApiSnsOauth2AccessToken(appId, secret, code, "authorization_code");
            var oauth2_Result = api.DoGet(oauth2AccessToken);
            return oauth2_Result;
        }

        public static WxApiSnsUserInfoResult GetUserInfo(string accessToken, string openId)
        {
            WxApi api = new WxApi();
            WxApiSnsUserInfo snsUserInfo = new WxApiSnsUserInfo(accessToken, openId, "zh_CN");
            var snsUserInfo_Result = api.DoGet(snsUserInfo);
            return snsUserInfo_Result;
        }

        public static WxApiUserInfoResult GetUserInfoByApiToken(string accessToken, string openId)
        {
            WxApi api = new WxApi();
            WxApiUserInfo snsUserInfo = new WxApiUserInfo(accessToken, openId);
            var userInfo_Result = api.DoGet(snsUserInfo);
            return userInfo_Result;
        }


    }
}
