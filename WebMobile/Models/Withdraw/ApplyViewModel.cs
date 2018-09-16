using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models.Withdraw
{
    public class ApplyViewModel : OwnBaseViewModel
    {
        private string _availableBalance = "0.00";

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

        public void LoadData(string userId)
        {
            var fund = CurrentDb.Fund.Where(m => m.UserId == userId).FirstOrDefault();

            if (fund != null)
            {
                _availableBalance = fund.AvailableBalance.ToF2Price();
            }

        }
    }
}