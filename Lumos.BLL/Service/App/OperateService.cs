using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class OperateService : BaseProvider
    {
        public CustomJsonResult GetResult(string pOperater, string pClient, RupOperateGetResult rup)
        {
            var result = new CustomJsonResult();

            switch (rup.Type)
            {
                case RupOperateGetResultType.Pay:
                    result = GetResultByPay(pOperater, rup);
                    break;
                case RupOperateGetResultType.Withdraw:
                    result = GetResultByWithdraw(pOperater, rup);
                    break;
            }

            return result;
        }

        private CustomJsonResult GetResultByPay(string pOperater, RupOperateGetResult rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetOperateResult();
            var order = CurrentDb.Order.Where(m => m.Id == rup.Id).FirstOrDefault();

            if (order == null)
            {
                ret.Result = RetOperateResult.ResultType.Failure;
                ret.Message = "系统找不到该订单号";
                ret.IsComplete = true;
                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "查询支付结果失败：找不到该订单", ret);
            }

            switch (order.Status)
            {
                case Enumeration.OrderStatus.Submitted:
                    ret.Result = RetOperateResult.ResultType.Success;
                    ret.Message = "该订单未支付";
                    ret.IsComplete = true;
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单未支付", ret);
                    break;
                case Enumeration.OrderStatus.WaitPay:
                    ret.Result = RetOperateResult.ResultType.Success;
                    ret.IsComplete = false;
                    ret.Message = "该订单未支付";
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单未支付", ret);
                    break;
                case Enumeration.OrderStatus.Payed:
                    ret.Result = RetOperateResult.ResultType.Success;
                    ret.Remarks = "3个工作日内校区客服会致电联系您上课时间等情况，请保持电话畅通，谢谢";
                    ret.Message = "支付成功";
                    ret.IsComplete = true;
                    ret.Buttons.Add(new RetOperateResult.Button() { Name = "回到首页", Color = "red", Url = "/Personal/Index" });
                    ret.Buttons.Add(new RetOperateResult.Button() { Name = "查看详情", Color = "green", Url = string.Format("/Order/Details?id={0}", order.Id) });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "订单号", Value = order.Sn });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "支付金额", Value = order.ChargeAmount.ToF2Price() });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "提交时间", Value = order.SubmitTime.ToUnifiedFormatDateTime() });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "支付时间", Value = order.PayTime.ToUnifiedFormatDateTime() });
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单已成功", ret);
                    break;
                case Enumeration.OrderStatus.Completed:
                    ret.Result = RetOperateResult.ResultType.Success;
                    ret.Message = "该订单已经完成";
                    ret.IsComplete = true;
                    ret.Buttons.Add(new RetOperateResult.Button() { Name = "回到首页", Color = "red", Url = "/Personal/Index" });
                    ret.Buttons.Add(new RetOperateResult.Button() { Name = "查看详情", Color = "green", Url = string.Format("/Order/Details?id={0}", order.Id) });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "订单号", Value = order.Sn });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "支付金额", Value = order.ChargeAmount.ToF2Price() });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "提交时间", Value = order.SubmitTime.ToUnifiedFormatDateTime() });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "支付时间", Value = order.PayTime.ToUnifiedFormatDateTime() });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "完成时间", Value = order.CompletedTime.ToUnifiedFormatDateTime() });
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单已完成", ret);
                    break;
                case Enumeration.OrderStatus.Cancled:
                    ret.Result = RetOperateResult.ResultType.Success;
                    ret.Message = "该订单已经取消";
                    ret.IsComplete = true;
                    ret.Buttons.Add(new RetOperateResult.Button() { Name = "回到首页", Color = "red", Url = "" });
                    ret.Buttons.Add(new RetOperateResult.Button() { Name = "查看详情", Color = "green", Url = string.Format("/Order/Details?id={0}", order.Id) });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "订单号", Value = order.Sn });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "提交时间", Value = order.SubmitTime.ToUnifiedFormatDateTime() });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "取消时间", Value = order.CancledTime.ToUnifiedFormatDateTime() });
                    ret.Fields.Add(new RetOperateResult.Field() { Name = "取消原因", Value = order.CancelReason });
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单已取消", ret);
                    break;
                default:
                    break;
            }

            return result;
        }


        private CustomJsonResult GetResultByWithdraw(string pOperater, RupOperateGetResult rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetOperateResult();
            var withdraw = CurrentDb.Withdraw.Where(m => m.Id == rup.Id).FirstOrDefault();

            if (withdraw == null)
            {
                ret.Result = RetOperateResult.ResultType.Failure;
                ret.Message = "系统找不到该提现记录";
                ret.IsComplete = true;
                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "查询结果失败：找不到该提现记录", ret);
            }


            switch (withdraw.Status)
            {
                case Enumeration.WithdrawStatus.Apply:
                case Enumeration.WithdrawStatus.Handing:
                    ret.Result = RetOperateResult.ResultType.Success;
                    ret.Message = "提现申请成功，正在处理中";
                    ret.IsComplete = true;
                    ret.Remarks = "因提现订单较多，我司统一在11月6日/12日/20日，统一进行打款到您的账户，请注意在这3天查收到款情况，如有任何问题，欢迎咨询：020-8231086";
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单未支付", ret);
                    break;
                case Enumeration.WithdrawStatus.Success:
                    ret.Result = RetOperateResult.ResultType.Success;
                    ret.IsComplete = true;
                    ret.Message = "提现成功";
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单未支付", ret);
                    break;
                case Enumeration.WithdrawStatus.Failure:
                    ret.Result = RetOperateResult.ResultType.Failure;
                    ret.Remarks = "";
                    ret.Message = "提现失败";
                    ret.IsComplete = true;
                    break;
                default:
                    break;
            }

            ret.Buttons.Add(new RetOperateResult.Button() { Name = "继续提现", Color = "red", Url = "/Withdraw/Apply" });
            ret.Buttons.Add(new RetOperateResult.Button() { Name = "回到首页", Color = "green", Url = "/Personal/Index" });

            ret.Fields.Add(new RetOperateResult.Field() { Name = "流水号", Value = withdraw.Sn });
            ret.Fields.Add(new RetOperateResult.Field() { Name = "提现金额", Value = withdraw.Amount.ToF2Price() });
            ret.Fields.Add(new RetOperateResult.Field() { Name = "提交时间", Value = withdraw.ApplyTime.ToUnifiedFormatDateTime() });

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);

            return result;
        }
    }
}
