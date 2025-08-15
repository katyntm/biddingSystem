using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarAuction.Infrastructure.Mock
{
    public class VehicleImage
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        //FK
        public Vehicle Vehicle { get; set; }
    }
}
