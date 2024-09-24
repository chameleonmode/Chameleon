using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System.Collections.Generic;

namespace Chameleon.Interfaces.App.UserProfiles
{
    public interface IMoveUserProfilesPopupViewModel
     : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency, IContentDialogViewModel
    {
        IList<IUserProfile> Profiles { get; set; } 
    }
}
