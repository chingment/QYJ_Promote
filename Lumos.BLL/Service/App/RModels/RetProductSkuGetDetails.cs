using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class RetProductSkuGetDetails
    {
        public RetProductSkuGetDetails()
        {
            this.BriefTags = new List<string>();
            this.BuyBtn = new Button();
        }

        public string Id { get; set; }
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ShowPrice { get; set; }
        public bool ShowPriceIsInVisiable { get; set; }
        public string BriefInfo { get; set; }
        public List<string> BriefTags { get; set; }

        [JsonConverter(typeof(JsonObjectConvert))]
        public string DisplayImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public int SaleQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int StockQuantity { get; set; }
        public bool IsFlashSale { get; set; }
        public int FlashSaleStSecond { get; set; }
        public int FlashSaleEnSecond { get; set; }

        public string FlashSaleNextTips { get; set; }
        public bool IsCanBuy { get; set; }
        public Button BuyBtn { get; set; }

        public string OrderId { get; set; }
        public class Button
        {
            public string Text { get; set; }

            public bool Enabled { get; set; }
        }

    }
}
