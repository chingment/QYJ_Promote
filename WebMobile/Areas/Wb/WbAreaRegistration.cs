using System.Web.Mvc;

namespace WebMobile.Areas.Wb
{
    public class WbAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Wb";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
  "Wb_Biz_default",
  "Wb/Biz/{controller}/{action}/{id}",
  new { action = "Index", id = UrlParameter.Optional },
 namespaces: new string[] { "WebMobile.Areas.Wb.Controllers" }

);

            context.MapRoute(
                "Wb_default",
                "Wb/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "WebMobile.Areas.Wb.Controllers" }
            );
         
        }

     
    }
}