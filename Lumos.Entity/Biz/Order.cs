﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Sn { get; set; }
        public DateTime? SubmitTime { get; set; }
        public DateTime? PayTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public DateTime? CancledTime { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public Enumeration.OrderStatus Status { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string PromoteId { get; set; }

        public string BroadcastChannelId { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public string WxPrepayId { get; set; }
        public DateTime? PayExpireTime { get; set; }
        public string RefereerId { get; set; }
        public string CancelReason { get; set; }
        public bool BuyProfitIsSettled { get; set; }
        public DateTime? BuyProfitSettledTime { get; set; }

        public bool IsInVisiable { get; set; }

        public string CtName { get; set; }
        public string CtPhone { get; set; }
        public string CtIsStudent { get; set; }
        public string CtSchool { get; set; }
    }
}
