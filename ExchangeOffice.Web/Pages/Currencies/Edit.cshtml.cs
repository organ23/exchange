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

namespace ExchangeOffice.Web.Pages.Currencies
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ILogger<EditModel> _logger;
        private readonly ExchangeOffice.Infrastructure.Data.ApplicationDbContext _context;

        public EditModel(
            ICurrencyRepository currencyRepository, 
            ILogger<EditModel> logger,
            ExchangeOffice.Infrastructure.Data.ApplicationDbContext context)
        {
            _currencyRepository = currencyRepository;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Currency Currency { get; set; } = default!;

        public IList<RateHistory> RateHistories { get; set; } = new List<RateHistory>();

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
            
            // Get rate history
            RateHistories = await _context.RateHistories
                .Where(rh => rh.CurrencyId == id)
                .OrderByDescending(rh => rh.EffectiveDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload rate history if validation fails
                RateHistories = await _context.RateHistories
                    .Where(rh => rh.CurrencyId == Currency.Id)
                    .OrderByDescending(rh => rh.EffectiveDate)
                    .ToListAsync();
                
                return Page();
            }

            try
            {
                // Get the current currency from the database
                var currentCurrency = await _currencyRepository.GetByIdAsync(Currency.Id);
                if (currentCurrency == null)
                {
                    return NotFound();
                }

                // Check if rates have changed
                bool ratesChanged = currentCurrency.BuyRate != Currency.BuyRate || 
                                   currentCurrency.SellRate != Currency.SellRate;

                // Update the currency
                currentCurrency.Name = Currency.Name;
                currentCurrency.BuyRate = Currency.BuyRate;
                currentCurrency.SellRate = Currency.SellRate;
                currentCurrency.LastUpdated = DateTime.Now;

                await _currencyRepository.UpdateAsync(currentCurrency);

                // Add rate history if rates changed
                if (ratesChanged)
                {
                    var rateHistory = new RateHistory
                    {
                        CurrencyId = Currency.Id,
                        BuyRate = Currency.BuyRate,
                        SellRate = Currency.SellRate,
                        EffectiveDate = DateTime.Now
                    };

                    await _currencyRepository.AddRateHistoryAsync(rateHistory);
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating currency");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the currency.");
                
                // Reload rate history if an error occurs
                RateHistories = await _context.RateHistories
                    .Where(rh => rh.CurrencyId == Currency.Id)
                    .OrderByDescending(rh => rh.EffectiveDate)
                    .ToListAsync();
                
                return Page();
            }
        }
    }
}
