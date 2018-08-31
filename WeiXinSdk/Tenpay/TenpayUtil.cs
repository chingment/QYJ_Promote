using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.WeiXinSdk.Tenpay
{
    public class TenpayUtil
    {
        private TenpayRequest _request = new TenpayRequest();
        private IWxConfig _config = null;

        public IWxConfig Config
        {
            get
            {
                return _config;
            }
        }

        public TenpayUtil(IWxConfig config)
        {
            this._config = config;
        }
        public string GetPrepayId(UnifiedOrder order)
        {

            TenpayUnifiedOrderApi api = new TenpayUnifiedOrderApi(_config, order);

            var result = _request.DoPost(_config, api);

            string prepayId = null;

            result.TryGetValue("prepay_id", out prepayId);

            return prepayId;
        }

        public string OrderQuery(string out_trade_no)
        {
            TenpayOrderQueryApi api = new TenpayOrderQueryApi(_config, out_trade_no);
            var result = _request.DoPost(_config, api);


            return _request.ReturnContent;
        }

        public string OrderPayReFund(string out_trade_no, string out_refund_no, string total_fee, string refund_fee,string refund_desc)
        {
            TenpayOrderPayReFundApi api = new TenpayOrderPayReFundApi(_config, out_trade_no, out_refund_no, total_fee, refund_fee, refund_desc);
            var result = _request.DoPost(_config, api, true);

            return _request.ReturnContent;
        }

        public string OrderRefundQuery(string out_refund_no)
        {
            TenpayOrderRefundQueryApi api = new TenpayOrderRefundQueryApi(_config, out_refund_no);
            var result = _request.DoPost(_config, api, true);

            return _request.ReturnContent;
        }

        public string ReceiveCoupon(string coupon_stock_id, string partner_trade_no, string openid)
        {
            TenpayMmPayMktTransfersSendCouponApi api = new TenpayMmPayMktTransfersSendCouponApi(_config, coupon_stock_id, partner_trade_no, openid);
            var result = _request.DoPost(_config, api, true);

            return _request.ReturnContent;
        }

        public string QueryCouponInfo(string coupon_id, string coupon_stock_id, string openid)
        {
            TenpayCouponQueryInfoApi api=new TenpayCouponQueryInfoApi(_config,coupon_id,coupon_stock_id,openid);
            var result = _request.DoPost(_config, api, true);

            return _request.ReturnContent;
        }
    }
}
