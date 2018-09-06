using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models.Promote
{
    public class CouponViewModel
    {
        public string PromoteId { get; set; }

        public string PromoteCouponId { get; set; }

        public bool PromoteIsEnd { get; set; }

        public string RefereeId { get; set; }
    }
}