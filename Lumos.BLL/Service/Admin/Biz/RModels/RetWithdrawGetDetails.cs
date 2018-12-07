using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public class RetWithdrawGetDetails
    {
        public string Nickname { get; set; }

        public string WithdrawSn { get; set; }

        public string AcBank { get; set; }

        public string AcBankCardNumber { get; set; }

        public string AcName { get; set; }

        public string AcIdNumber { get; set; }

        public string Amount { get; set; }

        public Lumos.Entity.Enumeration.WithdrawStatus Status { get; set; }

        public string ApplyTime { get; set; }
        public string StatusName { get; set; }
        public string SettlementTime { get; set; }
        public string FailureReason { get; set; }
    }
}
