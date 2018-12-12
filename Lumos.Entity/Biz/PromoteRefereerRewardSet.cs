using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("PromoteRefereerRewardSet")]
    public class PromoteRefereerRewardSet
    {
        [Key]
        public string Id { get; set; }
        public string PromoteId { get; set; }
        public Enumeration.PromoteRefereerRewardSetChannel Channel { get; set; }
        public string Reward { get; set; }
        public int Dept { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }

        public int IncreaseFactor { get; set; }
    }

    public class RewardModel
    {
        public decimal Money { get; set; }

        public List<GiftModel> Gifts { get; set; }

        public class GiftModel
        {
            public string SkuId { get; set; }

            public int Quantity { get; set; }
        }
    }
}
