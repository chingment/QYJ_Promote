using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    
    public class ReidsMqByCalProfit : ReidsMqObject<ReidsMqByCalProfitModel>
    {
        protected override string MessageQueueType { get { return "Order_Create"; } }
        protected override bool IsTran { get { return false; } }

        protected override string DB_Name { get { return "Order_DBName"; } }
    }
}
