using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;

namespace Chameleon.App.Users.AssistantUser.Dto
{
    public class CreateUserAssistantDto 
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public IList<int> ProfileIds { get; set; }
        public IList<int> ProfilePermissionIds { get; set; }
        public IList<int> FolderIds { get; set; }
        public IList<int> FolderPermissionIds { get; set; }
    }
}
