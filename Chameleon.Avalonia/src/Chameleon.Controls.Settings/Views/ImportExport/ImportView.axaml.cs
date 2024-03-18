using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.ImportExport.Views;
using Chameleon.Interfaces.App.Settings;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class ImportView : SubPageViewControl, IImportView
{
    public ImportView()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Resolve<IImportViewModel>();
    }
}