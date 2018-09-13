using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{
    public class WithdrawApplyPms
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string AcName { get; set; }
        public string AcIdNumber { get; set; }
        public string ApplyMethod { get; set; }
    }
}
