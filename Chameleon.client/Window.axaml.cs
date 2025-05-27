using Avalonia;

using Chameleon.app.Avalonia;
using Chameleon.client.UI.UserControls;

using FluentAvalonia.UI.Windowing;

namespace Chameleon.client;

public partial class MainWindow : AppWindow {
	public MainWindow() {
		InitializeComponent();

#if DEBUG
		this.AttachDevTools();
		this.Topmost = true;
#endif

		SplashScreen = new AppStartup.MainAppSplashScreen(new SplashScreen());
		TitleBar.ExtendsContentIntoTitleBar = true;
		TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
	}
}
