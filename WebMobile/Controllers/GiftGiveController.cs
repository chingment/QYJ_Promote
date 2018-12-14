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
    public class GiftGiveController : OwnBaseController
    {
        // GET: GiftGive
        public ActionResult My()
        {
            return View();
        }

        [HttpPost]
        public CustomJsonResult GetMy(RupGiftGiveGetMy rup)
        {
            var query = (from o in CurrentDb.GiftGive
                         where
                         o.CurrentQuantity > 0 &&
                         o.ClientId == this.CurrentUserId
                         select new { o.Id, o.SkuId, o.CurrentQuantity, o.LockQuantity, o.AvailableQuantity, o.CreateTime });

            query = query.OrderByDescending(r => r.CreateTime);

            var list = query.ToList();

            List<object> oList = new List<object>();

            foreach (var item in list)
            {
                var sku = CurrentDb.ProductSku.Where(m => m.Id == item.SkuId).FirstOrDefault();

                oList.Add(new
                {
                    Id = sku.Id,
                    ImgUrl = ImgSet.GetMain(sku.DisplayImgUrls),
                    Name = sku.Name,
                    Quantity = item.CurrentQuantity,
                    CreateTime = item.CreateTime
                });
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { skus = oList });

        }

        public CustomJsonResult Take()
        {
            return AppServiceFactory.GiftGive.Take(this.CurrentUserId, this.CurrentUserId);
        }
    }
}