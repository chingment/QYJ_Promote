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
    [Table("PromoteUser")]
    public class PromoteUser
    {
        [Key]
        public string Id { get; set; }
       
        public string ClientId { get; set; }
        public string RefereerId { get; set; }
        public string PromoteId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string WxPromoteImgMediaId { get; set; }
        public string PromoteImgUrl { get; set; }
        public string CtName { get; set; }
        public string CtPhone { get; set; }
        public bool CtIsStudent { get; set; }
        public string CtSchool { get; set; }
    }
}
