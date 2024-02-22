using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.MessageBox
{
    public interface IMessageBoxService
        : ISingletonDependency
    {
        ButtonResult ShowDialog(IMessageBoxOptions messageBoxOptions);
    }
}
