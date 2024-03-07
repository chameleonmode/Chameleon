using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

using System;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDialogWindowsService : ISingletonDependency
    {
         Task<int> ShowDialogWindow(IViewControl viewControl, string title);
       Task<int> ShowDialogWindow<TViewModel>(IViewControl viewControl, string title, Action<TViewModel> initialize)
            where TViewModel : class;
    }
}
