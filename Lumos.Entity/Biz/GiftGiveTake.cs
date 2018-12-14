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
    [Table("GiftGiveTake")]
    public class GiftGiveTake
    {
        [Key]
        public string Id { get; set; }
        public string Sn { get; set; }
        public string ClientId { get; set; }
        public int Quantity { get; set; }
        public DateTime TakeTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
