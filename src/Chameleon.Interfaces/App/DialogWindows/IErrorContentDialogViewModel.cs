using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IErrorContentDialogViewModel
        : ITransientDependency
    {
        string Text { get; set; }
    }
}
