using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Prism.Services.Dialogs;

namespace Chameleon.Avalonia.Prism.Module.MessageBox;

public partial class DialogWindowsWindow : Window, IDialogWindow
{
    public DialogWindowsWindow()
    {
        InitializeComponent();
    }

    public IDialogResult? Result { get; set; }

    object? IDialogWindow.Content { get => null; set { if (value != null) Dock.Children[1] = (Control)value; } }
}