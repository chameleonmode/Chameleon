using Chameleon.Interfaces.Ioc;
using System;

namespace Chameleon.Interfaces.Dialogs
{
    public interface IToastNotificationService
        : IDisposable
        , ITransientDependency
    {
        void ShowInformation(string message);
        void ShowError(string message);
        void ShowSuccess(string message);
        void ShowWarning(string message);
        void ClearAllMessages();
    }
}