using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.Auth
{
    public interface IAuthView 
        : IViewControl
        , ISingletonDependency
    {
        IAuthViewModel ViewModel { get; }
    }

    public interface IAuthLoginView  :
     ISingletonDependency
    {
    }

    public interface ITaskDialogView
    {
        T? FindTControl<T>(string name) where T : class;
        Task<object?> ShowTDialogAsync(string name);
    }
    public interface ILoginTaskDialog
        : ITaskDialogView
    {
    }
}
