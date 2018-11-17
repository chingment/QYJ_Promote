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
        public readonly string sesionKeyLoginVerifyCode = "sesionKeyLoginVerifyCode";

        [AllowAnonymous]
        public ActionResult Login()
        {
            Session[sesionKeyLoginVerifyCode] = null;
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

        public CustomJsonResult GetIndexPageData()
        {
            var ret = new IndexModel();

            ret.Title = OwnWebSettingUtils.GetWebName();
            ret.IsLogin = OwnRequest.IsLogin();

            if (ret.IsLogin)
            {
                ret.UserName = OwnRequest.GetUserNameWithSymbol();

            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }


        public class IndexModel
        {
            public IndexModel()
            {
                this.MenuNavByLeft = new List<MenuModel>();
            }

            public string Title { get; set; }

            public bool IsLogin { get; set; }

            public string UserName { get; set; }

            public List<MenuModel> MenuNavByLeft { get; set; }

            public class MenuModel
            {
                public MenuModel()
                {
                    this.SubMenus = new List<SubMenuModel>();
                }

                public string Name { get; set; }

                public List<SubMenuModel> SubMenus { get; set; }
            }

            public class SubMenuModel
            {
                public string Url { get; set; }

                public string Name { get; set; }
            }
        }

        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]

        public CustomJsonResult Login(RopLogin rop)
        {
            RetLogin ret = new RetLogin();

            if (Session[sesionKeyLoginVerifyCode] == null)
            {
                return Json(ResultType.Failure, ret, "验证码超时");
            }

            if (Session[sesionKeyLoginVerifyCode].ToString() != rop.VerifyCode)
            {
                return Json(ResultType.Failure, ret, "验证码不正确");
            }

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
        public CustomJsonResult LogOff()
        {
            OwnRequest.Quit();
            var ret = new { url = Areas.Wb.Own.OwnWebSettingUtils.GetLoginPage("") };
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "退出成功", ret);
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