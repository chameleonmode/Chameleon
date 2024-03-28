using Chameleon.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Chameleon.CT.Common.Base;

public partial class SubPageViewModelBase : ObservableObjectBase, ISubPageViewModel
{
    public virtual Task OnNavigatedToAsync(object? param)
    {
       // await InvokeInitializeAsyncCommand(param);
       return Task.CompletedTask;
    }
}
