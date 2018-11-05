using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.App
{
    public class RetProductSkuGetDetails
    {
        public string Id { get; set; }
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public string SalePrice { get; set; }
        public string ShowPrice { get; set; }
        public bool IsHiddenShowPrice { get; set; }
        public string BriefInfo { get; set; }
        public List<string> BriefTag { get; set; }
        public string DisplayImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public int SaleQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int StockQuantity { get; set; }

    }
}
