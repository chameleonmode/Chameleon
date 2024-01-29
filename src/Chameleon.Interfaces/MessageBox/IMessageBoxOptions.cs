using Prism.Services.Dialogs;
using System.Drawing;
using System.Windows;

namespace Chameleon.Interfaces.MessageBox
{
    public interface IMessageBoxOptions
    {
        string Title { get; set; }
        string Text { get; set; }
        Icon Icon { get; set; }
        MessageBoxButton Buttons { get; set; }
        ButtonResult DefaultButton { get; set; }
        IMessageBoxContentButtonsViewModel ContentButtons { get; set; }
    }
}
