using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models.Promote
{
    public class EditPromoteUserInfoViewModel
    {
        public string PromoteId { get; set; }

        public string CtName { get; set; }
        public string CtPhone { get; set; }
        public string CtIsStudent { get; set; }
        public string CtSchool { get; set; }
    }
}