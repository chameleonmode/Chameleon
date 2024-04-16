using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia;
using FluentAvalonia.UI.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Chameleon.Av.Fluent.Common.Controls.Dialogs;

namespace Chameleon.Av.Fluent.Common.Controls.Dialogs;

[TemplatePart(s_tpCommandsHost, typeof(ItemsControl))]
public partial class ChameleonDialogControl : HeaderedContentControl
{

    /// <summary>
    /// Defines the <see cref="DialogInputs"/> property
    /// </summary>
    public static readonly DirectProperty<ChameleonDialogControl, IList<DialogInputHost>> CommandsProperty =
        AvaloniaProperty.RegisterDirect<ChameleonDialogControl, IList<DialogInputHost>>(nameof(DialogInputs),
            x => x.DialogInputs, (x, v) => x.DialogInputs = v);
    /// <summary>
    /// Gets the list of Commands displayed in the TaskDialog
    /// </summary>
    public IList<DialogInputHost> DialogInputs
    {
        get => _dialogInputs;
        set => SetAndRaise(CommandsProperty, ref _dialogInputs, value);
    }

    public static readonly StyledProperty<string?> TitleDescriptionProperty =
    AvaloniaProperty.Register<ChameleonDialogControl, string?>(nameof(TitleDescription));
    public string? TitleDescription
    {
        get => GetValue(TitleDescriptionProperty);
        set => SetValue(TitleDescriptionProperty, value);
    }

    private const string s_tpCommandsHost = "DialogsHost";   
    private IList<DialogInputHost> _dialogInputs;
}
