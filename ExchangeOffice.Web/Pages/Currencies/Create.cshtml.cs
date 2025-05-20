using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Currencies
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ICurrencyRepository currencyRepository, ILogger<CreateModel> logger)
        {
            _currencyRepository = currencyRepository;
            _logger = logger;
        }

        [BindProperty]
        public Currency Currency { get; set; } = new Currency();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Check if currency code already exists
                if (await _currencyRepository.CodeExistsAsync(Currency.Code))
                {
                    ModelState.AddModelError("Currency.Code", "Currency code already exists.");
                    return Page();
                }

                // Set the last updated date
                Currency.LastUpdated = DateTime.Now;

                // Add the currency
                await _currencyRepository.AddAsync(Currency);

                // Add rate history
                var rateHistory = new RateHistory
                {
                    CurrencyId = Currency.Id,
                    BuyRate = Currency.BuyRate,
                    SellRate = Currency.SellRate,
                    EffectiveDate = DateTime.Now
                };

                await _currencyRepository.AddRateHistoryAsync(rateHistory);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating currency");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the currency.");
                return Page();
            }
        }
    }
}
