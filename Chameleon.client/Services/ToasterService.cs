using Avalonia.Controls.Notifications;
using Avalonia.Controls;
using Avalonia;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Interfaces.Services;

namespace Chameleon.client.Services;
public class ToasterService(IDispatchService dispatcher)
								: IToasterService {
	private int _notificationTimeout = 9;
	private WindowNotificationManager? _notificationManager;
	private readonly IDispatchService _dispatcher = dispatcher;

	public int NotificationTimeout {
		get => _notificationTimeout;
		set {
			_notificationTimeout = (value < 0) ? 0 : value;
		}
	}

	private Notification CreateNotification(string message, NotificationType notificationType, string title = "Chameleon")
	{
		return new Notification(
				title,
				message,
				notificationType,
				TimeSpan.FromSeconds(_notificationTimeout));
	}

	private void ShowOnUI(string message, NotificationType notificationType)
	{
		if (_notificationManager is WindowNotificationManager nm) {
			_dispatcher?.InvokeOnUiThread(() => {
				nm.Show(CreateNotification(message, notificationType));
			});
		} else {
			_dispatcher?.InvokeOnUiThread(() => {
				_notificationManager = new WindowNotificationManager(App.GetMainWindow) {
					Position = NotificationPosition.BottomRight,
					MaxItems = 6,
					Margin = new Thickness(0, 0, 15, 40)
				};
			  ShowOnUI(message, notificationType);
			});
		}
	}

	/// <summary>Set the host window.</summary>
	/// <param name="hostWindow">Parent window.</param>
	public void SetHostWindow(object? hostWindow)
	{
		var notificationManager = new WindowNotificationManager((hostWindow as TopLevel) ?? App.GetMainWindow) {
			Position = NotificationPosition.BottomRight,
			MaxItems = 2,
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

