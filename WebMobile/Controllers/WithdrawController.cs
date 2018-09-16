using Lumos;
using Lumos.BLL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Models.Withdraw;

namespace WebMobile.Controllers
{
    public class WithdrawController : OwnBaseController
    {
        public ActionResult My()
        {
            return View();
        }

        public ActionResult Apply()
        {
            var model = new ApplyViewModel();

            model.LoadData(this.CurrentUserId);
            return View(model);
        }

        [HttpPost]
        public CustomJsonResult Apply(WithdrawApplyPms model)
        {
            model.UserId = this.CurrentUserId;
            model.ApplyMethod = "wechat";
            return BizFactory.Withdraw.Apply(this.CurrentUserId, model);
        }

        [HttpPost]
        public CustomJsonResult GetMy(SearchCondition model)
        {
            var query = (from o in CurrentDb.Withdraw
                         where
                          o.UserId == this.CurrentUserId
                         select new { o.Sn, o.ApplyTime, o.Amount, o.Status, o.AcName, o.ApplyMethod, o.AcIdNumber, o.FailureReason });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.ApplyTime).Skip(pageSize * (model.PageIndex)).Take(pageSize);

            var list = query.ToList();

            List<MyWithdrawModel> oList = new List<MyWithdrawModel>();

            foreach (var item in list)
            {
                var myWithdraw = new MyWithdrawModel();
                myWithdraw.Sn = item.Sn;
                myWithdraw.Title = "提现到我的账户";
                myWithdraw.ApplyTime = item.ApplyTime.ToString("yyyy.MM.dd HH:mm:ss");
                myWithdraw.Amount = item.Amount.ToF2Price();

                switch (item.Status)
                {
                    case Enumeration.WithdrawStatus.Apply:
                    case Enumeration.WithdrawStatus.Handing:
                        myWithdraw.Remarks = "处理中";
                        myWithdraw.RemarksFontColor = "#000000";
                        break;
                    case Enumeration.WithdrawStatus.Success:
                        myWithdraw.Remarks = "成功";
                        myWithdraw.RemarksFontColor = "#1fc31b";
                        break;
                    case Enumeration.WithdrawStatus.Failure:
                        myWithdraw.Remarks = "失败";
                        if (!string.IsNullOrEmpty(item.FailureReason))
                        {
                            myWithdraw.Remarks += "，" + item.FailureReason;
                        }
                        myWithdraw.RemarksFontColor = "#f34754";
                        break;

                }

                oList.Add(myWithdraw);
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = oList };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }
    }
}