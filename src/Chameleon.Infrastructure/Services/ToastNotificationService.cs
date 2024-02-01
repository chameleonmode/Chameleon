using Chameleon.Common.Extensions;
using Chameleon.Interfaces.Dialogs;
using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Chameleon.Infrastructure.Services
{
    public class ToastNotificationService
        : IToastNotificationService
    {
        private readonly MessageOptions _notifierDisplayOptions;
        private readonly Notifier _notifier;

        public ToastNotificationService()
        {
            _notifier = CreateNotifier();
            _notifierDisplayOptions = new MessageOptions
            {
                ShowCloseButton = true,
                UnfreezeOnMouseLeave = true
            };
        }

        private Notifier CreateNotifier()
        {
            return new Notifier(cfg =>
            {
                this.InvokeOnUiThread(() => Configure(cfg));
            });
        }

        private void Configure(NotifierConfiguration cfg)
        {
            cfg.PositionProvider = new WindowPositionProvider(
                     parentWindow: Application.Current.MainWindow,
                     corner: Corner.BottomRight,
                     offsetX: 10,
                     offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(6));

            cfg.Dispatcher = Application.Current.Dispatcher;

            cfg.DisplayOptions.TopMost = false;
            cfg.DisplayOptions.Width = 250;
        }

        public void Dispose()
        {
            _notifier.Dispose();
        }

        public void ShowInformation(string message)
        {
            _notifier.ShowInformation(message, _notifierDisplayOptions);
        }

        public void ShowError(string message)
        {
            _notifier.ShowError(message, _notifierDisplayOptions);
        }

        public void ShowSuccess(string message)
        {
            _notifier.ShowSuccess(message, _notifierDisplayOptions);
        }

        public void ShowWarning(string message)
        {
            _notifier.ShowWarning(message, _notifierDisplayOptions);
        }

        public void ClearAllMessages()
        {
            _notifier.ClearMessages(new ClearAll());
        }
    }
}
