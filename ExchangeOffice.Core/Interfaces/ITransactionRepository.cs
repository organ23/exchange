using ExchangeOffice.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeOffice.Core.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IReadOnlyList<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IReadOnlyList<Transaction>> GetTransactionsByCurrencyAsync(int currencyId);
        Task<IReadOnlyList<Transaction>> GetTransactionsByCustomerAsync(int customerId);
        Task<string> GenerateTransactionNumberAsync();
        Task<decimal> GetTotalProfitForDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
