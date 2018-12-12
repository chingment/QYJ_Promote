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
    [Table("PromoteRefereerRewardFactor")]
    public class PromoteRefereerRewardFactor
    {
        [Key]
        public string Id { get; set; }

        public string PromoteRefereerRewardSetId { get; set; }
        public string PromoteId { get; set; }
        public string RefereerId { get; set; }
        public int Factor { get; set; }
    }
}
