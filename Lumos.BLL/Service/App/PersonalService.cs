using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class PersonalService : BaseProvider
    {
        public CustomJsonResult GetIndexPageData(string pOperater, string pClientId)
        {
            var result = new CustomJsonResult();

            var ret = new RetPersonalGetIndexPageData();

            ret.ServicePhone = "020-82310186";
            var wxUserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientId == pClientId).FirstOrDefault();
            if (wxUserInfo != null)
            {
                ret.Nickname = wxUserInfo.Nickname;
                ret.HeadImgUrl = wxUserInfo.HeadImgUrl;

                var fund = CurrentDb.Fund.Where(m => m.ClientId == pClientId).FirstOrDefault();

                if (fund != null)
                {
                    ret.AvailableBalance = fund.AvailableBalance.ToF2Price();
                }
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }
    }
}
