using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces.Repositories;
using CarAuction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarAuction.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly CarAuctionDbContext _context;

        public VehicleRepository(CarAuctionDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle> GetByIdAsync(Guid id)
        {
            return await _context.Set<Vehicle>().FindAsync(id);
        }

        public async Task<Vehicle> GetByVinAsync(string vin)
        {
            return await _context.Set<Vehicle>()
                .FirstOrDefaultAsync(v => v.VIN == vin);
        }

        public async Task<IEnumerable<string>> GetAllVinsAsync()
        {
            return await _context.Set<Vehicle>()
                .Select(v => v.VIN)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Set<Vehicle>().ToListAsync();
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            await _context.Set<Vehicle>().AddAsync(vehicle);
        }

        public async Task AddRangeAsync(IEnumerable<Vehicle> vehicles)
        {
            await _context.Set<Vehicle>().AddRangeAsync(vehicles);
        }

        public void Update(Vehicle vehicle)
        {
            _context.Set<Vehicle>().Update(vehicle);
        }

        public void Delete(Vehicle vehicle)
        {
            _context.Set<Vehicle>().Remove(vehicle);
        }
    }
}