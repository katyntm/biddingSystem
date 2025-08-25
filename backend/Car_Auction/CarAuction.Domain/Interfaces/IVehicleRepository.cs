using CarAuction.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarAuction.Domain.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle> GetByIdAsync(Guid id);
        Task<Vehicle> GetByVinAsync(string vin);
        Task<IEnumerable<string>> GetAllVinsAsync();
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task AddAsync(Vehicle vehicle);
        Task AddRangeAsync(IEnumerable<Vehicle> vehicles);
        void Update(Vehicle vehicle);
        void Delete(Vehicle vehicle);
    }
}