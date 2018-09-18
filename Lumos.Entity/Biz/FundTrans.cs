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
    [Table("FundTrans")]
    public class FundTrans
    {
        [Key]
        public string Id { get; set; }
        public string Sn { get; set; }
        public string UserId { get; set; }
        public Enumeration.FundTransChangeType ChangeType { get; set; }
        public decimal ChangeAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal LockBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }

        public string TipsIcon { get; set; }
    }
}
