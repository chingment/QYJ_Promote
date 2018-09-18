using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL
{
    public class WithdrawProvider : BaseProvider
    {
        private static readonly object lock_Apply = new object();
        public CustomJsonResult Apply(string pOperater, WithdrawApplyPms pWithdrawApplyPms)
        {
            CustomJsonResult result = new CustomJsonResult();

            lock (lock_Apply)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var fund = CurrentDb.Fund.Where(m => m.UserId == pWithdrawApplyPms.UserId).FirstOrDefault();

                    if (fund == null)
                    {
                        LogUtil.Error("用户Id:" + pWithdrawApplyPms.UserId + ",在Fund表没有记录");
                        return new CustomJsonResult(ResultType.Failure, "系统繁忙");
                    }

                    if (pWithdrawApplyPms.Amount <= 0)
                    {
                        return new CustomJsonResult(ResultType.Failure, "提现金额必须大于0");
                    }

                    if (fund.AvailableBalance < pWithdrawApplyPms.Amount)
                    {
                        return new CustomJsonResult(ResultType.Failure, "可提现余额不够");
                    }

                    var withdraw = new Withdraw();
                    withdraw.Id = GuidUtil.New();
                    withdraw.Sn = SnUtil.Build(Enumeration.BizSnType.Withraw, pWithdrawApplyPms.UserId);
                    withdraw.UserId = pWithdrawApplyPms.UserId;
                    withdraw.Amount = pWithdrawApplyPms.Amount;
                    withdraw.AcIdNumber = pWithdrawApplyPms.AcIdNumber;
                    withdraw.AcName = pWithdrawApplyPms.AcName;
                    withdraw.AcBank = pWithdrawApplyPms.AcBank;
                    withdraw.AcBankCardNumber = pWithdrawApplyPms.AcBankCardNumber;
                    withdraw.ApplyTime = this.DateTime;
                    withdraw.ApplyMethod = "1";
                    withdraw.Status = Enumeration.WithdrawStatus.Apply;
                    withdraw.Creator = pOperater;
                    withdraw.CreateTime = this.DateTime;
                    CurrentDb.Withdraw.Add(withdraw);

                    fund.AvailableBalance -= pWithdrawApplyPms.Amount;
                    fund.LockBalance += pWithdrawApplyPms.Amount;
                    fund.Mender = pOperater;
                    fund.MendTime = this.DateTime;

                    var fundTrans = new FundTrans();
                    fundTrans.Id = GuidUtil.New();
                    fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, pWithdrawApplyPms.UserId);
                    fundTrans.UserId = pWithdrawApplyPms.UserId;
                    fundTrans.ChangeType = Enumeration.FundTransChangeType.WtihdrawApply;
                    fundTrans.ChangeAmount = -pWithdrawApplyPms.Amount;
                    fundTrans.CurrentBalance = fund.CurrentBalance;
                    fundTrans.AvailableBalance = fund.AvailableBalance;
                    fundTrans.LockBalance = fund.LockBalance;
                    fundTrans.Creator = pOperater;
                    fundTrans.CreateTime = this.DateTime;
                    fundTrans.TipsIcon = IconUtil.WithdrawApply;
                    fundTrans.Description = string.Format("发起申请一笔提现，金额：{0}元", pWithdrawApplyPms.Amount);
                    CurrentDb.FundTrans.Add(fundTrans);
                    CurrentDb.SaveChanges();
                    ts.Complete();

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "提现申请成功");
                }
            }

            return result;

        }

        public CustomJsonResult Audit(string pOperater, WithdrawAuditPms pWithdrawAuditPms)
        {
            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var withdraw = CurrentDb.Withdraw.Where(m => m.Id == pWithdrawAuditPms.WithdrawId).FirstOrDefault();

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

                withdraw.AuditComments = pWithdrawAuditPms.AuditComments;

                switch (pWithdrawAuditPms.Operate)
                {
                    case WithdrawAuditOperate.Pass:
                        withdraw.Status = Enumeration.WithdrawStatus.Handing;
                        withdraw.Mender = pOperater;
                        withdraw.MendTime = this.DateTime;
                        withdraw.Auditor = pOperater;
                        withdraw.AuditTime = this.DateTime;

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "审核通过");

                        break;
                    case WithdrawAuditOperate.NoPass:

                        withdraw.Status = Enumeration.WithdrawStatus.Failure;
                        withdraw.FailureReason = pWithdrawAuditPms.AuditComments;
                        withdraw.Mender = pOperater;
                        withdraw.MendTime = this.DateTime;
                        withdraw.Auditor = pOperater;
                        withdraw.AuditTime = this.DateTime;

                        var fund = CurrentDb.Fund.Where(m => m.UserId == withdraw.UserId).FirstOrDefault();

                        fund.AvailableBalance += withdraw.Amount;
                        fund.LockBalance -= withdraw.Amount;
                        fund.Mender = pOperater;
                        fund.MendTime = this.DateTime;

                        var fundTrans = new FundTrans();
                        fundTrans.Id = GuidUtil.New();
                        fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, withdraw.UserId);
                        fundTrans.UserId = withdraw.UserId;
                        fundTrans.ChangeType = Enumeration.FundTransChangeType.WtihdrawApply;
                        fundTrans.ChangeAmount = withdraw.Amount;
                        fundTrans.CurrentBalance = fund.CurrentBalance;
                        fundTrans.AvailableBalance = fund.AvailableBalance;
                        fundTrans.LockBalance = fund.LockBalance;
                        fundTrans.Creator = pOperater;
                        fundTrans.CreateTime = this.DateTime;
                        fundTrans.Description = string.Format("申请一笔提现，金额：{0}元，审核不通过，已退回账户", withdraw.Amount);
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
