using Lumos;
using Lumos.BLL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Areas.Wb.Models.Biz.Withdraw;

namespace WebMobile.Areas.Wb.Controllers
{
    public class WithdrawController : WebMobile.Areas.Wb.Own.OwnBaseController
    {
        public ActionResult ListByDetails()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            var model = new DetailsViewModel();
            model.LoadData(id);
            return View(model);
        }


        public ActionResult ListByAudit()
        {
            return View();
        }

        public ActionResult Audit(string id)
        {
            var model = new AuditViewModel();
            model.LoadData(id);
            return View(model);
        }

        public ActionResult ListByDoTransfer()
        {
            return View();
        }

        public ActionResult DoTransfer(string id)
        {
            var model = new DoTransferViewModel();
            model.LoadData(id);
            return View(model);
        }

        [HttpPost]
        public CustomJsonResult GetListByDetails(SearchCondition condition)
        {
            var query = (from u in CurrentDb.Withdraw

                         join w in CurrentDb.WxUserInfo on u.UserId equals w.UserId
                         where (condition.Name == null || u.AcName.Contains(condition.Name))
                         select new { u.Id, u.Sn, w.Nickname, u.AcName, u.AcIdNumber, u.Amount, u.ApplyTime, u.Status });

            int total = query.Count();

            int pageIndex = condition.PageIndex;
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
        public CustomJsonResult GetListByAudit(SearchCondition condition)
        {
            var query = (from u in CurrentDb.Withdraw

                         join w in CurrentDb.WxUserInfo on u.UserId equals w.UserId

                         where (condition.Name == null || u.AcName.Contains(condition.Name))
                         &&
                         u.Status == Lumos.Entity.Enumeration.WithdrawStatus.Apply

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
        public CustomJsonResult Audit(WithdrawAuditPms model)
        {
            return BizFactory.Withdraw.Audit(this.CurrentUserId, model);
        }


        [HttpPost]
        public CustomJsonResult GetListByDoTransfer(SearchCondition condition)
        {
            var query = (from u in CurrentDb.Withdraw

                         join w in CurrentDb.WxUserInfo on u.UserId equals w.UserId

                         where (condition.Name == null || u.AcName.Contains(condition.Name))
                         &&
                         u.Status == Lumos.Entity.Enumeration.WithdrawStatus.Handing

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
        public CustomJsonResult DoTransfer(WithdrawDoTransferPms model)
        {
            return BizFactory.Withdraw.DoTransfer(this.CurrentUserId, model);
        }
    }
}