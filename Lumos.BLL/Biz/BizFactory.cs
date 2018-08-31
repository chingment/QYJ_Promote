using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public class BizFactory : BaseFactory
    {
        public static PayProvider Pay
        {
            get
            {
                return new PayProvider();
            }
        }

        public static WxUserProvider WxUser
        {
            get
            {
                return new WxUserProvider();
            }
        }
    }
}
