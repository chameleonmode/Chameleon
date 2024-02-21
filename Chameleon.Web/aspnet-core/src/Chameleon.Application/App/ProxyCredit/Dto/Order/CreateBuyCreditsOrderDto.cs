using System;
using System.ComponentModel.DataAnnotations;

namespace Chameleon.App
{
    public class CreateBuyCreditsOrderDto
    {
        [Required]
        // Stipe minimum amount is 0.30 pences
        [Range(1, 1000)]
        public decimal Amount { get; set; }
    }
}
