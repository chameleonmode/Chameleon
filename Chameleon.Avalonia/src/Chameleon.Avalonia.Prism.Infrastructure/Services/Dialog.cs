using System;
using System.ComponentModel;
using Avalonia.Controls;
using Chameleon.Interfaces.Dialogs;
using Prism.Services.Dialogs;

namespace Chameleon.Infrastructure.Dialogs
{
    public class DialogWindow : IPrismDialog
    {
        private readonly WindowBase _window;
        private readonly IDialogWindow _dialogWindow;
        public DialogWindow(IDialogWindow window)
        {
            _dialogWindow = window;
            _window = window as WindowBase;
        }

        public void Close()
        {
            _dialogWindow.Close();
        }

        public void Show()
        {
            _dialogWindow.Show();
        }


        public object Content
        {
            get => _dialogWindow.Content;
            set => _dialogWindow.Content = value;
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

        public event EventHandler Closed
        {
            add => _dialogWindow.Closed += value;
            remove => _dialogWindow.Closed -= value;
        }

        public event EventHandler Opened
        {
            add => _dialogWindow.Opened += value;
            remove => _dialogWindow.Opened -= value;
        }

        public event EventHandler<WindowClosingEventArgs>? Closing
        {
            add => _dialogWindow.Closing += value;
            remove => _dialogWindow.Closing -= value;
        }

        public string Title
        {
            get => (_window as Window).Title;
            set => (_window as Window).Title = value;
        }

        public object GetDialogViewModel()
        {
            return (IDialogAware)_dialogWindow.DataContext;
        }

        public Task ShowDialog(Window owner)
        {
            return _dialogWindow.ShowDialog(owner);
        }

        public WindowBase Owner
        {
            get => _dialogWindow.Owner;
        }

        public void ShowDialog()
        {
            ShowDialog(Owner as Window).Wait();//.ConfigureAwait(false).;
        }

        int IDialog.Result { get => (int)Result.Result; }
    }
}