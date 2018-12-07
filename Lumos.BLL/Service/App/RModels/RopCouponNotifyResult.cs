using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class RopCouponNotifyResult
    {
        public string PromoteId { get; set; }

        public List<CouponResult> Coupons { get; set; }

        public class CouponResult
        {
            public string WxCouponId { get; set; }

            public string WxCouponEncryptCode { get; set; }
        }
    }
}
