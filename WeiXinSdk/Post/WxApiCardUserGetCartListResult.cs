using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.WeiXinSdk
{
    public class WxUserCard
    {
        public string code { get; set; }

        public string card_id { get; set; }
    }
    public class WxApiCardUserGetCartListResult : WxApiBaseResult
    {
        public List<WxUserCard> card_list { get; set; }
    }
}
