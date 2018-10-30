using Lumos;
using Lumos.BLL;
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
                         o.ClientId == this.CurrentUserId
                         && o.IsNoDisplay == false
                         select new { o.Id, o.Sn, o.ChangeAmount, o.ChangeType, o.CreateTime, o.Description, o.TipsIcon });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (model.PageIndex)).Take(pageSize);

            var list = query.ToList();

            List<MyTranModel> oList = new List<MyTranModel>();

            foreach (var item in list)
            {
                var myTran = new MyTranModel();
                myTran.Sn = item.Sn;

                if (item.ChangeAmount > 0)
                {
                    myTran.ChangeAmount = "+" + item.ChangeAmount.ToF2Price();
                    myTran.ChangeAmountFontColor = "#e8541e";
                }
                else {
                    myTran.ChangeAmount = item.ChangeAmount.ToF2Price();
                    myTran.ChangeAmountFontColor = "#000000";
                }

                switch (item.ChangeType)
                {
                    case Enumeration.FundTransChangeType.ConsumeCoupon:
                    case Enumeration.FundTransChangeType.BuyCoupon:
                        myTran.ChangeType = item.Description;
                        break;
                    case Enumeration.FundTransChangeType.WtihdrawApply:
                    case Enumeration.FundTransChangeType.WtihdrawSuccess:
                    case Enumeration.FundTransChangeType.WtihdrawFailure:
                        myTran.ChangeType = item.Description;
                        break;
                    default:
                        myTran.ChangeType = "交易";
                        break;
                }

                myTran.TransTime = item.CreateTime.ToString("yyyy.MM.dd HH:mm:ss");
                myTran.Description = item.Description;
                myTran.TipsIcon = string.IsNullOrEmpty(item.TipsIcon) == false ? item.TipsIcon : IconUtil.Trans;
                oList.Add(myTran);
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = oList };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }

    }
}