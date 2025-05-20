using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using ExchangeOffice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeOffice.Infrastructure.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Include(t => t.FromCurrency)
                .Include(t => t.ToCurrency)
                .Include(t => t.Customer)
                .Include(t => t.Staff)
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Transaction>> GetTransactionsByCurrencyAsync(int currencyId)
        {
            return await _context.Transactions
                .Include(t => t.FromCurrency)
                .Include(t => t.ToCurrency)
                .Include(t => t.Customer)
                .Include(t => t.Staff)
                .Where(t => t.FromCurrencyId == currencyId || t.ToCurrencyId == currencyId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Transaction>> GetTransactionsByCustomerAsync(int customerId)
        {
            return await _context.Transactions
                .Include(t => t.FromCurrency)
                .Include(t => t.ToCurrency)
                .Include(t => t.Staff)
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<string> GenerateTransactionNumberAsync()
        {
            var today = DateTime.Today;
            var transactionsToday = await _context.Transactions
                .CountAsync(t => t.Date.Date == today);

            return $"TX{today:yyyyMMdd}{(transactionsToday + 1):D4}";
        }

        public async Task<decimal> GetTotalProfitForDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var transactions = await _context.Transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();

            decimal totalProfit = 0;
            foreach (var transaction in transactions)
            {
                // Profit from fees
                totalProfit += transaction.Fee;

                // Profit from exchange rate spread
                // This is a simplified calculation - in a real app, you'd need to consider the actual buy/sell rates
                var fromCurrency = await _context.Currencies.FindAsync(transaction.FromCurrencyId);
                var toCurrency = await _context.Currencies.FindAsync(transaction.ToCurrencyId);

                if (fromCurrency != null && toCurrency != null)
                {
                    // Calculate the theoretical "perfect" exchange amount
                    decimal perfectExchangeRate = fromCurrency.SellRate / toCurrency.BuyRate;
                    decimal perfectAmount = transaction.FromAmount * perfectExchangeRate;
                    
                    // The difference between the perfect amount and the actual amount is part of the profit
                    decimal exchangeProfit = perfectAmount - transaction.ToAmount;
                    if (exchangeProfit > 0)
                    {
                        totalProfit += exchangeProfit;
                    }
                }
            }

            return totalProfit;
        }
    }
}
