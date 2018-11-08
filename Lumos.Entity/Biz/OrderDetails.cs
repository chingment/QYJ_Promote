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
    [Table("OrderDetails")]
    public class OrderDetails
    {
        [Key]
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public string PromoteId { get; set; }
        public string PromoteSkuId { get; set; }
        public string SkuId { get; set; }
        public string SkuName { get; set; }
        public string SkuImgUrl { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }

        public Enumeration.OrderDetailsStatus Status { get; set; }

        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
