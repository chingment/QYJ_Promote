using Lumos;
using Lumos.BLL.Service.App;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMobile.Models.Promote;

namespace WebMobile.Controllers
{
    public class PromotecController : OwnBaseController
    {
        public ActionResult Coupon()
        {
            return View();
        }

        public ActionResult CouponGet()
        {
            return View();
        }

        [HttpGet]
        public CustomJsonResult GetConfig(int screenHeight)
        {
            LogUtil.Info("rup.ScreenHeight" + screenHeight);
            var uri = new Uri(Request.UrlReferrer.AbsoluteUri);

            var rup = new RupPromoteGetConfig();
            string promoteId = HttpUtility.ParseQueryString(uri.Query).Get("promoteId");
            string refereeId = HttpUtility.ParseQueryString(uri.Query).Get("refereeId");

            rup.PromoteId = promoteId;
            rup.RefereeId = refereeId;
            rup.ScreenHeight = screenHeight;
            return AppServiceFactory.Promote.GetConfig(this.CurrentUserId, this.CurrentUserId, rup);
        }

        [HttpGet]
        [AllowAnonymous]

        public CustomJsonResult UpdateBlackList()
        {
            FileStream fsRead = null;
            try
            {
                string promoteId = "c0c71a0657924059b39895f9e406ef26";

                fsRead = new FileStream(Server.MapPath("~/Files/a1.xls"), FileMode.Open);

                HSSFWorkbook workbook = new HSSFWorkbook(fsRead);

                if (workbook == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel读取失败");
                }

                ISheet sheet = workbook.GetSheetAt(0);

                if (sheet == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel工作本为空");
                }

                if (sheet.LastRowNum <= 1)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel工作本数据为空");
                }

                int rowCount = sheet.LastRowNum + 1;


                for (int i = 1; i < rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row != null)
                    {
                        var cell_phoneNumer = row.GetCell(2);
                        if (cell_phoneNumer != null)
                        {
                            string str_phoneNumer = cell_phoneNumer.ToString().Trim();
                            var promoteUser = CurrentDb.PromoteUser.Where(m => m.CtPhone == str_phoneNumer).FirstOrDefault();
                            if (promoteUser != null)
                            {
                                var promoteBlackList = CurrentDb.PromoteBlackList.Where(m => m.ClientId == promoteUser.ClientId && m.PromoteId == promoteId).FirstOrDefault();
                                if (promoteBlackList == null)
                                {
                                    promoteBlackList = new Lumos.Entity.PromoteBlackList();
                                    promoteBlackList.Id = GuidUtil.New();
                                    promoteBlackList.PromoteId = promoteId;
                                    promoteBlackList.ClientId = promoteUser.ClientId;
                                    promoteBlackList.CreateTime = DateTime.Now;
                                    promoteBlackList.Creator = GuidUtil.Empty();
                                    CurrentDb.PromoteBlackList.Add(promoteBlackList);
                                    CurrentDb.SaveChanges();
                                }
                            }
                        }
                    }
                }

                if (fsRead != null)
                {
                    fsRead.Close();
                    fsRead.Dispose();
                }

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "上传成功");
            }
            catch (Exception ex)
            {
                if (fsRead != null)
                {
                    fsRead.Close();
                    fsRead.Dispose();
                }

                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel上传失败");
            }

        }
    }
}