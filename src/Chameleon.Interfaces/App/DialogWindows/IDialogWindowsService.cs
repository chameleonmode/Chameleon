using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;
using Prism.Services.Dialogs;
using System;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDialogWindowsService : ISingletonDependency
    {
        ButtonResult ShowDialogWindow(IViewControl viewControl, string title);
        ButtonResult ShowDialogWindow<TViewModel>(IViewControl viewControl, string title, Action<TViewModel> initialize)
            where TViewModel : class;
    }
}
