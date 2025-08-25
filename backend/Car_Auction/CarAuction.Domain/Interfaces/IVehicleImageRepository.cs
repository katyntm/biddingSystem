using CarAuction.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarAuction.Domain.Interfaces
{
    public interface IVehicleImageRepository
    {
        Task<IEnumerable<VehicleImage>> GetByVehicleIdAsync(Guid vehicleId);
        Task AddAsync(VehicleImage image);
        Task AddRangeAsync(IEnumerable<VehicleImage> images);
        void DeleteRange(IEnumerable<VehicleImage> images);
    }
}