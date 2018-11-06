using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class RetOrderGetDetails
    {
        public RetOrderGetDetails()
        {
            this.Skus = new List<Sku>();
            this.Fields = new List<Field>();
        }

        public string OrderSn { get; set; }
        public string SubmitTime { get; set; }
        public string PayTime { get; set; }
        public string CompletedTime { get; set; }
        public string CancledTime { get; set; }
        public string CancelReason { get; set; }
        public Enumeration.OrderStatus Status { get; set; }
        public string StatusName { get; set; }
        public List<Field> Fields { get; set; }
        public List<Sku> Skus { get; set; }

        public class Sku
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public int Quantity { get; set; }

            public string ImgUrl { get; set; }

            public decimal SalePrice { get; set; }

            public decimal ChargeAmount { get; set; }
        }

        public class Field
        {
            public string Name
            {
                get; set;
            }

            public string Value
            {
                get; set;
            }
        }
    }
}
