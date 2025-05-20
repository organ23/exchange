using ExchangeOffice.Core.Entities;
using System.Threading.Tasks;

namespace ExchangeOffice.Core.Interfaces
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        Task<Currency> GetByCodeAsync(string code);
        Task AddRateHistoryAsync(RateHistory rateHistory);
        Task<bool> CodeExistsAsync(string code);
    }
}
