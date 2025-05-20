using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using ExchangeOffice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeOffice.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer> GetByIdNumberAsync(string idNumber)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.IdNumber == idNumber);
        }

        public async Task<IReadOnlyList<Customer>> SearchCustomersAsync(string searchTerm)
        {
            return await _context.Customers
                .Where(c => c.FirstName.Contains(searchTerm) || 
                            c.LastName.Contains(searchTerm) || 
                            c.IdNumber.Contains(searchTerm) ||
                            c.PhoneNumber.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
