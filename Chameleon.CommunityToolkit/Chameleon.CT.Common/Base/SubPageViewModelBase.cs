using Chameleon.Interfaces;
using System.Threading.Tasks;

namespace Chameleon.CT.Common.Base;

public class SubPageViewModelBase:ObservableObjectBase , ISubPageViewModel
{
    public bool Loaded;
    public virtual Task LoadAsync()
    {
        Loaded = true;
        return Task.CompletedTask;
    }
}
