using Chameleon.Interfaces.Ioc;


namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDialogWindowViewModel 
        : ITransientDependency
    {
        string Title { get; set; }
    }
}
