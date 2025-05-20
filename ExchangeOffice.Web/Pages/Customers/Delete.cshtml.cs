using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Customers
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<DeleteModel> _logger;
        private readonly ExchangeOffice.Infrastructure.Data.ApplicationDbContext _context;

        public DeleteModel(
            ICustomerRepository customerRepository,
            ILogger<DeleteModel> logger,
            ExchangeOffice.Infrastructure.Data.ApplicationDbContext context)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;
        
        public bool HasTransactions { get; set; }

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
            
            // Check if customer has transactions
            HasTransactions = await _context.Transactions.AnyAsync(t => t.CustomerId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Customer.Id == 0)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync(Customer.Id);
            if (customer == null)
            {
                return NotFound();
            }

            try
            {
                // Check if customer has transactions
                var transactions = await _context.Transactions
                    .Where(t => t.CustomerId == Customer.Id)
                    .ToListAsync();
                
                // Update transactions to remove customer reference
                foreach (var transaction in transactions)
                {
                    transaction.CustomerId = null;
                }
                
                await _context.SaveChangesAsync();
                
                // Delete the customer
                await _customerRepository.DeleteAsync(customer);
                
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the customer.");
                Customer = customer;
                HasTransactions = await _context.Transactions.AnyAsync(t => t.CustomerId == Customer.Id);
                return Page();
            }
        }
    }
}
