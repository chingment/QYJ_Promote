using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Security.Cryptography;
using WebMobile;

namespace System.Web
{
    public static class HmlExtension
    {
        public static IHtmlString TabbarIcon(this HtmlHelper htmlhelper, string name)
        {
            string selected = "";
            string img = "";
            string url = HttpContext.Current.Request.Url.AbsolutePath;
            switch (name)
            {
                case "首页":
                    img = "home";
                    if (url.IndexOf("/Home/Index") > -1)
                    {
                        selected = "selected";
                        img += "_active";
                    }
                    break;
                case "专题":
                    img = "specialcolumn";
                    if (url.IndexOf("/SpecialColumn/Index") > -1)
                    {
                        selected = "selected";
                        img += "_active";
                    }
                    break;
                case "分类":
                    img = "catalog";
                    if (url.IndexOf("/GalleryColumn/Index") > -1)
                    {
                        selected = "selected";
                        img += "_active";
                    }
                    break;
                case "购物车":
                    img = "cart";
                    if (url.IndexOf("/Cart/Index") > -1)
                    {
                        selected = "selected";
                        img += "_active";
                    }
                    break;
                case "个人":
                    img = "personal";
                    if (url.IndexOf("/Personal/Index") > -1)
                    {
                        selected = "selected";
                        img += "_active";
                    }
                    break;
            }
            img = "footer_tab_icon_" + img + ".png";

            StringBuilder sb = new StringBuilder();
            sb.Append("<span class=\"bar-icon\">");
            sb.Append("<img src = \"\" />");
            sb.Append("</span>");
            sb.Append("<span class=\"bar-txt "+ selected + "\">" + name + "</span>");

            return new MvcHtmlString(sb.ToString());

        }

        public static IHtmlString IsInPermission(this HtmlHelper helper, object value, params string[] permissions)
        {
            if (permissions == null)
                return helper.Raw(value);

            if (permissions.Length == 0)
                return helper.Raw(value);

            bool isHas = OwnRequest.IsInPermission(permissions);
            if (isHas)
            {
                return helper.Raw(value);
            }
            else
            {
                return helper.Raw("");
            }
        }
    }
}