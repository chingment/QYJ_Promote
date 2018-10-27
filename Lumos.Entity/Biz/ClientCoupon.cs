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
    [Table("ClientCoupon")]
    public class ClientCoupon
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public string ClientId { get; set; }
        public string Discounttip { get; set; }
        public string Description { get; set; }
        public decimal Number { get; set; }
        public string NumberType { get; set; }
        public string NumberUnit { get; set; }
        public DateTime? ValidStartTime { get; set; }
        public DateTime? ValidEndTime { get; set; }
        public string PromoteId { get; set; }
        public string PromoteCouponId { get; set; }
        public string OrderId { get; set; }
        public string OrderSn { get; set; }
        public bool IsBuy { get; set; }
        public DateTime BuyTime { get; set; }
        public bool IsGet { get; set; }
        public DateTime? GetTime { get; set; }
        public bool IsConsume { get; set; }
        public DateTime? ConsumeTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public string RefereeId { get; set; }
        public string WxCouponId { get; set; }
        public string WxCouponEncryptCode { get; set; }
        public string WxCouponDecryptCode { get; set; }

    }
}
