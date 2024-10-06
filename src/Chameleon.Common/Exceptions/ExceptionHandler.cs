using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth.Events;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.ExceptionOptions;
using Chameleon.Interfaces.Logger;
using Chameleon.Interfaces.Services;
using Chameleon.Prism.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Chameleon.Common.Exceptions
{
    public partial class ExceptionHandler
    {
        private const string _youTubeQuotaMessage = "The request cannot be completed because you have exceeded your API quota set by YouTube for this account";

        private static IAppLoggerHelper _appLoggerHelper;
        private static ILogger _logger;
        private static IEventAggregator _eventAggregator;

        public static void Initialize()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                OnUnhandledException(
                    (Exception)e.ExceptionObject,
                    nameof(AppDomain.CurrentDomain.UnhandledException)
                    );

            //TODO: ? Application.Current.DispatcherUnhandledException += (s, e) =>
            //    e.Handled = OnUnhandledException(
            //        e.Exception,
            //        nameof(Application.Current.DispatcherUnhandledException)
            //        );

            TaskScheduler.UnobservedTaskException += (s, e) =>
                {
                    e.SetObserved();
                };
        }

        private static void LogError(Exception ex)
        {
            if (_logger == null)
            {
                return;
            }

            _ = GetException(ex);
            //TODO: _logger.Log.Error(exception.Message, exception);
        }

        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static void SetEventAggregator(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public static void SetAppLoggerHelper(IAppLoggerHelper appLoggerHelper)
        {
            _appLoggerHelper = appLoggerHelper;
        }

        private static bool OnUnhandledException(Exception exeption, string exceptionEventName)
        {
            LogError(exeption);
            ShowException(exeption, exceptionEventName);
            return true;
        }

        public static void ShowException(Exception exception, string message = null)
        {
            Task.Factory.StartNew(() => { LogExceptionAsync(exception, message); })
                 .ContinueWith(val =>
                 {
                     val.Exception.Handle(ex =>
                     {
                         LogExceptionAsync(ex);
                         return true;
                     });
                 }, TaskContinuationOptions.OnlyOnFaulted);

            //ContainerServiceHelper.Resolve<IDispatcherService>().InvokeOnUiThread(() => ShowExceptionWindow(exception));
        }

        public static void LogException(Exception exception, string message = null)
        {
            if (message == null)
            {
                message = exception.Message;
            }
            else
            {
                message += $"\n${exception.Message}";
            }
            _appLoggerHelper?.LogError(message);
        }

        public static Task LogExceptionAsync(Exception exception, string message = null)
        {
            return Task.Run(() =>
            {
                LogException(exception, message);
            });
        }

        private static string GetExceptionMessage(Exception ex)
        {
            return GetException(ex).Message;
        }

        private static Exception GetException(Exception ex)
        {
            var innerException = ex.InnerException;
            if (innerException != null)
            {
                return GetException(innerException);
            }
            return ex;
        }

        private static bool IsWebException(Exception ex)
        {
            if (ex is WebException)
            {
                return true;
            }

            var innerException = ex.InnerException;

            if (innerException != null)
            {
                return IsWebException(innerException);
            }

            return false;
        }

        private static bool IsCurateException(Exception ex)
        {
            if (ex is CurateException)
            {
                return true;
            }

            var innerException = ex.InnerException;

            if (innerException != null)
            {
                return IsCurateException(innerException);
            }

            return false;
        }

        private static bool IsUserFriendlyException(Exception ex)
        {
            if (ex is UserFriendlyException)
            {
                return true;
            }

            var innerException = ex.InnerException;

            if (innerException != null)
            {
                return IsUserFriendlyException(innerException);
            }

            return false;
        }

        private static void ShowExceptionWindow(Exception exception)
        {
            if (IsWebException(exception))
            {
                ShowWebExceptionWindow(exception);

                return;
            }

            if (exception is YouTubeQuotaException)
            {
                ShowYouTubeQuotaExceptionWindow();

                return;
            }

            if (IsCurateException(exception))
            {
                ShowCurateExceptionWindow(exception);

                return;
            }

            var message = GetMessage(exception);

            Action<object, EventArgs>[] buttons =
            [
                CreateCopyButton(message),
                CreateExitButton(),
                CreateContinueButton()
            ];

            ShowWindow(message, buttons);
        }

        private static void ShowWebExceptionWindow(Exception exeption)
        {
            var message = GetWebMessage(exeption);

            Action<object, EventArgs>[] buttons =
            [
                CreateReconnectButton()
            ];

            ShowWindow(message, buttons, "Network error", 600d, 150d);
        }

        private static void ShowYouTubeQuotaExceptionWindow()
        {
            Action<object, EventArgs>[] buttons =
            [
                CreateCopyButton(_youTubeQuotaMessage, "Copy message"),
                CreateContinueButton("OK")
            ];

            ShowWindow(_youTubeQuotaMessage, buttons, "YouTube API Quota exceeded", 620d, 130d);
        }

        private static void ShowCurateExceptionWindow(Exception exception)
        {
            Action<object, EventArgs>[] buttons =
            [
                CreateContinueButton("OK")
            ];
            var message = GetExceptionMessage(exception);
            ShowWindow(message, buttons, "Curate Exception", 620d, 130d);
        }

        private static Action<object, EventArgs> CreateContinueButton(string content = "Continue")
        {
            //var continueButton = new Button
            //{
            //    Content = content,
            //    Margin = new Thickness(3)
            //};
            //continueButton.Click += Continue_Click;

            return Continue_Click;
        }

        private static Action<object, EventArgs> CreateCopyButton(string message, string content = "Copy error")
        {
            //var button = new Button
            //{
            //    Content = content,
            //    Margin = new Thickness(3)
            //};

            //button.Click += ;

            return (e, args) =>
            {
                 //IClipboardService.Instance.SetTextAsync(message);
            };
        }

        private static Action<object, EventArgs> CreateExitButton(string content = "Close application")
        {
            //var button = new Button
            //{
            //    Content = content,
            //    Margin = new Thickness(3)
            //};
            //button.Click += Exit_Click;

            return Exit_Click;
        }

        private static Action<object, EventArgs> CreateReconnectButton(string content = "Reconnect")
        {
            //var button = new Button
            //{
            //    Content = content,
            //    Margin = new Thickness(3)
            //};

            //button.Click += Reconnect_Click;

            return Reconnect_Click;
        }

        private static async void ShowWindow(string message, Action<object,EventArgs>[] buttons, string title = "Unhandled exception", double width = 600d, double height = 400d)
        {
            await ContainerServiceHelper.Resolve<IWindowDialogService>().ShowDialogAsync(buttons);
            //if (_exceptionWindow != null) return;

            //_exceptionWindow = new Window
            //{
            //    Width = width,
            //    Height = height,
            //    WindowStyle = WindowStyle.ToolWindow,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    ShowActivated = true,
            //    Title = title
            //};

            //var grid = new Grid
            //{
            //    Margin = new Thickness(5)
            //};

            //var stackPanel = new StackPanel
            //{
            //    Orientation = Orientation.Horizontal,
            //    HorizontalAlignment = HorizontalAlignment.Center
            //};

            //Grid.SetRow(stackPanel, 1);

            //foreach (var button in buttons)
            //{
            //    stackPanel.Children.Add(button);
            //}

            //grid.RowDefinitions.Add(new RowDefinition
            //{
            //    Height = new GridLength(1, GridUnitType.Star)
            //});
            //grid.RowDefinitions.Add(new RowDefinition
            //{
            //    Height = GridLength.Auto
            //});
            //grid.Children.Add(stackPanel);
            //grid.Children.Add(new TextBox
            //{
            //    Text = message,
            //    Margin = new Thickness(5)
            //});
            //_exceptionWindow.Content = grid;
            //_exceptionWindow.ShowDialog();
            //_exceptionWindow = null;
        }

        private static string GetMessage(Exception exeption)
        {
            var ex = exeption;
            var result = new StringBuilder();
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                result.Append("EntryAssembly: ");
                result.Append(AppContext.BaseDirectory);
                result.AppendLine();
            }
            result.AppendLine("UnhandledException:");
            PackException(ex, result);
            return result.ToString();
        }

        private static string GetWebMessage(Exception exception)
        {
            string message = GetExceptionMessage(exception);
            var result = new StringBuilder();

            result.AppendLine(message);
            result.AppendLine("Make sure the proxy is set up correctly, you're connected to the Internet, and then try again.");

            return result.ToString();
        }

        private static void PackException(Exception ex, StringBuilder stringBuilder, int index = 0)
        {
            AppendLine(stringBuilder, ex.Message, index);
            if (!string.IsNullOrWhiteSpace(ex.StackTrace))
            {
                AppendLine(stringBuilder, "StackTrace:", index);
                AppendLine(stringBuilder, ex.StackTrace, index);
            }
            if (ex.InnerException != null)
            {
                AppendLine(stringBuilder, "InnerException:", index);
                PackException(ex.InnerException, stringBuilder, ++index);
            }
        }

        private static void AppendLine(StringBuilder stringBuilder, string text, int index)
        {
            var tabOffset = new string('\t', index);
            stringBuilder.Append(tabOffset);
            var regex = MyRegexAppendLine();
            stringBuilder.AppendLine(regex.Replace(text, "\r\n" + tabOffset));
        }

        private static void Exit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private static void Continue_Click(object sender, EventArgs e)
        {
            //_exceptionWindow.Hide();
            //_exceptionWindow.Close();
        }

        private static void Reconnect_Click(object sender, EventArgs e)
        {
            _eventAggregator
                .GetEvent<SubmitAsyncEvent>()
                .Publish();

            //_exceptionWindow.Hide();
            //_exceptionWindow.Close();
        }

        private static void ExitApplication()
        {
            Dispose();
            Environment.Exit(1);
        }

        private static void Dispose()
        {
            // TODO:
            //Log.CloseAndFlush();
        }

        [GeneratedRegex("\r\n*\\s")]
        private static partial Regex MyRegexAppendLine();
    }
}
