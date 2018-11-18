using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.App
{
    public class OrderService : BaseProvider
    {

        public CustomJsonResult GetDetails(string pOperater, string pClientId, string orderId)
        {
            var result = new CustomJsonResult();

            var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();
            if (order == null)
            {
                var ret = new RetOperateResult();
                ret.Result = RetOperateResult.ResultType.Failure;
                ret.Message = "系统找不到该订单号";
                ret.IsComplete = true;
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "查询支付结果失败：找不到该订单", ret);
            }
            else
            {
                var ret = new RetOrderGetDetails();

                ret.OrderSn = order.Sn;
                ret.Status = order.Status;
                ret.StatusName = order.Status.GetCnName();
                ret.SubmitTime = order.SubmitTime.ToUnifiedFormatDateTime();
                ret.PayTime = order.PayTime.ToUnifiedFormatDateTime();
                ret.CompletedTime = order.CompletedTime.ToUnifiedFormatDateTime();
                ret.CancledTime = order.CancledTime.ToUnifiedFormatDateTime();
                ret.CancelReason = order.CancelReason;

                var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == orderId).ToList();

                foreach (var item in orderDetails)
                {
                    ret.Skus.Add(new RetOrderGetDetails.Sku { Id = item.SkuId, ImgUrl = item.SkuImgUrl, Name = item.SkuName, Quantity = item.Quantity, SalePrice = item.SalePrice, ChargeAmount = item.ChargeAmount });
                }

                var fieldBlock = new RetOrderGetDetails.Block();

                fieldBlock.Name = "联系人";
                fieldBlock.Fields.Add(new RetOrderGetDetails.Field { Name = "宝宝姓名", Value = order.CtName });
                fieldBlock.Fields.Add(new RetOrderGetDetails.Field { Name = "联系电话", Value = order.CtPhone });
                fieldBlock.Fields.Add(new RetOrderGetDetails.Field { Name = "校区地址", Value = order.CtSchool });

                ret.FieldBlocks.Add(fieldBlock);

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
            }
        }
    }
}
