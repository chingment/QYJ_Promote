using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
   public class RetWithdrawGetApplyPageData
    {
        private string _availableBalance = "0.00";

        private string _acName = "";
        private string _acIdNumber = "";
        private string _acBank = "";
        private string _acBankCardNumber = "";

        public string AvailableBalance
        {
            get
            {
                return _availableBalance;
            }
            set
            {
                _availableBalance = value;
            }
        }

        public string AcName
        {
            get
            {
                return _acName;
            }
            set
            {
                _acName = value;
            }
        }
        public string AcIdNumber
        {
            get
            {
                return _acIdNumber;
            }
            set
            {
                _acIdNumber = value;
            }
        }
        public string AcBank
        {
            get
            {
                return _acBank;
            }
            set
            {
                _acBank = value;
            }
        }
        public string AcBankCardNumber
        {
            get
            {
                return _acBankCardNumber;
            }
            set
            {
                _acBankCardNumber = value;
            }
        }
    }
}
