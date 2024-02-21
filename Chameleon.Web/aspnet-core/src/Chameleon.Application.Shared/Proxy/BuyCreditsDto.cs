using System;
using System.ComponentModel.DataAnnotations;

namespace Chameleon.App.Shared.Proxies
{
    public class BuyCreditsDto
    {
        [Required]
        // Stipe minimum amount is 0.30 pences
        [Range(1, 1000)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(maximumLength: 16, MinimumLength = 16)]
        public string Number { get; set; }

        [Required]
        [Range(1, 12)]
        public long ExpMonth { get; set; }

        [Required]
        [Range(2021, 3000)]
        public long ExpYear { get; set; }

        [Required]
        [StringLength(maximumLength: 3, MinimumLength = 3)]
        public string Cvc { get; set; }
    }
}
