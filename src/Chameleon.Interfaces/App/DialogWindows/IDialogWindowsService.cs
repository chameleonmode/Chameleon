using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

using System;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDialogWindowsService : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
         Task<int> ShowDialogWindow(IViewControl viewControl, string title);
       Task<int> ShowDialogWindow<TViewModel>(IViewControl viewControl, string title, Action<TViewModel> initialize)
            where TViewModel : class;
    }
}
