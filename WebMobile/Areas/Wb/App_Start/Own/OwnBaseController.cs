using System.Web.Mvc;
using Lumos.Web.Mvc;
using Lumos.DAL;

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
        private LumosDbContext _CurrentDb = null;


        public override string CurrentUserId
        {
            get
            {
                return OwnRequest.GetCurrentUserId();
            }
        }

        public LumosDbContext CurrentDb
        {
            get
            {
                if (_CurrentDb == null)
                {
                    _CurrentDb = new LumosDbContext();
                }

                return _CurrentDb;
            }
        }
    }
}