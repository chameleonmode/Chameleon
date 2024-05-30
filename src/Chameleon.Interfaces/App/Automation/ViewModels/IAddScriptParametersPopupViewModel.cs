using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.ViewModels;
public interface IAddScriptParametersPopupViewModel
    : ITransientDependency
    , IContentDialogViewModel
{
    IAutomationScriptDescription ScriptDescription { get; set; }
}
