using Lumos;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Models.Coupon;

namespace WebMobile.Controllers
{
    public class CouponController : OwnBaseController
    {

        public ActionResult My()
        {
            return View();
        }

        [HttpPost]
        public CustomJsonResult GetMy(SearchCondition model)
        {
            var query = (from o in CurrentDb.PromoteUserCoupon
                         where
                         o.UserId == this.CurrentUserId &&
                         o.IsBuy == true
                         select new { o.Id, o.WxCouponId, o.WxCouponDecryptCode, o.IsGet, o.IsConsume, o.CreateTime });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (model.PageIndex)).Take(pageSize);

            var list = query.ToList();

            List<MyCouponModel> oList = new List<MyCouponModel>();

            foreach (var item in list)
            {
                var myTran = new MyCouponModel();
                myTran.Name = "代金券";
                myTran.Discounttip = "";
                myTran.Validdate = "2018-09-08~2019-09-30";
                myTran.WxCouponId = item.WxCouponId;
                myTran.WxCouponDecryptCode = item.WxCouponDecryptCode;
                myTran.Amount = "7200";
                myTran.Discounttip = "报读课程使用";
                myTran.Description = "·具体详情到校区咨询";
                if (!item.IsGet)
                {
                    myTran.Status = 1;
                    myTran.StatusName = "待领取";
                }
                else
                {
                    if (item.IsConsume)
                    {
                        myTran.Status = 3;
                        myTran.StatusName = "已核销";
                    }
                    else
                    {
                        myTran.Status = 2;
                        myTran.StatusName = "待核销";
                    }
                }

                oList.Add(myTran);
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = oList };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }
    }
}