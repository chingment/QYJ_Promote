using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{

    public enum RopWithdrawAuditOperate
    {
        Unknow = 0,
        Pass = 1,
        NoPass = 2
    }

    public class RopWithdrawAudit
    {
        public string WithdrawId { get; set; }

        public string AuditComments { get; set; }

        public RopWithdrawAuditOperate Operate { get; set; }
    }
}
