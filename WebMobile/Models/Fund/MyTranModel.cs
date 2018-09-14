using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMobile.Models.Fund
{
    public class MyTranModel
    {
        public string Sn { get; set; }

        public string ChangeAmount { get; set; }

        public string ChangeType { get; set; }

        public string ChangeFeature { get; set; }

        public string TransTime { get; set; }

        public string Description { get; set; }

        public string Sign { get; set; }
    }
}