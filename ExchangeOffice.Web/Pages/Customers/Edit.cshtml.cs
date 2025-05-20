using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Customers
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            ICustomerRepository customerRepository,
            ITransactionRepository transactionRepository,
            ILogger<EditModel> logger)
        {
            _customerRepository = customerRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload transactions if validation fails
                Transactions = await _transactionRepository.GetTransactionsByCustomerAsync(Customer.Id);
                return Page();
            }

            try
            {
                await _customerRepository.UpdateAsync(Customer);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the customer.");
                
                // Reload transactions if an error occurs
                Transactions = await _transactionRepository.GetTransactionsByCustomerAsync(Customer.Id);
                return Page();
            }
        }
    }
}
