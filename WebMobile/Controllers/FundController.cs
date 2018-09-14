using Lumos;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Models.Fund;

namespace WebMobile.Controllers
{
    public class FundController : OwnBaseController
    {
        public ActionResult MyTrans()
        {
            return View();
        }


        [HttpPost]
        public CustomJsonResult GetMyTrans(SearchCondition model)
        {

            var query = (from o in CurrentDb.FundTrans
                         where
                         o.UserId == this.CurrentUserId
                         select new { o.Id, o.Sn, o.ChangeAmount, o.ChangeType, o.CreateTime, o.Description });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (model.PageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> oList = new List<object>();

            foreach (var item in list)
            {
                //var orderList = new OrderList();
                //orderList.Id = item.Id;
                //orderList.Sn = item.Sn;
                //orderList.ChargeAmount = item.ChargeAmount;
                //orderList.Status = (int)item.Status;
                //orderList.StatusName = item.Status.GetCnName();
                //orderList.CancelReason = item.CancledReason;
                //orderList.Description = item.Description;
                //orderList.ActivityId = item.ActivityId;


                oList.Add(new { });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = oList };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }

    }
}