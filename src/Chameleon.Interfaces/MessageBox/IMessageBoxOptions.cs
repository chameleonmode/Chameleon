using Chameleon.Interfaces.MessageBox;
using System.Drawing;
using System.Windows;

namespace Chameleon.Interfaces.MessageBox
{
    public enum MessageBoxButton
    {
        //
        // Summary:
        //     The message box displays an OK button.
        OK = 0,
        //
        // Summary:
        //     The message box displays OK and Cancel buttons.
        OKCancel = 1,
        //
        // Summary:
        //     The message box displays Yes, No, and Cancel buttons.
        YesNoCancel = 3,
        //
        // Summary:
        //     The message box displays Yes and No buttons.
        YesNo = 4
    }

    public interface IMessageBoxOptions
    {
        string Title { get; set; }
        string Text { get; set; }
        Icon Icon { get; set; }
        MessageBoxButton Buttons { get; set; }
        IMessageBoxContentButtonsViewModel ContentButtons { get; set; }
    }

    public class MessageBoxOptions  : IMessageBoxOptions
    {
       public string Title { get; set; }
       public string Text { get; set; }
       public Icon Icon { get; set; }
       public MessageBoxButton Buttons { get; set; }
       public IMessageBoxContentButtonsViewModel ContentButtons { get; set; }
    }
}
