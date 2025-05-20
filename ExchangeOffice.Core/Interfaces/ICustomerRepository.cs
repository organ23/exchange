using ExchangeOffice.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeOffice.Core.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetByIdNumberAsync(string idNumber);
        Task<IReadOnlyList<Customer>> SearchCustomersAsync(string searchTerm);
    }
}
