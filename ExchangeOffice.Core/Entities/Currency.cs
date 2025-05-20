using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExchangeOffice.Core.Entities
{
    public class Currency
    {
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal BuyRate { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal SellRate { get; set; }

        public DateTime LastUpdated { get; set; }

        // Navigation properties
        public ICollection<RateHistory> RateHistories { get; set; }
        public ICollection<Transaction> FromTransactions { get; set; }
        public ICollection<Transaction> ToTransactions { get; set; }
    }
}
