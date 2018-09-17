using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Areas.Wb.Models.Biz.Withdraw
{
    public class AuditViewModel : OwnBaseViewModel
    {
        private WxUserInfo _userInfo = new WxUserInfo();
        private Lumos.Entity.Withdraw _withdraw = new Lumos.Entity.Withdraw();


        public WxUserInfo UserInfo
        {
            get
            {
                return _userInfo;

            }
            set
            {
                _userInfo = value;
            }
        }

        public Lumos.Entity.Withdraw Withdraw
        {
            get
            {
                return _withdraw;

            }
            set
            {
                _withdraw = value;
            }
        }

        public void LoadData(string id)
        {
            var withdraw = CurrentDb.Withdraw.Where(m => m.Id == id).FirstOrDefault();
            if (withdraw != null)
            {
                _withdraw = withdraw;

                var userInfo = CurrentDb.WxUserInfo.Where(m => m.UserId == withdraw.UserId).FirstOrDefault();

                if (userInfo != null)
                {
                    _userInfo = userInfo;
                }
            }
        }

    }
}