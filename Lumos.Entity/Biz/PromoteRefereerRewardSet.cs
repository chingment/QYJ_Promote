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
        public Enumeration.PromoteRefereerRewardSetValueType ValueType { get; set; }
        public string Value { get; set; }
        public int Dept { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
