using CarAuction.Domain.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace CarAuction.Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IVehicleRepository Vehicles { get; }
        IVehicleImageRepository VehicleImages { get; }
        
        Task<int> SaveChangesAsync();
    }
}