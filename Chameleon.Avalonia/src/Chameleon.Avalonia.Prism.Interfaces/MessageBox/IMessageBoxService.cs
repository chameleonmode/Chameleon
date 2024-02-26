using Chameleon.Interfaces.Ioc;
using Prism.Services.Dialogs;

namespace Chameleon.Avalonia.Prism.Interfaces.MessageBox
{
    public interface IMessageBoxService
        : ISingletonDependency
    {
        void ShowDialog(IMessageBoxOptions messageBoxOptions, Action<ButtonResult> callback);
    }
}
