using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using ExchangeOffice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExchangeOffice.Infrastructure.Repositories
{
    public class CurrencyRepository : Repository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Currency> GetByCodeAsync(string code)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task AddRateHistoryAsync(RateHistory rateHistory)
        {
            await _context.RateHistories.AddAsync(rateHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.Currencies.AnyAsync(c => c.Code == code);
        }
    }
}
