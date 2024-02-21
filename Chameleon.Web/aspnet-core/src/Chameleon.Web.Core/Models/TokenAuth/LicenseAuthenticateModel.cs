using Abp.Authorization.Users;
using Chameleon.App.Licences.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Chameleon.Models.TokenAuth
{
    public class LicenseAuthenticateModel
    {
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [LicenseKey]
        public string LicenseKey { get; set; }
    }
}
