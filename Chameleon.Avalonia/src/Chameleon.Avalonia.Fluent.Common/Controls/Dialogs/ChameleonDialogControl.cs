using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia;
using FluentAvalonia.UI.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Chameleon.Av.Fluent.Common.Controls.Dialogs;
using FluentAvalonia.UI.Controls.Primitives;

namespace Chameleon.Av.Fluent.Common.Controls.Dialogs;

public partial class ChameleonDialogControl : HeaderedContentControl
{
    public ChameleonDialogControl()
    {
        _dialogInputs = new List<DialogInputHost>();
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _dialogInputsHost = e.NameScope.Get<ItemsControl>(s_tpCommandsHost);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        List<Control> commands = new(_dialogInputs);
        _dialogInputsHost.ItemsSource = commands;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
    }

    private ItemsControl _dialogInputsHost;

}
