using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class RetPromoteGetConfig
    {
        public RetPromoteGetConfig()
        {
            this.CouponPage = new CouponPageModel();
            this.PayResultPage = new PayResultPageModel();
            this.UserInfo = new UserInfoModel();
            this.UserInfo.CtName = "";
            this.UserInfo.CtPhone = "";
            this.UserInfo.CtIsStudent = "0";
            this.UserInfo.CtSchool = "";
        }

        public string PromoteId { get; set; }
        public string PromoteCouponId { get; set; }
        public string RefereeId { get; set; }
        public string ClientId { get; set; }
        public string ShareTitle { get; set; }
        public string ShareDesc { get; set; }
        public string ShareImgUrl { get; set; }
        public bool IsNeedBuy { get; set; }
        public bool IsBuy { get; set; }
        public bool IsGet { get; set; }
        public int Status { get; set; }
        public string StartTime { get; set; }
        public string NoStartDesc { get; set; }
        public string EndTime { get; set; }
        public string EndDesc { get; set; }
        public bool IsNeedFillInInfo { get; set; }
        public string OrderSn { get; set; }
        public CouponPageModel CouponPage { get; set; }

        public PayResultPageModel PayResultPage { get; set; }

        public UserInfoModel UserInfo { get; set; }

        public class UserInfoModel
        {
            public string CtName { get; set; }
            public string CtPhone { get; set; }
            public string CtIsStudent { get; set; }
            public string CtSchool { get; set; }
        }

        public class CouponPageModel
        {
            public string Title { get; set; }
            public string Bg01 { get; set; }
            public string Bg02 { get; set; }
            public string Bg03 { get; set; }

            public string Bg4GoPersonal { get; set; }
            public string Bg4GoInvite { get; set; }
            public string Bg4GoBuy { get; set; }
        }

        public class PayResultPageModel
        {
            public string Title { get; set; }
            public string Bg01 { get; set; }
            public string Bg02 { get; set; }
            public string Bg03 { get; set; }
            public string Bg04 { get; set; }
            public string Bg4GoPersonal { get; set; }
            public string Bg4GetCoupon { get; set; }
            public string Bg4OpenCoupon { get; set; }
            public string Bg4GoInvite { get; set; }
        }
    }
}
