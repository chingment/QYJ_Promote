﻿using Lumos;
using Lumos.BLL.Service.App;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMobile.Controllers
{
    public class ShareLogController : OwnBaseController
    {
        public ActionResult My()
        {
            return View();
        }

        [HttpPost]
        public CustomJsonResult GetMy(RupShareLogGetList rup)
        {
            var query = (from o in CurrentDb.ClientCoupon
                         join m in CurrentDb.WxUserInfo on o.ClientId equals m.ClientId

                         where
                         o.RefereerId == this.CurrentUserId &&
                         o.ClientId != o.RefereerId &&
                         o.IsBuy == true
                         select new { o.IsBuy, o.BuyTime, o.IsConsume, o.IsGet, m.Nickname, m.HeadImgUrl });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.BuyTime).Skip(pageSize * (rup.PageIndex)).Take(pageSize);

            var list = query.ToList();

            List<MyShareLogModel> oList = new List<MyShareLogModel>();

            foreach (var item in list)
            {
                var myShareLog = new MyShareLogModel();
                myShareLog.HeadImgUrl = item.HeadImgUrl;
                myShareLog.Nickname = item.Nickname;
                myShareLog.BuyTime = item.BuyTime.ToString("yyyy.MM.dd HH:mm:ss");

                if (item.IsConsume)
                {
                    myShareLog.Remarks = "已核销";
                    myShareLog.RemarksFontColor = "#e8541e";
                }
                else {
                    myShareLog.Remarks = "待核销";
                    myShareLog.RemarksFontColor = "#000000";
                }

                oList.Add(myShareLog);
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = oList };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }
    }
}