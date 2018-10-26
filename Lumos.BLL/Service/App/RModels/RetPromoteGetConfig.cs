using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class RetPromoteGetConfig
    {
        public string Title { get; set; }
        public string PromoteId { get; set; }
        public string PromoteCouponId { get; set; }
        public string RefereeId { get; set; }
        public string ClientId { get; set; }
        public string ShareTitle { get; set; }
        public string ShareDesc { get; set; }
        public string ShareImgUrl { get; set; }
        public bool IsNeedBuy { get; set; }
        public bool IsBuy { get; set; }
        public bool IsGet { get; set; }
        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }
        public string StartTime { get; set; }
        public string NoStartDesc { get; set; }
        public string EndTime { get; set; }
        public string EndDesc { get; set; }
        public string Bg4GoPersonal { get; set; }
        public string Bg4GetCoupon { get; set; }
        public string Bg4OpenCoupon { get; set; }
        public string Bg4GoInvite { get; set; }
        public string Bg4GoBuy { get; set; }
        public bool IsNeedFillInInfo { get; set; }

        public string Bg01 { get; set; }
        public string Bg02 { get; set; }
        public string Bg03 { get; set; }
        public string Bg04 { get; set; }
    }
}
