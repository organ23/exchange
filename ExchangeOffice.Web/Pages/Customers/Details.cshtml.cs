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
    public class DetailsModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(
            ICustomerRepository customerRepository,
            ITransactionRepository transactionRepository,
            ILogger<DetailsModel> logger)
        {
            _customerRepository = customerRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public Customer Customer { get; set; } = default!;
        public IEnumerable<Transaction> Transactions { get; set; } = new List<Transaction>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            
            Customer = customer;
            
            // Get customer's transactions
            Transactions = await _transactionRepository.GetTransactionsByCustomerAsync(id);

            return Page();
        }
    }
}
