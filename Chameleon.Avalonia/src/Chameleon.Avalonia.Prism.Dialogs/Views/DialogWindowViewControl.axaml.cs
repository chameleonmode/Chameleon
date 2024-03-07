using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Avalonia.Prism.Dialogs.ViewModels;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.Views;

namespace Chameleon.Avalonia.Prism.Dialogs;

[ViewModel(typeof(DialogWindowViewModel))]
public partial class DialogWindowViewControl : UserControl
        , IDialogWindowView
{
    public DialogWindowViewControl()
    {
        InitializeComponent();
    }

    public object InnerContent
    {
        get => (innerContent.Children[0] as ContentControl).Content;
        set 
        {
            innerContent.Children.Clear();
            innerContent.Children.Add(new ContentControl() { Content = value});
        }
    }

}