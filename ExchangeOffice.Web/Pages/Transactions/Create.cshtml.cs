using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Transactions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            ITransactionRepository transactionRepository,
            ICurrencyRepository currencyRepository,
            ICustomerRepository customerRepository,
            IStaffRepository staffRepository,
            ILogger<CreateModel> logger)
        {
            _transactionRepository = transactionRepository;
            _currencyRepository = currencyRepository;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
            _logger = logger;
        }

        [BindProperty]
        public Transaction Transaction { get; set; } = new Transaction();

        public IEnumerable<Currency> Currencies { get; set; } = new List<Currency>();
        public SelectList CurrencySelectList { get; set; } = new SelectList(Enumerable.Empty<Currency>());
        public SelectList CustomerSelectList { get; set; } = new SelectList(Enumerable.Empty<Customer>());

        public async Task<IActionResult> OnGetAsync(int? customerId = null)
        {
            await LoadFormDataAsync();

            // Set default values
            Transaction.Date = DateTime.Now;
            Transaction.CustomerId = customerId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadFormDataAsync();
                return Page();
            }

            try
            {
                // Get current user ID
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int staffId))
                {
                    Transaction.StaffId = staffId;
                }
                else
                {
                    // Fallback to admin user if authentication is not properly set up
                    var adminStaff = await _staffRepository.FindAsync(s => s.Username == "admin");
                    Transaction.StaffId = adminStaff.FirstOrDefault()?.Id ?? 1;
                }

                // Generate transaction number
                Transaction.TransactionNumber = await _transactionRepository.GenerateTransactionNumberAsync();

                // Add the transaction
                await _transactionRepository.AddAsync(Transaction);

                return RedirectToPage("./Details", new { id = Transaction.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the transaction.");
                await LoadFormDataAsync();
                return Page();
            }
        }

        private async Task LoadFormDataAsync()
        {
            // Load currencies
            Currencies = await _currencyRepository.GetAllAsync();
            CurrencySelectList = new SelectList(Currencies, "Id", "Code");

            // Load customers
            var customers = await _customerRepository.GetAllAsync();
            CustomerSelectList = new SelectList(customers, "Id", "IdNumber", Transaction.CustomerId);

            // Add customer names to the display text
            foreach (var item in CustomerSelectList)
            {
                var customer = customers.FirstOrDefault(c => c.Id.ToString() == item.Value);
                if (customer != null)
                {
                    item.Text = $"{customer.IdNumber} - {customer.FirstName} {customer.LastName}";
                }
            }
        }
    }
}
