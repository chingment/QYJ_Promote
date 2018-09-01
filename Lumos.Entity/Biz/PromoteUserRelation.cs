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
    [Table("PromoteUserRelation")]
    public class PromoteUserRelation
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CUserId { get; set; }
        public string PromoteId { get; set; }
        public int Dept { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
