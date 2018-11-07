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
                ret.DetailsDes = productSku.DetailsDes;
                ret.SaleQuantity = productSku.SaleQuantity;
                ret.SellQuantity = productSku.SellQuantity;
                ret.StockQuantity = productSku.StockQuantity;
                ret.SalePrice = productSku.SalePrice;
                ret.ShowPrice = productSku.ShowPrice;


                ret.BriefTags.Add("限时秒杀");
                ret.BriefTags.Add("双11活动");


                if (string.IsNullOrEmpty(rup.PromoteId))
                {
                    ret.IsCanBuy = false;
                    ret.BuyBtn.Text = "活动已结束";
                    ret.BuyBtn.Enabled = false;
                }
                else
                {
                    var promoteSkus = CurrentDb.PromoteSku.Where(m => m.PromoteId == rup.PromoteId && m.SkuId == rup.SkuId).OrderBy(m => m.BuyStartTime).ToList();
                    if (promoteSkus.Count > 0)
                    {
                        ret.BriefTags.Add("专享特惠");

                        ret.IsFlashSale = true;
                        ret.FlashSaleStSecond = 10;
                        ret.FlashSaleEnSecond = 10;

                        foreach (var item in promoteSkus)
                        {
                            if (item.BuyStartTime <= this.DateTime && item.BuyEndTime >= this.DateTime)
                            {
                                ret.SalePrice = item.SkuSalePrice;
                                break;
                            }
                        }


                    }

                    var isCanBuy = false;
                    var clientCoupon = CurrentDb.ClientCoupon.Where(m => m.PromoteId == rup.PromoteId && m.ClientId == pClientId).FirstOrDefault();
                    if (clientCoupon != null)
                    {
                        if (clientCoupon.IsBuy)
                        {
                            isCanBuy = true;
                        }
                    }

                    if (isCanBuy)
                    {
                        ret.IsCanBuy = true;
                        ret.BuyBtn.Text = "立即购买";
                        ret.BuyBtn.Enabled = false;
                    }
                    else
                    {
                        ret.IsCanBuy = false;
                        ret.BuyBtn.Text = "您没有资格购买";
                        ret.BuyBtn.Enabled = false;
                    }
                }

                if (ret.SalePrice >= ret.ShowPrice)
                {
                    ret.ShowPriceIsInVisiable = true;
                }

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
            }
        }
    }
}
