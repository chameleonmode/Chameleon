using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs;

public interface ITopMostSidePanelViewModel : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
    List<IUserProfileActionsViewModel> RunningList { get; set; }
    void Update();
}
