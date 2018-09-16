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
    [Table("Withdraw")]
    public class Withdraw
    {
        [Key]
        public string Id { get; set; }
        public string Sn { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string AcName { get; set; }
        public string AcIdNumber { get; set; }
        public Enumeration.WithdrawStatus Status { get; set; }
        public string ApplyMethod { get; set; }
        public DateTime ApplyTime { get; set; }
        public string SettlementMethod { get; set; }
        public DateTime? SettlementTime { get; set; }
        public string FailureReason { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
