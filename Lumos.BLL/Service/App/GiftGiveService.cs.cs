using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.App
{
    public class GiftGiveService : BaseProvider
    {
        public CustomJsonResult Take(string operater, string clientId)
        {
            var result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var giftGives = CurrentDb.GiftGive.Where(m => m.ClientId == clientId && m.AvailableQuantity > 0).ToList();


                if (giftGives.Count == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, "没有可领取的奖品");
                }

                var giftGiveTake = new GiftGiveTake();
                giftGiveTake.Id = GuidUtil.New();
                giftGiveTake.ClientId = clientId;
                giftGiveTake.TakeTime = this.DateTime;
                giftGiveTake.Quantity = giftGives.Sum(m => m.AvailableQuantity);
                giftGiveTake.CreateTime = this.DateTime;
                giftGiveTake.Creator = operater;
                CurrentDb.GiftGiveTake.Add(giftGiveTake);

                foreach (var giftGive in giftGives)
                {

                    var quantity = giftGive.AvailableQuantity;

                    var giftGiveTakeDetails = new GiftGiveTakeDetails();
                    giftGiveTakeDetails.Id = GuidUtil.New();
                    giftGiveTakeDetails.GiftGiveTakeId = giftGiveTake.Id;
                    giftGiveTakeDetails.ClientId = clientId;
                    giftGiveTakeDetails.SkuId = giftGive.SkuId;
                    giftGiveTakeDetails.Quantity = quantity;
                    giftGiveTakeDetails.TakeTime = this.DateTime;
                    giftGiveTakeDetails.CreateTime = this.DateTime;
                    giftGiveTakeDetails.Creator = operater;
                    CurrentDb.GiftGiveTakeDetails.Add(giftGiveTakeDetails);


                    giftGive.AvailableQuantity = 0;
                    giftGive.CurrentQuantity = 0;
                    giftGive.LockQuantity = 0;
                    giftGive.MendTime = this.DateTime;
                    giftGive.Mender = operater;


                    var giftGiveTrans = new GiftGiveTrans();
                    giftGiveTrans.Id = GuidUtil.New();
                    giftGiveTrans.ClientId = clientId;
                    giftGiveTrans.SkuId = giftGive.SkuId;
                    giftGiveTrans.ChangeQuantity = quantity;
                    giftGiveTrans.LockQuantity = giftGive.LockQuantity;
                    giftGiveTrans.AvailableQuantity = giftGive.AvailableQuantity;
                    giftGiveTrans.CurrentQuantity = giftGive.CurrentQuantity;
                    giftGiveTrans.CreateTime = this.DateTime;
                    giftGiveTrans.Creator = operater;
                    CurrentDb.GiftGiveTrans.Add(giftGiveTrans);

                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "确认领取成功");
            }

            return result;
        }
    }
}
