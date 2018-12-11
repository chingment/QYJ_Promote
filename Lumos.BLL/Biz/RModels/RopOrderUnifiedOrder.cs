using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Biz
{
    public class RopOrderUnifiedOrder
    {
        public RopOrderUnifiedOrder()
        {
            this.Skus = new List<SkuModel>();
        }

        public string PromoteId { get; set; }

        public string RefereerId { get; set; }

        public string OrderId { get; set; }

        public List<SkuModel> Skus { get; set; }

        public class SkuModel
        {
            public string SkuId { get; set; }
        }

    }
}
