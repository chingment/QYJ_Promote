using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Security.Cryptography;
using WebMobile;
using Lumos.DAL;

namespace System.Web
{
    public static class HmlExtension
    {
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

        public static MvcHtmlString initPromote(this HtmlHelper helper, string name, Lumos.Entity.Enumeration.PromoteClass classs)
        {

            LumosDbContext dbContext = new LumosDbContext();
            var promote = dbContext.Promote.OrderByDescending(m => m.CreateTime).ToList();

            if (classs != Lumos.Entity.Enumeration.PromoteClass.Unknow)
            {
                promote = promote.Where(m => m.Class == classs).ToList();
            }

            StringBuilder sb = new StringBuilder();

            string id = name.Replace('.', '_');
            sb.Append("<select id=\"" + id + "\" data-placeholder=\"请选择\" name =\"" + name + "\" class=\"chosen-select\" style=\"width: 230px\" >");
            sb.Append("<option value=\"-1\"></option>");


            string[] arr_selectval = null;

            //if (selectval != null)
            //{
            //    arr_selectval = selectval.Split(',');
            //}

            foreach (var m in promote)
            {
                string selected = "";

                //if (selectval != null)
                //{
                //    if (arr_selectval.Contains(m.Id.ToString()))
                //    {
                //        selected = "selected";
                //    }
                //}

                sb.Append("<option value=\"" + m.Id + "\"   " + selected + "   >&nbsp;" + m.Name + "</option>");
            }

            sb.Append("</select>");
            return new MvcHtmlString(sb.ToString());
        }
    }
}