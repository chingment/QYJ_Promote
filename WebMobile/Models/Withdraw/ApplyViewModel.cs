using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models.Withdraw
{
    public class ApplyViewModel : OwnBaseViewModel
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

        public void LoadData(string userId)
        {
            var fund = CurrentDb.Fund.Where(m => m.UserId == userId).FirstOrDefault();

            if (fund != null)
            {
                _availableBalance = fund.AvailableBalance.ToF2Price();
            }

            var withdraw = CurrentDb.Withdraw.Where(m => m.UserId == userId).OrderByDescending(m => m.ApplyTime).FirstOrDefault();
            if (withdraw != null)
            {
                _acName = withdraw.AcName;
                _acIdNumber = withdraw.AcIdNumber;
                _acBank = withdraw.AcBank;
                _acBankCardNumber = withdraw.AcBankCardNumber;
            }
        }
    }
}