using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExchangeOffice.Core.Entities
{
    public class Staff
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; }

        public bool IsActive { get; set; }

        // Navigation property
        public ICollection<Transaction> Transactions { get; set; }
    }
}
