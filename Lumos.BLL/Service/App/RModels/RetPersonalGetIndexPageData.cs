using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class RetPersonalGetIndexPageData
    {
        private string _headImgUrl = null;
        private string _nickname = null;
        private string _availableBalance = "0.00";
        private string _servicePhone = "";
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

        public string ServicePhone
        {
            get
            {
                return _servicePhone;
            }
            set
            {
                _servicePhone = value;
            }
        }
    }
}
