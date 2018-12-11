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
            this.Skus = new List<SkuModel>();
            this.CouponPage = new CouponPageModel();
            this.PayResultPage = new PayResultPageModel();
            this.UserInfo = new UserInfoModel();
            this.UserInfo.CtName = "";
            this.UserInfo.CtPhone = "";
            this.UserInfo.CtIsStudent = "0";
            this.UserInfo.CtSchool = "";
        }

        public string PromoteId { get; set; }
        //public string PromoteSkuId { get; set; }
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

        public List<SkuModel> Skus { get; set; }

        public class UserInfoModel
        {
            public string CtName { get; set; }
            public string CtPhone { get; set; }
            public string CtIsStudent { get; set; }
            public string CtSchool { get; set; }
        }

        public class CouponPageModel
        {
            public CouponPageModel()
            {
                this.Bg01 = new ImgModel();
                this.Bg02 = new ImgModel();
                this.Bg03 = new ImgModel();
                this.Bg4GoPersonal = new ImgModel();
                this.Bg4GoInvite = new ImgModel();
                this.Bg4GoBuy = new ImgModel();
            }
            public string Title { get; set; }
            public ImgModel Bg01 { get; set; }
            public ImgModel Bg02 { get; set; }
            public ImgModel Bg03 { get; set; }
            public ImgModel Bg4GoPersonal { get; set; }
            public ImgModel Bg4GoInvite { get; set; }
            public ImgModel Bg4GoBuy { get; set; }
        }

        public class PayResultPageModel
        {
            public PayResultPageModel()
            {
                this.Bg01 = new ImgModel();
                this.Bg02 = new ImgModel();
                this.Bg03 = new ImgModel();
                this.Bg04 = new ImgModel();
                this.Bg4GoPersonal = new ImgModel();
                this.Bg4GoInvite = new ImgModel();
                this.Bg4GetCoupon = new ImgModel();
                this.Bg4OpenCoupon = new ImgModel();
            }

            public string Title { get; set; }
            public ImgModel Bg01 { get; set; }
            public ImgModel Bg02 { get; set; }
            public ImgModel Bg03 { get; set; }
            public ImgModel Bg04 { get; set; }
            public ImgModel Bg4GoPersonal { get; set; }
            public ImgModel Bg4GetCoupon { get; set; }
            public ImgModel Bg4OpenCoupon { get; set; }
            public ImgModel Bg4GoInvite { get; set; }
        }

        public class ImgModel
        {
            public string Src { get; set; }

            public int Height { get; set; }
        }

        public class SkuModel
        {
            public string PromoteSkuId { get; set; }
            public string SkuId { get; set; }
        }
    }
}
