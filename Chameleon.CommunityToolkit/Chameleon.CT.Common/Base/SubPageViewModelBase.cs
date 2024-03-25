using Chameleon.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Chameleon.CT.Common.Base;

public partial class SubPageViewModelBase : ObservableObjectBase, ISubPageViewModel
{
    public virtual async Task OnNavigatedToAsync(object? param)
    {
        if (!Loaded)
            await InvokeInitializeAsyncCommand(param);
    }
}
