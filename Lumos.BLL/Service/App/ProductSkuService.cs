using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class ProductSkuService : BaseProvider
    {
        private RetOperateResult GetNoExistsResult()
        {
            var ret = new RetOperateResult();
            ret.Result = RetOperateResult.ResultType.Failure;
            ret.Remarks = "";
            ret.Message = "商品不存在";
            ret.IsComplete = true;
            ret.Buttons.Add(new RetOperateResult.Button() { Name = "返回个人中心", Color = "red", Url = "/Personal/Index" });

            return ret;
        }

        public CustomJsonResult GetDetails(string pOperater, string pClientId, RupProductSkuGetDetails rup)
        {
            var productSku = CurrentDb.ProductSku.Where(m => m.Id == rup.SkuId).FirstOrDefault();

            if (productSku == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商品不存在", GetNoExistsResult());
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
                ret.SalePrice = productSku.SalePrice;
                ret.ShowPrice = productSku.ShowPrice;

                if (!string.IsNullOrEmpty(rup.PromoteId))
                {
                    var clientCoupon = CurrentDb.ClientCoupon.Where(m => m.PromoteId == rup.PromoteId && m.ClientId == pClientId).FirstOrDefault();
                    if (clientCoupon != null)
                    {
                        if (clientCoupon.IsBuy)
                        {
                            var promoteSku = CurrentDb.PromoteSku.Where(m => m.PromoteId == rup.PromoteId && m.SkuId == rup.SkuId).FirstOrDefault();
                            if (promoteSku != null)
                            {
                                if (promoteSku.BuyStartTime <= this.DateTime && promoteSku.BuyEndTime >= this.DateTime)
                                {
                                    ret.SalePrice = promoteSku.SkuSalePrice;
                                }
                            }
                        }
                    }
                }

                if (ret.SalePrice >= ret.ShowPrice)
                {
                    ret.IsHiddenShowPrice = true;
                }

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
            }
        }
    }
}
