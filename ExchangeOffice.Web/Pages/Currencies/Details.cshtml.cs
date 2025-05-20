using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Currencies
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ILogger<DetailsModel> _logger;
        private readonly ExchangeOffice.Infrastructure.Data.ApplicationDbContext _context;

        public DetailsModel(
            ICurrencyRepository currencyRepository, 
            ILogger<DetailsModel> logger,
            ExchangeOffice.Infrastructure.Data.ApplicationDbContext context)
        {
            _currencyRepository = currencyRepository;
            _logger = logger;
            _context = context;
        }

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
    }
}
