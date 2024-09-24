using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IErrorContentDialogViewModel
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string Text { get; set; }
    }
}
