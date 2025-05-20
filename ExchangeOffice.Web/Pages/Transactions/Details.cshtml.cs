using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages.Transactions
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<DetailsModel> _logger;
        private readonly ExchangeOffice.Infrastructure.Data.ApplicationDbContext _context;

        public DetailsModel(
            ITransactionRepository transactionRepository,
            ILogger<DetailsModel> logger,
            ExchangeOffice.Infrastructure.Data.ApplicationDbContext context)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
            _context = context;
        }

        public Transaction Transaction { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            // Get transaction with all related entities
            var transaction = await _context.Transactions
                .Include(t => t.Customer)
                .Include(t => t.FromCurrency)
                .Include(t => t.ToCurrency)
                .Include(t => t.Staff)
                .FirstOrDefaultAsync(t => t.Id == id);
                
            if (transaction == null)
            {
                return NotFound();
            }
            
            Transaction = transaction;
            return Page();
        }
    }
}
