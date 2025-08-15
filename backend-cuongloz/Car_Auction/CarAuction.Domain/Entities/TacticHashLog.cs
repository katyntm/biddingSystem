using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarAuction.Domain.Entities
{
    public class TacticHashLog
    {
        public Guid Id { get; set; }
        public Guid TacticId { get; set; }
        public string MD5Hash { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
