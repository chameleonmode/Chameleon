using Chameleon.Interfaces;

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
