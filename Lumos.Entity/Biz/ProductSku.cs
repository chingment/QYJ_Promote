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
    [Table("ProductSku")]
    public class ProductSku
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ShowPrice { get; set; }
        public string DisplayImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public int SaleQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int StockQuantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
