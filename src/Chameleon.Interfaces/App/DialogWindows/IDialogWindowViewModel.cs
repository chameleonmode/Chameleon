using Chameleon.Interfaces.Ioc;


namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDialogWindowViewModel 
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string Title { get; set; }
    }
}
