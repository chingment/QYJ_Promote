using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models.Promote
{
    public class ShareLogModel
    {
        public string Type { get; set; }
        public string PromoteId { get; set; }
        public string RefereeId { get; set; }
        public string ShareLink { get; set; }
    }
}