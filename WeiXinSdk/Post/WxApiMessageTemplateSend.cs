﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.WeiXinSdk
{
    public class WxApiMessageTemplateSend : IWxApiPostRequest<WxApiMessageTemplateSendBaseResult>
    {
        private string access_token { get; set; }

        public WxApiMessageTemplateSend(string access_token, WxPostDataType postdatatpye, object postdata)
        {
            this.access_token = access_token;
            this.PostDataTpye = postdatatpye;
            this.PostData = postdata;
        }

        public WxPostDataType PostDataTpye { get; set; }


        public object PostData { get; set; }


        public string ApiUrl
        {
            get
            {
                return string.Format("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}", this.access_token);
            }
        }

        public IDictionary<string, string> GetUrlParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("access_token", this.access_token);
            return parameters;
        }
    }
}
