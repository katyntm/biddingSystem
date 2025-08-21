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
    public class VehicleImageRepository : IVehicleImageRepository
    {
        private readonly CarAuctionDbContext _context;

        public VehicleImageRepository(CarAuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleImage>> GetByVehicleIdAsync(Guid vehicleId)
        {
            return await _context.Set<VehicleImage>()
                .Where(i => i.VehicleId == vehicleId)
                .ToListAsync();
        }

        public async Task AddAsync(VehicleImage image)
        {
            await _context.Set<VehicleImage>().AddAsync(image);
        }

        public async Task AddRangeAsync(IEnumerable<VehicleImage> images)
        {
            await _context.Set<VehicleImage>().AddRangeAsync(images);
        }

        public void DeleteRange(IEnumerable<VehicleImage> images)
        {
            _context.Set<VehicleImage>().RemoveRange(images);
        }
    }
}