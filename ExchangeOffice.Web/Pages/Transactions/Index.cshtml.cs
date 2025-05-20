using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Transactions
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            ITransactionRepository transactionRepository,
            ICurrencyRepository currencyRepository,
            ILogger<IndexModel> logger)
        {
            _transactionRepository = transactionRepository;
            _currencyRepository = currencyRepository;
            _logger = logger;
        }

        public IEnumerable<Transaction> Transactions { get; set; } = new List<Transaction>();
        public IEnumerable<Currency> Currencies { get; set; } = new List<Currency>();
        
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? CurrencyId { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Load currencies for filter dropdown
                Currencies = await _currencyRepository.GetAllAsync();
                
                // Set default date range if not provided
                if (!FromDate.HasValue)
                {
                    FromDate = DateTime.Today.AddDays(-30);
                }
                
                if (!ToDate.HasValue)
                {
                    ToDate = DateTime.Today.AddDays(1);
                }
                
                // Apply filters
                if (CurrencyId.HasValue)
                {
                    Transactions = await _transactionRepository.GetTransactionsByCurrencyAsync(CurrencyId.Value);
                    
                    // Further filter by date
                    Transactions = Transactions.Where(t => t.Date >= FromDate && t.Date <= ToDate);
                }
                else
                {
                    Transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(FromDate.Value, ToDate.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading transactions");
                ModelState.AddModelError(string.Empty, "An error occurred while loading transactions.");
                Transactions = new List<Transaction>();
            }
        }
    }
}
