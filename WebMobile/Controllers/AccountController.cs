using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lumos.BLL;
using Lumos.Entity;
using WebMobile.Models;
using Lumos.Common;
using Lumos.DAL.AuthorizeRelay;
using Lumos.Redis;
using System.ComponentModel.DataAnnotations;
using Lumos;
using WebMobile.Models.Account;
using Lumos.Session;
using Lumos.Web;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebMobile.Controllers
{
    public class AccountController : OwnBaseController
    {
        public string OrderConfirm()
        {
            Models.Home.SearchCondition model = new Models.Home.SearchCondition();

            model.Name = "dasda";

            string a1 = JsonConvert.SerializeObject(model);



            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("version", "1.3.0.7");
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("http://localhost:18068/Home/NotifyEvent", a1, headers);

            return result;

        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            OrderConfirm();
            string a = "/Promote/Coupon?promoteId=a999753c5fe14e26bbecad576b6a6909&amp;refereeId=00000000000000000000000000000000";

            string c = HttpUtility.HtmlDecode(a);

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public CustomJsonResult Login(LoginViewModel model)
        {


            GoToViewModel gotoViewModel = new GoToViewModel();

            var result = SysFactory.AuthorizeRelay.SignIn(model.UserName, model.Password, CommonUtils.GetIP(), Enumeration.LoginType.Website);

            if (result.ResultType == Enumeration.LoginResult.Failure)
            {

                if (result.ResultTip == Enumeration.LoginResultTip.UserNotExist || result.ResultTip == Enumeration.LoginResultTip.UserPasswordIncorrect)
                {
                    return Json(ResultType.Failure, gotoViewModel, "用户名或密码不正确");
                }

                if (result.ResultTip == Enumeration.LoginResultTip.UserDisabled)
                {
                    return Json(ResultType.Failure, gotoViewModel, "账户被禁用");
                }

                if (result.ResultTip == Enumeration.LoginResultTip.UserDeleted)
                {
                    return Json(ResultType.Failure, gotoViewModel, "账户被删除");
                }
            }

            UserInfo userInfo = new UserInfo();
            userInfo.Token = GuidUtil.New();
            userInfo.UserId = result.User.Id;
            userInfo.UserName = result.User.UserName;

            SSOUtil.SetUserInfo(userInfo);


            Response.Cookies.Add(new HttpCookie(OwnRequest.SESSION_NAME, userInfo.Token));


            gotoViewModel.Url = model.ReturnUrl;

            return Json(ResultType.Success, gotoViewModel, "登录成功");

        }

    }
}