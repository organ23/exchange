using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using ExchangeOffice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExchangeOffice.Infrastructure.Repositories
{
    public class StaffRepository : Repository<Staff>, IStaffRepository
    {
        public StaffRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Staff> GetByUsernameAsync(string username)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.Username == username);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Staff.AnyAsync(s => s.Username == username);
        }
    }
}
