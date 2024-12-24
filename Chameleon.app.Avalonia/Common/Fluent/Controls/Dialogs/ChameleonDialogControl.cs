using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Chameleon.Av.Fluent.Common.Controls.Dialogs;

public partial class ChameleonDialogControl : HeaderedContentControl {
	private ItemsControl? _dialogInputsHost;
	public ChameleonDialogControl()
	{
		_dialogInputs = [];
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
		if (_dialogInputsHost != null)
			_dialogInputsHost.ItemsSource = commands;
	}
}
