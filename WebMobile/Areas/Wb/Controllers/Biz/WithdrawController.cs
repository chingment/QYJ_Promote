using Lumos;
using Lumos.BLL;
using Lumos.BLL.Service.Admin;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Areas.Wb.Controllers
{
    public class WithdrawController : WebMobile.Areas.Wb.Own.OwnBaseController
    {
        public ActionResult ListByDetails()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }


        public ActionResult ListByAudit()
        {
            return View();
        }

        public ActionResult Audit()
        {
            return View();
        }

        public ActionResult ListByDoTransfer()
        {
            return View();
        }

        public ActionResult DoTransfer()
        {
            return View();
        }

        public CustomJsonResult GetDetails(string withdrawId)
        {
            return AdminServiceFactory.Withdraw.GetDetails(this.CurrentUserId, withdrawId);
        }

        [HttpPost]
        public CustomJsonResult GetListByDetails(RupWithdrawGetList rup)
        {
            var query = (from u in CurrentDb.Withdraw

                         join w in CurrentDb.WxUserInfo on u.ClientId equals w.ClientId
                         where (rup.Name == null || u.AcName.Contains(rup.Name))
                         &&
                         (rup.Sn == null || u.Sn.Contains(rup.Sn)) &&
                          (rup.StartTime == null || u.ApplyTime >= rup.StartTime) &&
                        (rup.EndTime == null || u.ApplyTime <= rup.EndTime)
                         select new { u.Id, u.Sn, w.Nickname, u.AcName, u.AcIdNumber, u.Amount, u.ApplyTime, u.Status });

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = 10;
            query = query.OrderBy(r => r.ApplyTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            var list = query.ToList();

            List<object> olist = new List<object>();
            foreach (var item in list)
            {
                string statusName = "";
                switch (item.Status)
                {
                    case Enumeration.WithdrawStatus.Apply:
                        statusName = "待审核";
                        break;
                    case Enumeration.WithdrawStatus.Handing:
                        statusName = "待转账";
                        break;
                    case Enumeration.WithdrawStatus.Success:
                        statusName = "成功";
                        break;
                    case Enumeration.WithdrawStatus.Failure:
                        statusName = "失败";
                        break;
                }
                olist.Add(new
                {
                    Id = item.Id,
                    Sn = item.Sn,
                    Nickname = item.Nickname,
                    AcName = item.AcName,
                    AcIdNumber = item.AcIdNumber,
                    Amount = item.Amount.ToF2Price(),
                    ApplyTime = item.ApplyTime,
                    StatusName = statusName
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public CustomJsonResult GetListByAudit(RupWithdrawGetList rup)
        {
            var query = (from u in CurrentDb.Withdraw

                         join w in CurrentDb.WxUserInfo on u.ClientId equals w.ClientId

                         where (rup.Name == null || u.AcName.Contains(rup.Name))
                         &&
                         u.Status == Lumos.Entity.Enumeration.WithdrawStatus.Apply
                            &&
                         (rup.Sn == null || u.Sn.Contains(rup.Sn)) &&
                                                  (rup.StartTime == null || u.ApplyTime >= rup.StartTime) &&
                        (rup.EndTime == null || u.ApplyTime <= rup.EndTime)
                         select new { u.Id, u.Sn, w.Nickname, u.AcName, u.AcIdNumber, u.Amount, u.ApplyTime });

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = 10;
            query = query.OrderBy(r => r.ApplyTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            var list = query.ToList();

            List<object> olist = new List<object>();
            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    Sn = item.Sn,
                    Nickname = item.Nickname,
                    AcName = item.AcName,
                    AcIdNumber = item.AcIdNumber,
                    Amount = item.Amount.ToF2Price(),
                    ApplyTime = item.ApplyTime,
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public CustomJsonResult Audit(RopWithdrawAudit rop)
        {
            return AdminServiceFactory.Withdraw.Audit(this.CurrentUserId, rop);
        }


        [HttpPost]
        public CustomJsonResult GetListByDoTransfer(RupWithdrawGetList condition)
        {
            var query = (from u in CurrentDb.Withdraw

                         join w in CurrentDb.WxUserInfo on u.ClientId equals w.ClientId

                         where (condition.Name == null || u.AcName.Contains(condition.Name))
                         &&
                         u.Status == Lumos.Entity.Enumeration.WithdrawStatus.Handing
                            &&
                         (condition.Sn == null || u.Sn.Contains(condition.Sn)) &&
                         (condition.StartTime == null || u.ApplyTime >= condition.StartTime) &&
                        (condition.EndTime == null || u.ApplyTime <= condition.EndTime)
                         select new { u.Id, u.Sn, w.Nickname, u.AcName, u.AcIdNumber, u.Amount, u.ApplyTime });

            int total = query.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            query = query.OrderBy(r => r.ApplyTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            var list = query.ToList();

            List<object> olist = new List<object>();
            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    Sn = item.Sn,
                    Nickname = item.Nickname,
                    AcName = item.AcName,
                    AcIdNumber = item.AcIdNumber,
                    Amount = item.Amount.ToF2Price(),
                    ApplyTime = item.ApplyTime,
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }


        [HttpPost]
        public CustomJsonResult DoTransfer(RopWithdrawDoTransfer rop)
        {
            return AdminServiceFactory.Withdraw.DoTransfer(this.CurrentUserId, rop);
        }
    }
}