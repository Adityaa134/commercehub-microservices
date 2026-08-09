using Microsoft.EntityFrameworkCore;
using UsersService.Core.Domain.Entities;
using UsersService.Core.Domain.RepositoryContracts;
using UsersService.Infrastructure.DBContext;

namespace UsersService.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AddressRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Address> AddAddressAsync(Address address)
        {
            if (address.IsDefault)
            {
                await ResetDefaultAddressForUserAsync(address.UserId);
            }

            await _dbContext.Addresses.AddAsync(address);
            await _dbContext.SaveChangesAsync();
            return address;
        }

        public async Task<List<Address>> GetAddressesByUserIdAsync(Guid userId)
        {
            return await _dbContext.Addresses
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.FullName)
                .ToListAsync();
        }

        public async Task<Address?> GetAddressByIdAsync(Guid addressId)
        {
            return await _dbContext.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == addressId);
        }

        public async Task<Address?> GetAddressByIdAndUserIdAsync(Guid addressId, Guid userId)
        {
            return await _dbContext.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        }

        public async Task<Address> UpdateAddressAsync(Address address)
        {
            if (address.IsDefault)
            {
                await ResetDefaultAddressForUserAsync(address.UserId);
            }

            _dbContext.Addresses.Update(address);
            await _dbContext.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAddressAsync(Guid addressId, Guid userId)
        {
            Address? address = await _dbContext.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
                return false;

            _dbContext.Addresses.Remove(address);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task ResetDefaultAddressForUserAsync(Guid userId)
        {
            await _dbContext.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ExecuteUpdateAsync(setters => setters.SetProperty(a => a.IsDefault, false));
        }
    }
}
