using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Currencies
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ILogger<DeleteModel> _logger;
        private readonly ExchangeOffice.Infrastructure.Data.ApplicationDbContext _context;

        public DeleteModel(
            ICurrencyRepository currencyRepository, 
            ILogger<DeleteModel> logger,
            ExchangeOffice.Infrastructure.Data.ApplicationDbContext context)
        {
            _currencyRepository = currencyRepository;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Currency Currency { get; set; } = default!;
        
        public bool HasTransactions { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var currency = await _currencyRepository.GetByIdAsync(id);
            if (currency == null)
            {
                return NotFound();
            }
            
            Currency = currency;
            
            // Check if there are any transactions using this currency
            HasTransactions = await _context.Transactions
                .AnyAsync(t => t.FromCurrencyId == id || t.ToCurrencyId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Currency.Id == 0)
            {
                return NotFound();
            }

            var currency = await _currencyRepository.GetByIdAsync(Currency.Id);
            if (currency == null)
            {
                return NotFound();
            }
            
            // Check if there are any transactions using this currency
            bool hasTransactions = await _context.Transactions
                .AnyAsync(t => t.FromCurrencyId == Currency.Id || t.ToCurrencyId == Currency.Id);
                
            if (hasTransactions)
            {
                HasTransactions = true;
                Currency = currency;
                ModelState.AddModelError(string.Empty, "Cannot delete currency with associated transactions.");
                return Page();
            }

            try
            {
                // Delete rate histories first
                var rateHistories = await _context.RateHistories
                    .Where(rh => rh.CurrencyId == Currency.Id)
                    .ToListAsync();
                    
                _context.RateHistories.RemoveRange(rateHistories);
                await _context.SaveChangesAsync();
                
                // Then delete the currency
                await _currencyRepository.DeleteAsync(currency);
                
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting currency");
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the currency.");
                Currency = currency;
                HasTransactions = await _context.Transactions
                    .AnyAsync(t => t.FromCurrencyId == Currency.Id || t.ToCurrencyId == Currency.Id);
                return Page();
            }
        }
    }
}
