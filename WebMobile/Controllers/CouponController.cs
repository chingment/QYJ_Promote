using Lumos;
using Lumos.BLL.Service.App;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class CouponController : OwnBaseController
    {

        public ActionResult My()
        {
            return View();
        }

        [HttpPost]
        public CustomJsonResult GetMy(RupCouponGetList rup)
        {
            var query = (from o in CurrentDb.ClientCoupon
                         where
                         o.ClientId == this.CurrentUserId &&
                         o.IsBuy == true
                         select new { o.Id, o.Name, o.Number, o.NumberUnit, o.RefereerId, o.Discounttip, o.Description, o.ValidStartTime, o.ValidEndTime, o.PromoteId, o.OrderSn, o.WxCouponId, o.WxCouponDecryptCode, o.IsGet, o.IsConsume, o.CreateTime });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (rup.PageIndex)).Take(pageSize);

            var list = query.ToList();

            List<MyCouponModel> oList = new List<MyCouponModel>();

            foreach (var item in list)
            {
                var myCoupon = new MyCouponModel();
                myCoupon.Name = item.Name;
                myCoupon.Validdate = item.ValidStartTime.ToUnifiedFormatDate() + "-" + item.ValidEndTime.ToUnifiedFormatDate();
                myCoupon.WxCouponId = item.WxCouponId;
                myCoupon.WxCouponDecryptCode = item.WxCouponDecryptCode;
                myCoupon.Number = item.Number.ToF2Price();
                myCoupon.NumberUnit = item.NumberUnit;
                myCoupon.Discounttip = item.Discounttip;
                myCoupon.Description = item.Description;

                //1待领取，2打开（待核销），3 已核销

                if (item.ValidEndTime.Value < DateTime.Now)
                {
                    if (item.IsConsume)
                    {
                        myCoupon.Status = 3;
                        myCoupon.StatusName = "已使用";
                    }
                    else
                    {
                        myCoupon.Status = 4;
                        myCoupon.StatusName = "已过期";
                    }
                }
                else
                {
                    if (!item.IsGet)
                    {
                        myCoupon.Status = 1;
                        myCoupon.StatusName = "待领取";
                        myCoupon.GetMethod = "url";
                        //myCoupon.GetUrl = string.Format("/Promotec/CouponGet?promoteId={0}&orderSn={1}&isSuccessed={2}", item.PromoteId, item.OrderSn, "True");
                        myCoupon.GetUrl = string.Format("/Promotec/Coupon?promoteId={0}&refereerId={1}", item.PromoteId, item.RefereerId == null ? "" : item.RefereerId);
                    }
                    else
                    {
                        if (item.IsConsume)
                        {
                            myCoupon.Status = 3;
                            myCoupon.StatusName = "已使用";
                        }
                        else
                        {
                            myCoupon.Status = 2;
                            myCoupon.StatusName = "待使用";
                        }
                    }
                }

                oList.Add(myCoupon);
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = oList };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }
    }
}