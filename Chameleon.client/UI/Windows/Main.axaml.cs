using Avalonia;
using Chameleon.client.UI.UserControls;
using FluentAvalonia.UI.Windowing;

namespace Chameleon.client.UI.Windows;

public partial class Main : AppWindow {
	public Main() {
		InitializeComponent();

#if DEBUG
		this.AttachDevTools();
		Topmost = true;
#endif
	}
}
