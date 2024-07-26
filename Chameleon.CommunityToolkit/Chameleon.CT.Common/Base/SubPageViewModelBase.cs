using Chameleon.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Chameleon.CT.Common.Base;

public partial class SubPageViewModelBase : PageViewModelBase, ISubPageViewModel
{
    public SubPageViewModelBase()
    {
        
    }

    public SubPageViewModelBase(string title)
    {
        Title = title;
    }
}
