using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Biz
{
    public class RetOrderUnifiedOrder
    {
        public string WxPrepayId { get; set; }

        public bool IsBuy { get; set; }

        public string OrderId { get; set; }

        public string OrderSn { get; set; }
    }
}
