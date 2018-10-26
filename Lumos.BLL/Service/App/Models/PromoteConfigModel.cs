using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App.Models
{
    public class PromoteConfigModel
    {
        public string PromoteId { get; set; }
        public string PromoteCouponId { get; set; }
        public string RefereeId { get; set; }
        public string ClientId { get; set; }
        public string ShareTitle { get; set; }
        public string ShareDesc { get; set; }
        public string ShareImgUrl { get; set; }
        public bool IsBuy { get; set; }
        public bool IsGet { get; set; }
        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }
        public string StartTime { get; set; }
        public string StartDesc { get; set; }
        public string EndTime { get; set; }
        public bool EndDesc { get; set; }
        public bool Img4GoPersonal { get; set; }
        public bool Img4GetCoupon { get; set; }
        public bool Img4OpenCoupon { get; set; }
        public bool Img4GoInvite { get; set; }
        public bool Img4GoBuy { get; set; }
        public bool IsNeedFillInInfo { get; set; }
    }
}
