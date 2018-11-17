using Lumos.Common;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Admin
{
    public class WithdrawProvider : BaseProvider
    {
        public CustomJsonResult Audit(string pOperater, RopWithdrawAudit rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var withdraw = CurrentDb.Withdraw.Where(m => m.Id == rop.WithdrawId).FirstOrDefault();

                if (withdraw == null)
                {
                    return new CustomJsonResult(ResultType.Failure, "找不到提现记录");
                }

                if (withdraw.Status == Enumeration.WithdrawStatus.Handing)
                {
                    return new CustomJsonResult(ResultType.Failure, "该提现申请正在处理中");
                }

                if (withdraw.Status == Enumeration.WithdrawStatus.Success || withdraw.Status == Enumeration.WithdrawStatus.Failure)
                {
                    return new CustomJsonResult(ResultType.Failure, "该提现申请已经被处理");
                }

                withdraw.AuditComments = rop.AuditComments;

                switch (rop.Operate)
                {
                    case RopWithdrawAuditOperate.Pass:
                        withdraw.Status = Enumeration.WithdrawStatus.Handing;
                        withdraw.Mender = pOperater;
                        withdraw.MendTime = this.DateTime;
                        withdraw.Auditor = pOperater;
                        withdraw.AuditTime = this.DateTime;

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "审核通过");

                        break;
                    case RopWithdrawAuditOperate.NoPass:

                        withdraw.Status = Enumeration.WithdrawStatus.Failure;
                        withdraw.FailureReason = rop.AuditComments;
                        withdraw.Mender = pOperater;
                        withdraw.MendTime = this.DateTime;
                        withdraw.Auditor = pOperater;
                        withdraw.AuditTime = this.DateTime;

                        var fund = CurrentDb.Fund.Where(m => m.ClientId == withdraw.ClientId).FirstOrDefault();

                        fund.AvailableBalance += withdraw.Amount;
                        fund.LockBalance -= withdraw.Amount;
                        fund.Mender = pOperater;
                        fund.MendTime = this.DateTime;

                        var fundTrans = new FundTrans();
                        fundTrans.Id = GuidUtil.New();
                        fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, withdraw.ClientId);
                        fundTrans.ClientId = withdraw.ClientId;
                        fundTrans.ChangeType = Enumeration.FundTransChangeType.WtihdrawFailure;
                        fundTrans.ChangeAmount = withdraw.Amount;
                        fundTrans.CurrentBalance = fund.CurrentBalance;
                        fundTrans.AvailableBalance = fund.AvailableBalance;
                        fundTrans.LockBalance = fund.LockBalance;
                        fundTrans.Creator = pOperater;
                        fundTrans.CreateTime = this.DateTime;
                        fundTrans.Description = string.Format("资金提现失败，原因：{0}", withdraw.FailureReason);
                        fundTrans.TipsIcon = IconUtil.Withdraw;
                        fundTrans.IsNoDisplay = false;
                        CurrentDb.FundTrans.Add(fundTrans);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "审核不通过");
                        break;
                    default:
                        result = new CustomJsonResult(ResultType.Failure, "未知操作");
                        break;
                }

                CurrentDb.SaveChanges();
                ts.Complete();
            }


            return result;

        }


        public CustomJsonResult DoTransfer(string pOperater, RopWithdrawDoTransfer rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var withdraw = CurrentDb.Withdraw.Where(m => m.Id == rop.WithdrawId).FirstOrDefault();

                if (withdraw == null)
                {
                    return new CustomJsonResult(ResultType.Failure, "找不到提现记录");
                }


                if (withdraw.Status == Enumeration.WithdrawStatus.Success || withdraw.Status == Enumeration.WithdrawStatus.Failure)
                {
                    return new CustomJsonResult(ResultType.Failure, "该提现申请已经被处理");
                }

                var fund = CurrentDb.Fund.Where(m => m.ClientId == withdraw.ClientId).FirstOrDefault();
                var fundTrans = new FundTrans();
                switch (rop.Operate)
                {
                    case RopWithdrawDoTransferOperate.Pass:
                        withdraw.Status = Enumeration.WithdrawStatus.Success;
                        withdraw.FailureReason = rop.AuditComments;
                        withdraw.Mender = pOperater;
                        withdraw.MendTime = this.DateTime;
                        withdraw.Auditor = pOperater;
                        withdraw.AuditTime = this.DateTime;

                        fund.CurrentBalance -= withdraw.Amount;
                        fund.LockBalance -= withdraw.Amount;
                        fund.Mender = pOperater;
                        fund.MendTime = this.DateTime;


                        fundTrans.Id = GuidUtil.New();
                        fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, withdraw.ClientId);
                        fundTrans.ClientId = withdraw.ClientId;
                        fundTrans.ChangeType = Enumeration.FundTransChangeType.WtihdrawSuccess;
                        fundTrans.ChangeAmount = -withdraw.Amount;
                        fundTrans.CurrentBalance = fund.CurrentBalance;
                        fundTrans.AvailableBalance = fund.AvailableBalance;
                        fundTrans.LockBalance = fund.LockBalance;
                        fundTrans.Creator = pOperater;
                        fundTrans.CreateTime = this.DateTime;
                        fundTrans.TipsIcon = IconUtil.Withdraw;
                        fundTrans.IsNoDisplay = true;
                        CurrentDb.FundTrans.Add(fundTrans);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "审核通过");

                        break;
                    case RopWithdrawDoTransferOperate.NoPass:

                        withdraw.Status = Enumeration.WithdrawStatus.Failure;
                        withdraw.FailureReason = rop.AuditComments;
                        withdraw.Mender = pOperater;
                        withdraw.MendTime = this.DateTime;
                        withdraw.Auditor = pOperater;
                        withdraw.AuditTime = this.DateTime;


                        fund.AvailableBalance += withdraw.Amount;
                        fund.LockBalance -= withdraw.Amount;
                        fund.Mender = pOperater;
                        fund.MendTime = this.DateTime;

                        fundTrans.Id = GuidUtil.New();
                        fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, withdraw.ClientId);
                        fundTrans.ClientId = withdraw.ClientId;
                        fundTrans.ChangeType = Enumeration.FundTransChangeType.WtihdrawApply;
                        fundTrans.ChangeAmount = withdraw.Amount;
                        fundTrans.CurrentBalance = fund.CurrentBalance;
                        fundTrans.AvailableBalance = fund.AvailableBalance;
                        fundTrans.LockBalance = fund.LockBalance;
                        fundTrans.Creator = pOperater;
                        fundTrans.CreateTime = this.DateTime;
                        fundTrans.Description = string.Format("资金提现失败，原因：{0}", withdraw.FailureReason);
                        fundTrans.TipsIcon = IconUtil.Withdraw;
                        fundTrans.IsNoDisplay = false;
                        CurrentDb.FundTrans.Add(fundTrans);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "审核不通过");
                        break;
                    default:
                        result = new CustomJsonResult(ResultType.Failure, "未知操作");
                        break;
                }

                CurrentDb.SaveChanges();
                ts.Complete();
            }


            return result;

        }

    }
}
