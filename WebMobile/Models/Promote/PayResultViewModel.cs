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

        public string IsSuccessed { get; set; }

        public string IsGetCoupon { get; set; }
    }
}