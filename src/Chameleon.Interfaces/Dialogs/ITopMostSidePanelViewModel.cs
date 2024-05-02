using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs;

public interface ITopMostSidePanelViewModel : ISingletonDependency
{
    List<IUserProfileActionsViewModel> RunningList { get; set; }
    void Update();
}
