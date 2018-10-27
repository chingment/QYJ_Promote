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
    [Table("PromoteCoupon")]
    public class PromoteCoupon
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Discounttip { get; set; }
        public string Description { get; set; }
        public decimal Number { get; set; }
        public string NumberType { get; set; }
        public string NumberUnit { get; set; }
        public DateTime? ValidStartTime { get; set; }
        public DateTime? ValidEndTime { get; set; }
        public string PromoteId { get; set; }
        public string ProductSkuId { get; set; }
        public string WxCouponId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
