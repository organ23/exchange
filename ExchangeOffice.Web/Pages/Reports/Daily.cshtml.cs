using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Reports
{
    [Authorize]
    public class DailyModel : PageModel
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<DailyModel> _logger;
        private readonly ExchangeOffice.Infrastructure.Data.ApplicationDbContext _context;

        public DailyModel(
            ITransactionRepository transactionRepository,
            ILogger<DailyModel> logger,
            ExchangeOffice.Infrastructure.Data.ApplicationDbContext context)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
            _context = context;
        }

        public IEnumerable<Transaction> Transactions { get; set; } = new List<Transaction>();
        public DateTime ReportDate { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal TotalFees { get; set; }
        public decimal TotalProfit { get; set; }
        
        public class CurrencySummaryItem
        {
            public decimal Bought { get; set; }
            public decimal Sold { get; set; }
        }
        
        public Dictionary<string, CurrencySummaryItem> CurrencySummary { get; set; } = new Dictionary<string, CurrencySummaryItem>();

        public async Task OnGetAsync(DateTime? date = null)
        {
            // Set report date
            ReportDate = date ?? DateTime.Today;
            
            // Get transactions for the day
            var startDate = ReportDate.Date;
            var endDate = startDate.AddDays(1);
            
            try
            {
                // Get transactions with all related entities
                Transactions = await _context.Transactions
                    .Include(t => t.Customer)
                    .Include(t => t.FromCurrency)
                    .Include(t => t.ToCurrency)
                    .Include(t => t.Staff)
                    .Where(t => t.Date >= startDate && t.Date < endDate)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();
                
                // Calculate totals
                TotalFees = Transactions.Sum(t => t.Fee);
                TotalVolume = Transactions.Sum(t => t.FromAmount);
                TotalProfit = await _transactionRepository.GetTotalProfitForDateRangeAsync(startDate, endDate);
                
                // Calculate currency summary
                foreach (var transaction in Transactions)
                {
                    // From currency (we're buying this currency from the customer)
                    if (!CurrencySummary.ContainsKey(transaction.FromCurrency.Code))
                    {
                        CurrencySummary[transaction.FromCurrency.Code] = new CurrencySummaryItem();
                    }
                    CurrencySummary[transaction.FromCurrency.Code].Bought += transaction.FromAmount;
                    
                    // To currency (we're selling this currency to the customer)
                    if (!CurrencySummary.ContainsKey(transaction.ToCurrency.Code))
                    {
                        CurrencySummary[transaction.ToCurrency.Code] = new CurrencySummaryItem();
                    }
                    CurrencySummary[transaction.ToCurrency.Code].Sold += transaction.ToAmount;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading daily report");
                ModelState.AddModelError(string.Empty, "An error occurred while loading the report.");
                Transactions = new List<Transaction>();
            }
        }
    }
}
