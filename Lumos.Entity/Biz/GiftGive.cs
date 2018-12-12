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
    [Table("GiftGive")]
    public class GiftGive
    {
        [Key]
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string SkuId { get; set; }
        public int CurrentQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
