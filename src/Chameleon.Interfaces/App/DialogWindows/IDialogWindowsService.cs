using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

using System;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDialogWindowsService : ISingletonDependency
    {
        int ShowDialogWindow(IViewControl viewControl, string title);
        int ShowDialogWindow<TViewModel>(IViewControl viewControl, string title, Action<TViewModel> initialize)
            where TViewModel : class;
    }
}
