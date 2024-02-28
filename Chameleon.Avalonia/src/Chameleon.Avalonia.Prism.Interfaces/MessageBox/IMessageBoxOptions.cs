using Avalonia.Controls;
using Chameleon.Interfaces.MessageBox;
using Prism.Services.Dialogs;
using System.Drawing;
using System.Windows;

namespace Chameleon.Avalonia.Prism.Interfaces.MessageBox
{
    public interface IPrismMessageBoxOptions : IMessageBoxOptions
    {
        Window Owner { get; set; }
        ButtonResult DefaultButton { get; set; }
    }
}
