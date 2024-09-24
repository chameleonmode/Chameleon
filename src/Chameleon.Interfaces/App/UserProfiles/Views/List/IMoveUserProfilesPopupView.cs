using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;
using System.Collections.Generic;

namespace Chameleon.Interfaces.App.UserProfiles.Views.List
{
    public interface IMoveUserProfilesPopupView
        : IViewControl
        , Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        //IList<IUserProfile> ProfileIds { get; set; }
    }
}
