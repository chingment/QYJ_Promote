using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class RetPromoteGetPang
    {
        public RetPromoteGetPang()
        {
            this.PangCount = new List<PangCountModel>();
        }

        public List<PangCountModel> PangCount { get; set; }

        public class PangCountModel
        {
            public string HeadImgUrl { get; set; }
            public string Nickname { get; set; }

            public int Count { get; set; }
        }
    }
}
