using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;

namespace Chameleon.Avalonia.Common.Services;

public class ToastNotificationService
        : IToastNotificationService
{
    private int _notificationTimeout = 10;
    private WindowNotificationManager _notificationManager;
    private readonly IDispatcherService _dispatcher;

    public ToastNotificationService(IDispatcherService dispatcher)
    {
        _dispatcher = dispatcher;

       // _notificationManager = new WindowNotificationManager(ApplicationHelper.GetMainWindow())
       // {
       //     Position = NotificationPosition.BottomRight,
       //     MaxItems = 4,
       //     Margin = new Thickness(0, 0, 15, 40),
       // };
    }

    public int NotificationTimeout
    {
        get => _notificationTimeout;
        set
        {
            _notificationTimeout = (value < 0) ? 0 : value;
        }
    }

    Notification CreateNotification(string message, NotificationType notificationType, string title = "Chameleon")
    {
       return  new Notification(
           title,
           message,
           notificationType,
           TimeSpan.FromSeconds(_notificationTimeout));
    }
    void ShowOnUI(string message, NotificationType notificationType)
    {
        if (_notificationManager is { } nm)
        {
            _dispatcher.InvokeOnUiThread(() =>
            {
                nm.Show(CreateNotification(message, notificationType));
            });
        }
    }

    /// <summary>Set the host window.</summary>
    /// <param name="hostWindow">Parent window.</param>
    public void SetHostWindow(object? hostWindow)
    {
        var notificationManager = new WindowNotificationManager((hostWindow as TopLevel) ?? ApplicationHelper.GetMainWindow())
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 4,
            Margin = new Thickness(0, 0, 15, 40)
        };

        _notificationManager = notificationManager;
    }
    public void ShowInformation(string message)
    {
        ShowOnUI(message, NotificationType.Information);
    }

    public void ShowError(string message)
    {
        ShowOnUI(message, NotificationType.Error);
    }

    public void ShowSuccess(string message)
    {
        ShowOnUI(message, NotificationType.Success);
    }

    public void ShowWarning(string message)
    {
        ShowOnUI(message, NotificationType.Warning);
    }

    public void ClearAllMessages()
    {
        throw new NotImplementedException();
    }
}
