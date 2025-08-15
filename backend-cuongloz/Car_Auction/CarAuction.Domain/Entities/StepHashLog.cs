using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarAuction.Domain.Entities
{
    public class StepHashLog
    {
        public Guid Id { get; set; }
        public Guid StepId { get; set; }
        public string MD5Hash { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
