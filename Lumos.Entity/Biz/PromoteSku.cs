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
    [Table("PromoteSku")]
    public class PromoteSku
    {
        [Key]
        public string Id { get; set; }
        public string SkuId { get; set; }
        public string SkuName { get; set; }
        public decimal SkuSalePrice { get; set; }
        public string PromoteId { get; set; }
        public string WxCouponId { get; set; }
        public DateTime BuyStartTime { get; set; }
        public DateTime BuyEndTime { get; set; }
        public int SellQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int StockQuantity { get; set; }
        public int SaleQuantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public string RefereePromoteId { get; set; }

        public string ExtAtrrs { get; set; }
    }
}
