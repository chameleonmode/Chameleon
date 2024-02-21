using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace Chameleon.App
{
    public class GenerateLicenseDto
    {
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}
