using Lumos.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public class SnUtil
    {

        public static string Build(Entity.Enumeration.BizSnType type)
        {

            string prefix = "";
            Random ran = new Random();
            string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ran.Next(1000, 9999);


            string sn = prefix + dateTime;
            return sn;
        }


        public static string BulidOrderNo(Entity.Enumeration.BizSnType type)
        {
            LumosDbContext CurrentDb = new LumosDbContext();

            var bizSn = CurrentDb.BizSn.Where(m => m.Type == type).FirstOrDefault();
            if (bizSn == null)
            {
                Random ran = new Random();
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ran.Next(1000, 9999);

                return "error sn";
            }


            //var orderType = type.ToString();
            //var bizSn = CurrentDb.BizSn.FirstOrDefault(x => x.Type == type);
            //var bizSnStart = DateTime.Now.ToString("yyMMdd-");
            //var bizSnEnd = "1";
            //if (bizSn != null)
            //{
            //    if (bizSn.OrderNo.Split('-')[0] != orderType + flowNoStart)
            //    {
            //        flowNoEnd = (int.Parse(flowNo.OrderNo.Split('-')[1]) + 1).ToString();
            //    }
            //}
            //else
            //{
            //    db.FlowNo.Add(new FlowNo { OrderNo = flowNoStart + flowNoEnd.PadLeft(5, '0'), OrderType = orderType, Remark = "New" });
            //}
            //return orderType + flowNoStart + flowNoEnd.PadLeft(5, '0');

            return "";
        }

    }
}
