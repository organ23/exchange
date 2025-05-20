using ExchangeOffice.Core.Entities;
using ExchangeOffice.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeOffice.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICustomerRepository _customerRepository;

    public IndexModel(
        ILogger<IndexModel> logger,
        ICurrencyRepository currencyRepository,
        ITransactionRepository transactionRepository,
        ICustomerRepository customerRepository)
    {
        _logger = logger;
        _currencyRepository = currencyRepository;
        _transactionRepository = transactionRepository;
        _customerRepository = customerRepository;
    }

    public IEnumerable<Currency> Currencies { get; set; } = new List<Currency>();
    public IEnumerable<Transaction> RecentTransactions { get; set; } = new List<Transaction>();
    public int TodayTransactionsCount { get; set; }
    public int CurrenciesCount { get; set; }
    public int CustomersCount { get; set; }
    public decimal TodayProfit { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // Get currencies
            Currencies = await _currencyRepository.GetAllAsync();
            CurrenciesCount = Currencies.Count();

            // Get today's transactions
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var todayTransactions = await _transactionRepository.GetTransactionsByDateRangeAsync(today, tomorrow);
            TodayTransactionsCount = todayTransactions.Count;

            // Get recent transactions (last 5)
            RecentTransactions = todayTransactions.OrderByDescending(t => t.Date).Take(5);

            // Get today's profit
            TodayProfit = await _transactionRepository.GetTotalProfitForDateRangeAsync(today, tomorrow);

            // Get customers count
            var customers = await _customerRepository.GetAllAsync();
            CustomersCount = customers.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            TodayTransactionsCount = 0;
            CurrenciesCount = 0;
            CustomersCount = 0;
            TodayProfit = 0;
        }
    }
}
