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
using WebMobile.Areas.Wb.Models.Report;

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
            sbTable.Append("<th>校区</th>");
            sbTable.Append("<th>支付时间</th>");
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
                StringBuilder sql = new StringBuilder(" b.Nickname,c.CtName,c.CtPhone,c.CtIsStudent, CtSchool,  a.IsBuy,a.BuyTime,a.IsGet,a.GetTime,a.IsConsume,a.ConsumeTime from PromoteUserCoupon a left join WxUserInfo  b on a.UserId=b.UserId left join PromoteUser c on a.userId=c.UserId ");

                sql.Append(" where a.BuyTime is not null ");


                if (model.StartTime != null)
                {
                    sql.Append(" and  a.BuyTime >='" + CommonUtils.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.BuyTime <='" + CommonUtils.ConverToShortDateEnd(model.EndTime.Value) + "'");
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
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "商品SKU信息报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }


        public ActionResult PromoteCouponConsumeRecord(PromoteCouponBuyRecordViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>序号</th>");
            sbTable.Append("<th>昵称</th>");
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
                sbTable.Replace("{content}", "<tr><td colspan=\"8\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder("select b.Nickname,a.IsBuy,a.BuyTime,a.IsGet,a.GetTime,a.IsConsume,a.ConsumeTime from PromoteUserCoupon a left join WxUserInfo  b on a.UserId=b.UserId ");

                sql.Append(" where a.BuyTime is not null ");


                if (model.StartTime != null)
                {
                    sql.Append(" and  a.BuyTime >='" + CommonUtils.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.BuyTime <='" + CommonUtils.ConverToShortDateEnd(model.EndTime.Value) + "'");
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
                            case 1:

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
                            case 3:

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
                            case 5:

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
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "商品SKU信息报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }

        public ActionResult PromoteShareRecord(PromoteCouponBuyRecordViewModel model)
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
                StringBuilder sql = new StringBuilder(" select b.Nickname,a.CreateTime,a.[Type],a.ShareLink from PromoteShareLog a left join WxUserInfo  b on a.UserId=b.UserId ");

                sql.Append(" where 1=1 ");


                if (model.StartTime != null)
                {
                    sql.Append(" and  a.CreateTime >='" + CommonUtils.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.CreateTime <='" + CommonUtils.ConverToShortDateEnd(model.EndTime.Value) + "'");
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
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "商品SKU信息报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }
    }
}