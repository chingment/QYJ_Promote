using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public class BizFactory : BaseFactory
    {
        public static OrderProvider Order
        {
            get
            {
                return new OrderProvider();
            }
        }

        public static WithdrawProvider Withdraw
        {
            get
            {
                return new WithdrawProvider();
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
