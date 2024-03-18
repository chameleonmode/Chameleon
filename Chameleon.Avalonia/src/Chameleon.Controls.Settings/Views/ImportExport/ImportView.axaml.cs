using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.ImportExport.Views;
using Chameleon.Interfaces.App.Settings;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class ImportView : UserControl, IImportView
{
    public ImportView()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Resolve<IImportViewModel>();
    }
}