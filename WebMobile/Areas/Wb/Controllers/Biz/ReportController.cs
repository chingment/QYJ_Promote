﻿using Lumos;
using Lumos.Common;
using Lumos.DAL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMobile.Areas.Wb.Models.Biz.Report;

namespace WebMobile.Areas.Wb.Controllers
{
    public class ReportTable
    {
        public ReportTable()
        {

        }

        public ReportTable(string html)
        {
            this.Html = html;
        }

        public string Html
        {
            get;
            set;
        }
    }

    public class ReportController : WebMobile.Areas.Wb.Own.OwnBaseController
    {
        public ActionResult PromoteCouponBuyRecord(PromoteCouponBuyRecordViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>序号</th>");
            sbTable.Append("<th>昵称</th>");
            sbTable.Append("<th>姓名</th>");
            sbTable.Append("<th>手机</th>");
            sbTable.Append("<th>是否学员</th>");
            sbTable.Append("<th>校区</th>");
            sbTable.Append("<th>支付状态</th>");
            sbTable.Append("<th>支付时间</th>");
            sbTable.Append("<th>领取状态</th>");
            sbTable.Append("<th>领取时间</th>");
            sbTable.Append("<th>核销状态</th>");
            sbTable.Append("<th>核销时间</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"12\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder(" select b.Nickname,c.CtName,c.CtPhone,c.CtIsStudent, CtSchool,  a.IsBuy,a.BuyTime,a.IsGet,a.GetTime,a.IsConsume,a.ConsumeTime from ClientCoupon a left join WxUserInfo  b on a.ClientId=b.ClientId left join PromoteUser c on a.ClientId=c.ClientId  and a.PromoteId=c.PromoteId ");

                sql.Append(" where a.BuyTime is not null ");

                if (model.PromoteId != null)
                {
                    sql.Append(" and  a.PromoteId ='" + model.PromoteId + "'"); ;
                }

                if (model.StartTime != null)
                {
                    sql.Append(" and  a.BuyTime >='" + CommonUtil.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.BuyTime <='" + CommonUtil.ConverToShortDateEnd(model.EndTime.Value) + "'");
                }


                sql.Append(" order by a.BuyTime desc  ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");

                    sbTableContent.Append("<td>" + (r + 1) + "</td>");

                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            case 3:

                                td_value = dtData.Rows[r][c].ToString().Trim();
                                if (td_value == "" || td_value == "0")
                                {
                                    td_value = "否";
                                }
                                else
                                {
                                    td_value = "是";
                                }

                                break;
                            case 5:

                                td_value = dtData.Rows[r][c].ToString().Trim();
                                if (td_value == "True")
                                {
                                    td_value = "已支付";
                                }
                                else
                                {
                                    td_value = "未支付";
                                }

                                break;
                            case 7:

                                td_value = dtData.Rows[r][c].ToString().Trim();
                                if (td_value == "True")
                                {
                                    td_value = "已领取";
                                }
                                else
                                {
                                    td_value = "未领取";
                                }

                                break;
                            case 9:

                                td_value = dtData.Rows[r][c].ToString().Trim();
                                if (td_value == "True")
                                {
                                    td_value = "已核销";
                                }
                                else
                                {
                                    td_value = "未核销";
                                }

                                break;
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "卡卷购买记录报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }

        public ActionResult PromoteShareRecord(PromoteShareRecordViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>序号</th>");
            sbTable.Append("<th>昵称</th>");
            sbTable.Append("<th>分享时间</th>");
            sbTable.Append("<th>分享到</th>");
            sbTable.Append("<th>分享链接</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"5\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder(" select b.Nickname,a.CreateTime,a.[Type],a.ShareLink from ClientShareLog a left join WxUserInfo  b on a.ClientId=b.ClientId ");

                sql.Append(" where 1=1 ");

                if (model.PromoteId != null)
                {
                    sql.Append(" and  a.PromoteId ='" + model.PromoteId + "'"); ;
                }

                if (model.StartTime != null)
                {
                    sql.Append(" and  a.CreateTime >='" + CommonUtil.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.CreateTime <='" + CommonUtil.ConverToShortDateEnd(model.EndTime.Value) + "'");
                }


                sql.Append(" order by a.CreateTime desc  ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");

                    sbTableContent.Append("<td>" + (r + 1) + "</td>");

                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            case 2:

                                td_value = dtData.Rows[r][c].ToString().Trim();
                                if (td_value == "1")
                                {
                                    td_value = "微信好友";
                                }
                                else
                                {
                                    td_value = "微信朋友圈";
                                }

                                break;
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "活动分享记录报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }

        public ActionResult PromoteShareBuyRecord(PromoteShareBuyRecordViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>序号</th>");
            sbTable.Append("<th>分享者昵称</th>");
            sbTable.Append("<th>被分享者昵称</th>");
            sbTable.Append("<th>卡券领取情况</th>");
            sbTable.Append("<th>卡券核销情况</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"5\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder(" select ClientId, Nickname,b.Num from WxUserInfo a inner join (select  RefereerId, count(*) as Num from[dbo].[ClientCoupon]   ");


                if (model.PromoteId != null)
                {
                    sql.Append(" where  PromoteId ='" + model.PromoteId + "'"); ;
                }

                sql.Append(" group by RefereerId) b on a.ClientId = b.RefereerId ");

                sql.Append(" where 1=1 ");



                sql.Append(" order by Num desc  ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    var clientId = dtData.Rows[r]["ClientId"].ToString().Trim();
                    var nickname = dtData.Rows[r]["Nickname"].ToString().Trim();
                    var num = dtData.Rows[r]["Num"].ToString().Trim();


                    string sql2 = "  select b.Nickname,  a.IsBuy,a.BuyTime,a.IsGet,a.GetTime,a.IsConsume,a.ConsumeTime from ClientCoupon a inner join WxUserInfo  b on a.ClientId=b.ClientId    where a.RefereerId='" + clientId + "' ";

                    DataTable dtData2 = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql2).Tables[0].ToStringDataTable();

                    if (num == "1")
                    {
                        sbTableContent.Append("<tr >");
                        sbTableContent.Append("<td>" + (r + 1) + "</td>");
                        sbTableContent.Append("<td>" + nickname + "</td>");
                        sbTableContent.Append("<td>" + dtData2.Rows[0]["Nickname"].ToString().Trim() + "</td>");

                        var isGet2 = dtData2.Rows[0]["IsGet"].ToString().Trim();
                        var isConsume3 = dtData2.Rows[0]["IsConsume"].ToString().Trim();

                        if (isGet2 == "True")
                        {
                            isGet2 = "已领取";
                        }
                        else
                        {
                            isGet2 = "未领取";
                        }

                        if (isConsume3 == "True")
                        {
                            isConsume3 = "已核销";
                        }
                        else
                        {
                            isConsume3 = "未核销";
                        }

                        sbTableContent.Append("<td>" + isGet2 + "</td>");
                        sbTableContent.Append("<td>" + isConsume3 + "</td>");

                        sbTableContent.Append("</tr>");
                    }
                    else
                    {
                        sbTableContent.Append("<tr>");
                        sbTableContent.Append("<td rowspan=\"" + num + "\">" + (r + 1) + "</td>");
                        sbTableContent.Append("<td rowspan=\"" + num + "\">" + nickname + "</td>");

                        for (int a = 0; a < dtData2.Rows.Count; a++)
                        {
                            var nickname2 = dtData2.Rows[a]["Nickname"].ToString().Trim();
                            var isGet2 = dtData2.Rows[a]["IsGet"].ToString().Trim();
                            var isConsume3 = dtData2.Rows[a]["IsConsume"].ToString().Trim();

                            if (isGet2 == "True")
                            {
                                isGet2 = "已领取";
                            }
                            else
                            {
                                isGet2 = "未领取";
                            }

                            if (isConsume3 == "True")
                            {
                                isConsume3 = "已核销";
                            }
                            else
                            {
                                isConsume3 = "未核销";
                            }

                            if (a == 0)
                            {
                                sbTableContent.Append("<td>" + nickname2 + "</td>");
                                sbTableContent.Append("<td>" + isGet2 + "</td>");
                                sbTableContent.Append("<td>" + isConsume3 + "</td>");
                                sbTableContent.Append("</tr>");
                            }
                            else
                            {
                                sbTableContent.Append("<tr>");
                                sbTableContent.Append("<td>" + nickname2 + "</td>");
                                sbTableContent.Append("<td>" + isGet2 + "</td>");
                                sbTableContent.Append("<td>" + isConsume3 + "</td>");
                                sbTableContent.Append("</tr>");
                            }

                        }

                    }
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "活动分享记录报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }

        public ActionResult PromoteSkuBuyRecord(PromoteShareRecordViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>序号</th>");
            sbTable.Append("<th>订单号</th>");
            sbTable.Append("<th>昵称</th>");
            sbTable.Append("<th>姓名</th>");
            sbTable.Append("<th>手机</th>");
            sbTable.Append("<th>是否学员</th>");
            sbTable.Append("<th>校区</th>");
            sbTable.Append("<th>商品名称</th>");
            sbTable.Append("<th>价格</th>");
            sbTable.Append("<th>下单时间</th>");
            sbTable.Append("<th>支付时间</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"11\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder(" select Sn,c.Nickname, a.CtName,a.CtPhone,a.CtIsStudent,a.CtSchool,b.SkuName,b.SalePrice,a.SubmitTime,a.PayTime from [Order] a  left join OrderDetails b on a.Id=b.OrderId  left join WxUserInfo c on a.ClientId=c.ClientId ");

                sql.Append(" where a.[Status]=3 ");

                if (model.PromoteId != null)
                {
                    sql.Append(" and  a.PromoteId ='" + model.PromoteId + "'"); ;
                }

                if (model.StartTime != null)
                {
                    sql.Append(" and  a.PayTime >='" + CommonUtil.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.PayTime <='" + CommonUtil.ConverToShortDateEnd(model.EndTime.Value) + "'");
                }


                sql.Append(" order by a.PayTime desc  ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");

                    sbTableContent.Append("<td>" + (r + 1) + "</td>");

                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            case 4:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                if (td_value == "" || td_value == "0")
                                {
                                    td_value = "否";
                                }
                                else
                                {
                                    td_value = "是";
                                }
                                break;
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "商品购买记录报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }


        public ActionResult WithdrawRecord(WithdrawRecordViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>序号</th>");
            sbTable.Append("<th>流水号</th>");
            sbTable.Append("<th>昵称</th>");
            sbTable.Append("<th>姓名</th>");
            sbTable.Append("<th>身份证号码</th>");
            sbTable.Append("<th>开户行</th>");
            sbTable.Append("<th>银行卡号码</th>");
            sbTable.Append("<th>申请时间</th>");
            sbTable.Append("<th>提现金额</th>");
            sbTable.Append("<th>状态</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"10\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder(" select a.Sn,b.Nickname,a.AcName,a.AcIdNumber, AcBank,AcBankCardNumber,a.ApplyTime,a.Amount,a.[Status] from dbo.Withdraw a left join dbo.WxUserInfo b on a.clientId=b.clientId ");

                sql.Append(" where 1=1 ");

                if (model.Status != 0)
                {
                    sql.Append(" and  a.[status] ='" + model.Status + "'"); ;
                }

                if (model.StartTime != null)
                {
                    sql.Append(" and  a.ApplyTime >='" + CommonUtil.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.ApplyTime <='" + CommonUtil.ConverToShortDateEnd(model.EndTime.Value) + "'");
                }


                sql.Append(" order by a.ApplyTime desc  ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");

                    sbTableContent.Append("<td>" + (r + 1) + "</td>");

                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            case 8:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                switch (td_value)
                                {
                                    case "1":
                                        td_value = "申请中";
                                        break;
                                    case "2":
                                        td_value = "待转账";
                                        break;
                                    case "3":
                                        td_value = "转账成功";
                                        break;
                                    case "4":
                                        td_value = "转账失败";
                                        break;
                                }
                                break;
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "提现记录报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }
    }
}