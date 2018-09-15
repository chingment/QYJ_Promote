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
                         select new { o.Id, o.PromoteId, o.OrderSn, o.WxCouponId, o.WxCouponDecryptCode, o.IsGet, o.IsConsume, o.CreateTime });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (model.PageIndex)).Take(pageSize);

            var list = query.ToList();

            List<MyCouponModel> oList = new List<MyCouponModel>();

            foreach (var item in list)
            {
                var myCoupon = new MyCouponModel();
                myCoupon.Name = "代金券";
                myCoupon.Discounttip = "";
                myCoupon.Validdate = "2018.09.07-2018.09.30";
                myCoupon.WxCouponId = item.WxCouponId;
                myCoupon.WxCouponDecryptCode = item.WxCouponDecryptCode;
                myCoupon.Amount = "7200";
                myCoupon.Discounttip = "报读课程使用";
                myCoupon.Description = "·具体详情到校区咨询";

                //string promoteId, string orderSn, bool isSuccessed = false

                //1待领取，2打开（待核销），3 已核销
                if (!item.IsGet)
                {
                    myCoupon.Status = 1;
                    myCoupon.StatusName = "待领取";
                    myCoupon.GetMethod = "url";
                    myCoupon.GetUrl = string.Format("/Promote/PayResult?promoteId={0}&orderSn={1}&isSuccessed={2}", item.PromoteId, item.OrderSn, "True");
                }
                else
                {
                    if (item.IsConsume)
                    {
                        myCoupon.Status = 3;
                        myCoupon.StatusName = "已核销";
                    }
                    else
                    {
                        myCoupon.Status = 2;
                        myCoupon.StatusName = "待核销";
                    }
                }

                oList.Add(myCoupon);
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = oList };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }
    }
}