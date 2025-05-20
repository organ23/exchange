using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Reports
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ICurrencyRepository currencyRepository, ILogger<IndexModel> logger)
        {
            _currencyRepository = currencyRepository;
            _logger = logger;
        }

        public IEnumerable<Currency> Currencies { get; set; } = new List<Currency>();

        public async Task OnGetAsync()
        {
            Currencies = await _currencyRepository.GetAllAsync();
        }
    }
}
