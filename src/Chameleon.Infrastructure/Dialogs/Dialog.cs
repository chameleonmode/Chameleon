using System;
using System.ComponentModel;
using System.Windows;
using Chameleon.Interfaces.Dialogs;
using Prism.Services.Dialogs;

namespace Chameleon.Infrastructure.Dialogs
{
    public class DialogWindow : IDialog
    {
        private readonly Window _window;
        private readonly IDialogWindow _dialogWindow;
        public DialogWindow(IDialogWindow window)
        {
            _dialogWindow = window;
            _window = window as Window;
        }

        public void Close()
        {
            _dialogWindow.Close();
        }

        public void Show()
        {
            _dialogWindow.Show();
        }

        public bool? ShowDialog()
        {
            return _dialogWindow.ShowDialog();
        }

        public object Content
        {
            get => _dialogWindow.Content;
            set => _dialogWindow.Content = value;
        }

        public Window Owner
        {
            get => _dialogWindow.Owner;
            set => _dialogWindow.Owner = value;
        }

        public object DataContext
        {
            get => _dialogWindow.DataContext;
            set => _dialogWindow.DataContext = value;
        }

        public IDialogResult Result
        {
            get => _dialogWindow.Result;
            set => _dialogWindow.Result = value;
        }

        public Style Style
        {
            get => _dialogWindow.Style;
            set => _dialogWindow.Style = value;
        }

        public event RoutedEventHandler Loaded
        {
            add => _dialogWindow.Loaded += value;
            remove => _dialogWindow.Loaded -= value;
        }

        public event EventHandler Closed
        {
            add => _dialogWindow.Closed += value;
            remove => _dialogWindow.Closed -= value;
        }

        public event CancelEventHandler Closing
        {
            add => _dialogWindow.Closing += value;
            remove => _dialogWindow.Closing -= value;
        }

        public string Title
        {
            get => _window.Title;
            set => _window.Title = value;
        }
    }
}