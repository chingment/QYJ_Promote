using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lumos.BLL.Service.App
{
    public class MyWithdrawModel
    {
        public string Sn { get; set; }
        public string Title { get; set; }
        public string Amount { get; set; }
        public string Remarks { get; set; }
        public string RemarksFontColor { get; set; }
        public string ApplyTime { get; set; }
    }
}