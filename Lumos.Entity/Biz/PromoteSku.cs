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
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
