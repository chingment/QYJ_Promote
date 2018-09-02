using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("PromoteUserCoupon")]
    public class PromoteUserCoupon
    {
        [Key]
        public string Id { get; set; }
        public string PromoteId { get; set; }
        public string PromoteCouponId { get; set; }
        public string WxCouponId { get; set; }
        public string UserId { get; set; }
        public bool IsBuy { get; set; }
        public DateTime BuyTime { get; set; }
        public bool IsGet { get; set; }
        public DateTime GetTime { get; set; }
        public bool IsConsume { get; set; }
        public DateTime ConsumeTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
