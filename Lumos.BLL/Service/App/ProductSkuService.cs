﻿using Lumos.Entity;
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
                var ret_Operate = new RetOperateResult();
                ret_Operate.Result = RetOperateResult.ResultType.Failure;
                ret_Operate.Remarks = "";
                ret_Operate.Message = "该商品不存在";
                ret_Operate.IsComplete = true;
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "回到首页", Color = "green", Url = "/Personal/Index" });
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "个人中心", Color = "red", Url = "/Personal/Index" });
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品不存在", ret_Operate);
            }

            var promote = CurrentDb.Promote.Where(m => m.Id == rup.PromoteId).FirstOrDefault();
            if (promote == null)
            {
                var ret_Operate = new RetOperateResult();
                ret_Operate.Result = RetOperateResult.ResultType.Failure;
                ret_Operate.Remarks = "";
                ret_Operate.Message = "该活动无效";
                ret_Operate.IsComplete = true;
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "回到首页", Color = "green", Url = "/Personal/Index" });
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "个人中心", Color = "red", Url = "/Personal/Index" });
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该活动无效", ret_Operate);
            }

            var orderDetails = CurrentDb.OrderDetails.Where(m => m.PromoteId == rup.PromoteId && m.ProductSkuId == rup.SkuId && m.ClientId == pClientId && m.Status == Entity.Enumeration.OrderDetailsStatus.Payed).FirstOrDefault();
            if (orderDetails != null)
            {
                var ret_Operate = new RetOperateResult();
                ret_Operate.Result = RetOperateResult.ResultType.Success;
                ret_Operate.Remarks = "";
                ret_Operate.Message = "您已购买成功";
                ret_Operate.IsComplete = true;
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "个人中心", Color = "red", Url = "/Personal/Index" });
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "查看详情", Color = "green", Url = string.Format("/Order/Details?id={0}", orderDetails.OrderId) });
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "您已购买成功", ret_Operate);
            }

            var isHasBuyCoupon = false;
            var clientCoupon = CurrentDb.ClientCoupon.Where(m => m.PromoteId == rup.PromoteId && m.ClientId == pClientId).FirstOrDefault();
            if (clientCoupon != null)
            {
                if (clientCoupon.IsBuy)
                {
                    isHasBuyCoupon = true;
                }
            }

            if (!isHasBuyCoupon)
            {
                var ret_Operate = new RetOperateResult();
                ret_Operate.Result = RetOperateResult.ResultType.Success;
                ret_Operate.Remarks = "";
                ret_Operate.Message = "您没有资格参与，谢谢关注";
                ret_Operate.IsComplete = true;
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "回到首页", Color = "green", Url = "/Personal/Index" });
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "个人中心", Color = "red", Url = "/Personal/Index" });
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "您没有资格参与，谢谢关注", ret_Operate);

            }


            var promoteSkus = CurrentDb.PromoteSku.Where(m => m.PromoteId == rup.PromoteId && m.SkuId == rup.SkuId).OrderBy(m => m.BuyStartTime).ToList();
            if (promoteSkus.Count == 0)
            {
                var ret_Operate = new RetOperateResult();
                ret_Operate.Result = RetOperateResult.ResultType.Success;
                ret_Operate.Remarks = "";
                ret_Operate.Message = "该商品不存在";
                ret_Operate.IsComplete = true;
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "回到首页", Color = "green", Url = "/Personal/Index" });
                ret_Operate.Buttons.Add(new RetOperateResult.Button() { Name = "个人中心", Color = "red", Url = "/Personal/Index" });
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该商品不存在注", ret_Operate);
            }

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
            ret.BriefTags.Add("专享特惠");

            ret.IsFlashSale = true;

            PromoteSku curPromoteSku = null;
            foreach (var item in promoteSkus)
            {
                if (item.BuyEndTime >= this.DateTime)
                {
                    curPromoteSku = item;
                    ret.SalePrice = item.SkuSalePrice;
                    break;
                }
            }

            if (curPromoteSku == null)
            {
                ret.IsCanBuy = false;
                ret.BuyBtn.Text = "活动已结束";
                ret.BuyBtn.Enabled = false;
            }
            else
            {
                ret.FlashSaleStSecond = Convert.ToInt32((curPromoteSku.BuyStartTime - DateTime.Now).TotalSeconds);
                ret.FlashSaleEnSecond = Convert.ToInt32((curPromoteSku.BuyEndTime - DateTime.Now).TotalSeconds);

                var orderDetails2 = CurrentDb.OrderDetails.Where(m => m.PromoteId == rup.PromoteId && m.ProductSkuId == rup.SkuId && m.ClientId == pClientId && m.Status == Entity.Enumeration.OrderDetailsStatus.WaitPay).FirstOrDefault();

                if (orderDetails2 == null)
                {
                    if (curPromoteSku.SellQuantity <= 0)
                    {
                        ret.IsCanBuy = false;
                        ret.BuyBtn.Text = "已售完";
                        ret.BuyBtn.Enabled = false;
                    }
                    else
                    {
                        ret.IsCanBuy = true;
                        ret.BuyBtn.Text = "立即购买";
                        ret.BuyBtn.Enabled = true;
                    }
                }
                else
                {
                    ret.OrderId = orderDetails2.OrderId;
                    ret.IsCanBuy = true;
                    ret.BuyBtn.Text = "立即购买";
                    ret.BuyBtn.Enabled = true;
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
