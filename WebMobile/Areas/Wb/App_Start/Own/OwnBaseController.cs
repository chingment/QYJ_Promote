using System.Web.Mvc;
using Lumos.Web.Mvc;

namespace WebMobile.Areas.Wb.Own
{

    /// <summary>
    /// BaseController用来扩展Controller,凡是在都该继承BaseController
    /// </summary>
    [WebMobile.Areas.Wb.Own.OwnException]
    [WebMobile.Areas.Wb.Own.OwnAuthorize]
    [ValidateInput(false)]
    public abstract class OwnBaseController : BaseController
    {

        public override string CurrentUserId
        {
            get
            {
                return OwnRequest.GetCurrentUserId();
            }
        }
    }
}