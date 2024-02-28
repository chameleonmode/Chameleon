using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.MessageBox
{
    public interface IMessageBoxService
        : ISingletonDependency
    {
        int ShowDialog(IMessageBoxOptions messageBoxOptions);
    }
}
