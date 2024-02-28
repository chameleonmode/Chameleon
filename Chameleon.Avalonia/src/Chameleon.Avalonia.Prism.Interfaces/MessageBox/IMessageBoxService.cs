using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.MessageBox;
using Prism.Services.Dialogs;

namespace Chameleon.Avalonia.Prism.Interfaces.MessageBox
{
    public interface IPrismMessageBoxService
        : ISingletonDependency
    {
        void ShowDialog(IPrismMessageBoxOptions messageBoxOptions, Action<ButtonResult> callback);
    }
}
