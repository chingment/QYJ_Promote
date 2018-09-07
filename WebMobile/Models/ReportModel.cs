using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models
{
    public class ReportModel
    {
        public Enumeration.OperateType Operate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string TableHtml { get; set; }
    }
}