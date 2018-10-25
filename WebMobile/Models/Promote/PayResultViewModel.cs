using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models.Promote
{
    public class PayResultViewModel
    {
        public string PromoteId { get; set; }

        public string OrderSn { get; set; }

        public bool IsSuccessed { get; set; }

        public bool IsGetCoupon { get; set; }

        public bool PromoteIsEnd { get; set; }

        public string RefereeId { get; set; }
    }
}