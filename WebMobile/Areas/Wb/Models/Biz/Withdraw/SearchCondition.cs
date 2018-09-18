using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Areas.Wb.Models.Biz.Withdraw
{
    public class SearchCondition: BaseSearchCondition
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}