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

            List<MyTranModel> oList = new List<MyTranModel>();

            foreach (var item in list)
            {
                var myTran = new MyTranModel();
                myTran.Sn = item.Sn;
                myTran.ChangeAmount = "+" + item.ChangeAmount.ToF2Price();

                switch (item.ChangeType)
                {
                    case Enumeration.FundTransChangeType.ConsumeCoupon:
                        myTran.ChangeType = "分享用户核销优惠券";
                        myTran.Sign = "1";
                        break;
                    case Enumeration.FundTransChangeType.WtihdrawApply:
                        myTran.ChangeType = "提现";
                        myTran.Sign = "2";
                        break;
                    default:
                        myTran.ChangeType = "交易";
                        myTran.Sign = "3";
                        break;
                }

                myTran.TransTime = item.CreateTime.ToUnifiedFormatDateTime();
                myTran.Description = item.Description;

                oList.Add(myTran);
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = oList };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }

    }
}