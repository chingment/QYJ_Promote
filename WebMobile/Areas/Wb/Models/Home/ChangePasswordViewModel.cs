using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Areas.Wb.Models.Home
{
    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}