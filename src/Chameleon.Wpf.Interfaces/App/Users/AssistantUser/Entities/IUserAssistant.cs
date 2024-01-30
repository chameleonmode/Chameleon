using Chameleon.Interfaces.Entities;
using System.Collections.Generic;

namespace Chameleon.Interfaces.App.Assistants
{
    public interface IUserAssistant 
        : IEntity<long>
    {
        string UserName { get; set; }
        string EmailAddress { get; set; }
        string Password { get; set; }
        bool CanCreateProfiles { get; set; }
        IList<int> ProfileIds { get; set; }
        IList<int> ProfilePermissionIds { get; set; }
        IList<int> FolderIds { get; set; }
        IList<int> FolderPermissionIds{ get; set; }
    }
}
