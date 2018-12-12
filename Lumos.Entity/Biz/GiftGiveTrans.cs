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
    [Table("GiftGiveTrans")]
    public class GiftGiveTrans
    {
        [Key]
        public string Id { get; set; }
        public string Sn { get; set; }
        public string ClientId { get; set; }
        public string SkuId { get; set; }
        public Enumeration.GiftGiveTransType ChangeType { get; set; }
        public decimal ChangeQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal LockQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string TipsIcon { get; set; }
        public bool IsNoDisplay { get; set; }
    }
}
