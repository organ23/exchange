using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExchangeOffice.Core.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string IdNumber { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public ICollection<Transaction> Transactions { get; set; }
    }
}
