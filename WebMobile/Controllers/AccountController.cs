using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lumos.BLL;
using Lumos.Entity;
using Lumos.Common;
using Lumos.DAL.AuthorizeRelay;
using Lumos.Redis;
using System.ComponentModel.DataAnnotations;
using Lumos;
using Lumos.Session;
using Lumos.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using Lumos.BLL.Service.Admin;
using Lumos.BLL.Biz;

namespace WebMobile.Controllers
{
    public class AccountController : OwnBaseController
    {

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            //var s = CurrentDb.PromoteCoupon.Where(m => m.Id == "00000000000000000000000000000004").FirstOrDefault();

            //string b = JsonConvert.SerializeObject(s);
            //OrderConfirm();
            //string a = "/Promote/Coupon?promoteId=a999753c5fe14e26bbecad576b6a6909&amp;refereeId=00000000000000000000000000000000";

            //string c = HttpUtility.HtmlDecode(a);

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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

            UserInfo userInfo = new UserInfo();
            userInfo.Token = GuidUtil.New();
            userInfo.UserId = result.User.Id;
            userInfo.UserName = result.User.UserName;

            SSOUtil.SetUserInfo(userInfo);


            Response.Cookies.Add(new HttpCookie(OwnRequest.SESSION_NAME, userInfo.Token));


            ret.Url = rop.ReturnUrl;

            return Json(ResultType.Success, ret, "登录成功");

        }

    }
}