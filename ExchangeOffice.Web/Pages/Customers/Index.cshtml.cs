using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Customers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ICustomerRepository customerRepository, ILogger<IndexModel> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();

        [BindProperty(SupportsGet = true)]
        public string CurrentFilter { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(CurrentFilter))
            {
                Customers = await _customerRepository.SearchCustomersAsync(CurrentFilter);
            }
            else
            {
                Customers = await _customerRepository.GetAllAsync();
            }
        }
    }
}
