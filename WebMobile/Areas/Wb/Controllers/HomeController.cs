using Lumos;
using Lumos.BLL;
using Lumos.BLL.Biz;
using Lumos.BLL.Service.Admin;
using Lumos.Common;
using Lumos.Entity;
using Lumos.Session;
using Lumos.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Areas.Wb.Own;

namespace WebMobile.Areas.Wb.Controllers
{
    public class HomeController : WebMobile.Areas.Wb.Own.OwnBaseController
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            Session["WebSSOLoginVerifyCode"] = null;
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }

        public ViewResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [CheckVerifyCode("WebSSOLoginVerifyCode")]
        public CustomJsonResult Login(RopLogin rop)
        {
            RetLogin ret = new RetLogin();

            var result = AdminServiceFactory.AuthorizeRelay.SignIn(rop.UserName, rop.Password, CommonUtil.GetIP(), Enumeration.LoginType.Website);

            if (result.ResultType == Enumeration.LoginResult.Failure)
            {

                if (result.ResultTip == Enumeration.LoginResultTip.UserNotExist || result.ResultTip == Enumeration.LoginResultTip.UserPasswordIncorrect)
                {
                    return Json(ResultType.Failure, ret, "用户名或密码不正确");
                }

                if (result.ResultTip == Enumeration.LoginResultTip.UserDisabled)
                {
                    return Json(ResultType.Failure, ret, "账户被禁用");
                }

                if (result.ResultTip == Enumeration.LoginResultTip.UserDeleted)
                {
                    return Json(ResultType.Failure, ret, "账户被删除");
                }
            }

            string host = "";
            string returnUrl = "";


            switch (result.User.Type)
            {
                case Enumeration.UserType.Staff:
                    host = System.Configuration.ConfigurationManager.AppSettings["custom:WebBackUrl"];
                    //returnUrl = string.Format("{0}?returnUrl={1}", host, model.ReturnUrl);
                    returnUrl = string.Format("{0}", "/Wb/Home/Index");
                    break;
                case Enumeration.UserType.Merchant:
                    host = System.Configuration.ConfigurationManager.AppSettings["custom:WebMerchUrl"];
                    //returnUrl = string.Format("{0}?returnUrl={1}", host, model.ReturnUrl);
                    returnUrl = string.Format("{0}", host);
                    break;
            }


            UserInfo userInfo = new UserInfo();
            userInfo.UserId = result.User.Id;
            userInfo.UserName = result.User.UserName;
            userInfo.Token = GuidUtil.New();

            SSOUtil.SetUserInfo(userInfo);

            ret.Url = string.Format("{0}?token={1}", returnUrl, userInfo.Token);

            return Json(ResultType.Success, ret, "登录成功");

        }


        [HttpPost]
        public ActionResult LogOff()
        {
            OwnRequest.Quit();

            return Redirect(WebMobile.Areas.Wb.Own.OwnWebSettingUtils.GetLoginPage(""));
        }

        [HttpPost]
        public CustomJsonResult ChangePassword(RopChangePassword rop)
        {

            var result = AdminServiceFactory.AuthorizeRelay.ChangePassword(this.CurrentUserId, this.CurrentUserId, rop.OldPassword, rop.NewPassword);

            if (result.Result == ResultType.Success)
            {
                return Json(ResultType.Success, "点击<a href=\"" + Wb.Own.OwnWebSettingUtils.GetLoginPage("") + "\">登录</a>");
            }

            return result;

        }
    }
}