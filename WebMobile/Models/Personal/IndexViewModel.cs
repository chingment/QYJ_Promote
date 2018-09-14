using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models.Personal
{
    public class IndexViewModel : OwnBaseViewModel
    {
        private string _headImgUrl = null;
        private string _nickname = null;
        private string _availableBalance = "0.00";

        public string HeadImgUrl
        {
            get
            {
                return _headImgUrl;
            }
            set
            {
                _headImgUrl = value;
            }
        }
        public string Nickname
        {
            get
            {
                return _nickname;
            }
            set
            {
                _nickname = value;
            }
        }

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
            var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.UserId == userId).FirstOrDefault();
            if (wxUserInfo != null)
            {
                _nickname = wxUserInfo.Nickname;
                _headImgUrl = wxUserInfo.HeadImgUrl;

                var fund = CurrentDb.Fund.Where(m => m.UserId == userId).FirstOrDefault();

                if (fund != null)
                {
                    _availableBalance = fund.AvailableBalance.ToF2Price();
                }
            }
        }
    }
}