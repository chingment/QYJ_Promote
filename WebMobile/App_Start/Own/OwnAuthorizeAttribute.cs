using Lumos.Mvc;
using System;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.Reflection;
using Lumos;

namespace WebMobile
{
    #region 授权过滤器
    // 摘要:
    //     继承Authorize属性
    //     扩展Permission权限代码,用来控制用户是否拥有该类或方法的权限
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class OwnAuthorizeAttribute : AuthorizeAttribute
    {
        ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            if (skipAuthorization)
            {
                return;
            }

            #endregion
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            log.InfoFormat("当前未登录的URL:{0}", filterContext.HttpContext.Request.RawUrl);

         
            string userAgent = filterContext.HttpContext.Request.UserAgent;
            string loginPage = OwnConfig.GetLoginPage("");

            if (userAgent.ToLower().Contains("micromessenger"))
            {
                log.Info("去往微信浏览器授权验证");
                loginPage = OwnConfig.WxOauth2("");
            }
            else
            {
                log.Info("去往用户登录页面验证");
            }

            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                log.Info("用户没有登录或登录超时");

                bool isAjaxRequest = filterContext.RequestContext.HttpContext.Request.IsAjaxRequest();
                if (isAjaxRequest)
                {
                    MessageBoxModel messageBox = new MessageBoxModel();
                    messageBox.No = Guid.NewGuid().ToString();
                    messageBox.Type = MessageBoxTip.Exception;
                    messageBox.Title = "您没有权限访问,可能链接超时,请登录";
                    messageBox.LoginPage = loginPage;
                    CustomJsonResult jsonResult = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "", messageBox);
                    filterContext.Result = jsonResult;
                    filterContext.Result.ExecuteResult(filterContext);
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    filterContext.Result = new RedirectResult(loginPage);
                }
            }

        }
    }

}