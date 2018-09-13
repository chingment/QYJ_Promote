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
    [Table("Fund")]
    public class Fund
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal LockBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
