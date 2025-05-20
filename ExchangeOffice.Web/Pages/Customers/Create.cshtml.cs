using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Customers
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ICustomerRepository customerRepository, ILogger<CreateModel> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

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
                // Check if customer with same ID number already exists
                var existingCustomer = await _customerRepository.GetByIdNumberAsync(Customer.IdNumber);
                if (existingCustomer != null)
                {
                    ModelState.AddModelError("Customer.IdNumber", "A customer with this ID number already exists.");
                    return Page();
                }

                // Set creation date
                Customer.CreatedAt = DateTime.Now;

                // Add the customer
                await _customerRepository.AddAsync(Customer);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the customer.");
                return Page();
            }
        }
    }
}
