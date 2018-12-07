using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public enum RopWithdrawDoTransferOperate
    {
        Unknow = 0,
        Pass = 1,
        NoPass = 2
    }

    public class RopWithdrawDoTransfer
    {
        public string WithdrawId { get; set; }

        public string AuditComments { get; set; }

        public RopWithdrawDoTransferOperate Operate { get; set; }
    }
}
