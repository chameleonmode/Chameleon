using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.Automation.ViewModels;
public interface ISelectAutomationPopupViewModel
    : ITransientDependency
    , IContentDialogViewModel
{
    IList<IUserProfile> UserProfiles { get; set; }
}
