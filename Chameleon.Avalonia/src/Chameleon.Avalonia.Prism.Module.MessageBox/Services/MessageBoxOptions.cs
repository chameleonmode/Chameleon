using Avalonia.Controls;
using Chameleon.Avalonia.Prism.Interfaces.MessageBox;
using Chameleon.Avalonia.Prism.Module.MessageBox.ViewModels;
using Chameleon.Interfaces.MessageBox;
using Prism.Services.Dialogs;
using System.Drawing;
using System.Windows;

namespace Chameleon.Avalonia.Prism.Module.MessageBox.Services
{
    public class MessageBoxOptions
        : IMessageBoxOptions
    {
        public MessageBoxOptions()
        {

        }

        public MessageBoxOptions(string title, string text, Icon icon)
        {
            Title = title;
            Text = text;
            Icon = icon;
        }

        public string Title { get; set; }// = //Avalonia. Application.Current?.MainWindow?.Title;
        public string Text { get; set; }
        public Icon Icon { get; set; } = SystemIcons.Information;
        public MessageBoxButton Buttons { get; set; } = MessageBoxButton.OK;
        public ButtonResult DefaultButton { get; set; }
        public IMessageBoxContentButtonsViewModel ContentButtons { get; set; } = new MessageBoxContentButtonsViewModel();
        public Window Owner
        {
            get; set;
        }
    }
}
