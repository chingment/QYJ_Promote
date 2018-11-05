using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class ProductSkuService : BaseProvider
    {
        public CustomJsonResult GetDetails(string pOperater, string pClientId, RupProductSkuGetDetails rup)
        {
            var productSku = CurrentDb.ProductSku.Where(m => m.Id == rup.SkuId).FirstOrDefault();

            if (productSku == null)
            {
                var ret = new RetOperateResult();
                ret.Result = RetOperateResult.ResultType.Failure;
                ret.Remarks = "";
                ret.Message = "商品不存在";
                ret.IsComplete = true;
                ret.Buttons.Add(new RetOperateResult.Button() { Name = "返回个人中心", Color = "red", Url = "/Personal/Index" });
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品不存在", ret);
            }
            else
            {
                var ret = new RetProductSkuGetDetails();

                ret.Id = productSku.Id;
                ret.Name = productSku.Name;
                ret.DisplayImgUrls = productSku.DisplayImgUrls;
                ret.DetailsDes = productSku.DisplayImgUrls;
                ret.SaleQuantity = productSku.SaleQuantity;
                ret.SellQuantity = productSku.SellQuantity;
                ret.StockQuantity = productSku.StockQuantity;

                if (string.IsNullOrEmpty(rup.PromoteId))
                {


                }

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "商品不存在", ret);
            }
        }
    }
}
