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
            this.PromoteUser = new PromoteUserModel();
        }

        public string PromoteId { get; set; }

        public PromoteUserModel PromoteUser { get; set; }

        public string RefereerId { get; set; }

        public string OrderId { get; set; }

        public List<SkuModel> Skus { get; set; }


        public class SkuModel
        {
            public string SkuId { get; set; }
        }

        public class PromoteUserModel
        {
            public string CtName { get; set; }

            public string CtPhone { get; set; }

            public string CtSchool { get; set; }

            public bool CtIsStudent { get; set; }
        }

    }
}
