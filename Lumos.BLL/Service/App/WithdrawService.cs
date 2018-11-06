using Lumos.Common;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.App
{
    public class WithdrawService : BaseProvider
    {
        private static readonly object lock_Apply = new object();
        public CustomJsonResult Apply(string pOperater, string pClientId, RopWithdrawApply rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            lock (lock_Apply)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var fund = CurrentDb.Fund.Where(m => m.ClientId == pClientId).FirstOrDefault();

                    if (fund == null)
                    {
                        LogUtil.Error("用户Id:" + pClientId + ",在Fund表没有记录");
                        return new CustomJsonResult(ResultType.Failure, "系统繁忙");
                    }

                    if (rop.Amount <= 0)
                    {
                        return new CustomJsonResult(ResultType.Failure, "提现金额必须大于0");
                    }

                    if (fund.AvailableBalance < rop.Amount)
                    {
                        return new CustomJsonResult(ResultType.Failure, "可提现余额不够");
                    }

                    var withdraw = new Withdraw();
                    withdraw.Id = GuidUtil.New();
                    withdraw.Sn = SnUtil.Build(Enumeration.BizSnType.Withraw, pClientId);
                    withdraw.ClientId = pClientId;
                    withdraw.Amount = rop.Amount;
                    withdraw.AcIdNumber = rop.AcIdNumber;
                    withdraw.AcName = rop.AcName;
                    withdraw.AcBank = rop.AcBank;
                    withdraw.AcBankCardNumber = rop.AcBankCardNumber;
                    withdraw.ApplyTime = this.DateTime;
                    withdraw.ApplyMethod = "1";
                    withdraw.Status = Enumeration.WithdrawStatus.Apply;
                    withdraw.Creator = pOperater;
                    withdraw.CreateTime = this.DateTime;
                    CurrentDb.Withdraw.Add(withdraw);

                    fund.AvailableBalance -= rop.Amount;
                    fund.LockBalance += rop.Amount;
                    fund.Mender = pOperater;
                    fund.MendTime = this.DateTime;

                    var fundTrans = new FundTrans();
                    fundTrans.Id = GuidUtil.New();
                    fundTrans.Sn = SnUtil.Build(Enumeration.BizSnType.FundTrans, pClientId);
                    fundTrans.ClientId = pClientId;
                    fundTrans.ChangeType = Enumeration.FundTransChangeType.WtihdrawApply;
                    fundTrans.ChangeAmount = -rop.Amount;
                    fundTrans.CurrentBalance = fund.CurrentBalance;
                    fundTrans.AvailableBalance = fund.AvailableBalance;
                    fundTrans.LockBalance = fund.LockBalance;
                    fundTrans.Creator = pOperater;
                    fundTrans.CreateTime = this.DateTime;
                    fundTrans.TipsIcon = IconUtil.Withdraw;
                    fundTrans.Description = string.Format("资金提现-到{0}({1})", withdraw.AcBank, CommonUtils.GetLastString(withdraw.AcBankCardNumber, 4));
                    CurrentDb.FundTrans.Add(fundTrans);
                    CurrentDb.SaveChanges();
                    ts.Complete();


                    var ret = new RetWithdrawApply();

                    ret.Id = withdraw.Id;

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "提现申请成功", ret);
                }
            }

            return result;

        }
    }
}
