using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeOffice.Core.Entities
{
    public class RateHistory
    {
        public int Id { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal BuyRate { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal SellRate { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        // Navigation property
        public Currency Currency { get; set; }
    }
}
