using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{

    public enum WithdrawAuditOperate
    {
        Unknow = 0,
        Pass = 1,
        NoPass = 2
    }

    public class WithdrawAuditPms
    {
        public string WithdrawId { get; set; }

        public string AuditComments { get; set; }

        public WithdrawAuditOperate Operate { get; set; }
    }
}
