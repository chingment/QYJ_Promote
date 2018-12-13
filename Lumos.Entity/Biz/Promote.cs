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
    [Table("Promote")]
    public class Promote
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsNeedBuy { get; set; }
        public Enumeration.PromoteClass Class { get; set; }
        public Enumeration.PromoteTargetType TargetType { get; set; }
    }
}
