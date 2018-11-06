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
        }

        public string OrderSn { get; set; }
        public string SubmitTime { get; set; }
        public string PayTime { get; set; }
        public string CompletedTime { get; set; }
        public Enumeration.OrderStatus Status { get; set; }
        public string StatusName { get; set; }
        public List<Sku> Skus { get; set; }
        public Student Stu { get; set; }

        public class Sku
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Quantity { get; set; }

            public string ImgUrl { get; set; }

            public decimal SalePrice { get; set; }

            public decimal ChargeAmount { get; set; }
        }

        public class Student
        {
            public string CtName { get; set; }
            public string CtPhone { get; set; }
            public string CtIsStudent { get; set; }
            public string CtSchool { get; set; }
        }
    }
}
