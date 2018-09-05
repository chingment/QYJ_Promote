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
        public string PromoteId { get; set; }
        public string ProductSkuId { get; set; }
        public string WxCouponId { get; set; }
        public int Quantity { get; set; }
        public int QuantityBySold { get; set; }
        public int QuantityByUnSold { get; set; }
        public int QuantityByReceived { get; set; }
        public int QuantityByUnreceived { get; set; }
        public int QuantityByConsumed { get; set; }
        public int QuantityByUnConsumed { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
