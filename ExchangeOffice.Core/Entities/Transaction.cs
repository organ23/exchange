using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeOffice.Core.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string TransactionNumber { get; set; }

        public int? CustomerId { get; set; }

        [Required]
        public int FromCurrencyId { get; set; }

        [Required]
        public int ToCurrencyId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal FromAmount { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ToAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Fee { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal Rate { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int StaffId { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public Currency FromCurrency { get; set; }
        public Currency ToCurrency { get; set; }
        public Staff Staff { get; set; }
    }
}
