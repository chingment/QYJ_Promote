using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{
    public enum WithdrawDoTransferOperate
    {
        Unknow = 0,
        Pass = 1,
        NoPass = 2
    }

    public class WithdrawDoTransferPms
    {
        public string WithdrawId { get; set; }

        public string AuditComments { get; set; }

        public WithdrawDoTransferOperate Operate { get; set; }
    }
}
