using ExchangeOffice.Core.Entities;
using System.Threading.Tasks;

namespace ExchangeOffice.Core.Interfaces
{
    public interface IStaffRepository : IRepository<Staff>
    {
        Task<Staff> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
    }
}
