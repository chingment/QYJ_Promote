using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Lumos.Redis;
using Newtonsoft.Json;
using Lumos.Common;

namespace Lumos.BLL
{


    public class UserResultNotify
    {
        public string errMsg { get; set; }
    }

    public class PayProvider : BaseProvider
    {
        private static readonly object goSettlelock = new object();
        public CustomJsonResult ResultNotify(int operater, Enumeration.OrderNotifyLogNotifyFrom from, string content, string orderSn = "")
        {

            return null;
        }

        private CustomJsonResult PayCompleted(int operater, string orderSn, DateTime completedTime, string coupon_id, decimal wxDiscountFee)
        {
            
            return null;
        }
    }
}
