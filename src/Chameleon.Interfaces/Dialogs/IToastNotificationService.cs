using Chameleon.Interfaces.Ioc;
using System;

namespace Chameleon.Interfaces.Dialogs
{
    public interface IToastNotificationService
        : ISingletonDependency
    {
        void SetHostWindow(object? hostWindow);
        void ShowInformation(string message);
        void ShowError(string message);
        void ShowSuccess(string message);
        void ShowWarning(string message);
        void ClearAllMessages();
    }
}