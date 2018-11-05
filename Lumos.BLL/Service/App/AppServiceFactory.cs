using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class AppServiceFactory : BaseFactory
    {
        public static PromoteService Promote
        {
            get
            {
                return new PromoteService();
            }
        }

        public static ProductSkuService ProductSku
        {
            get
            {
                return new ProductSkuService();
            }
        }

    }
}
