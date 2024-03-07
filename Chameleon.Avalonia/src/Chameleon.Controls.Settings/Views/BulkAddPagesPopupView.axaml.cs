using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Avalonia.Controls.Settings;

[ViewModel(typeof(BulkAddPagesPopupViewModel))]
public partial class BulkAddPagesPopupView : UserControl, IBulkAddPagesPopupView
{
    public BulkAddPagesPopupView()
    {
        InitializeComponent();
    }

    public string Urls { get => (DataContext as BulkAddPagesPopupViewModel)?.Urls; set => (DataContext as BulkAddPagesPopupViewModel).Urls = value; }
}