using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models.Coupon
{
    public class MyCouponModel
    {
        public string Name { get; set; }

        public string Amount { get; set; }

        public string Discounttip { get; set; }

        public string Validdate { get; set; }

        public string Description { get; set; }

        public string StatusName { get; set; }

        public int Status { get; set; }

        public string WxCouponId { get; set; }

        public string WxCouponDecryptCode { get; set; }

        public string GetMethod { get; set; }

        public string GetUrl { get; set; }
    }
}