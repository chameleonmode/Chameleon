using Chameleon.Interfaces.Ioc;
using Prism.Services.Dialogs;

namespace Chameleon.Interfaces.MessageBox
{
    public interface IMessageBoxService
        : ISingletonDependency
    {
        ButtonResult ShowDialog(IMessageBoxOptions messageBoxOptions);
    }
}
