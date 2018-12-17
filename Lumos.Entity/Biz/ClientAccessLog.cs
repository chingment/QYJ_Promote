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
    [Table("ClientAccessLog")]
    public class ClientAccessLog
    {
        [Key]
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string PromoteId { get; set; }
        public string RefereerId { get; set; }
        public string AccessUrl { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
