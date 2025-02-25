using Avalonia;

using Chameleon.app.Avalonia;
using Chameleon.client.Main.UserControls;

using FluentAvalonia.UI.Windowing;

namespace Chameleon.client.Main;

public partial class MainWindow : AppWindow {
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
		this.AttachDevTools();
#endif

		SplashScreen = new AppStartup.MainAppSplashScreen(new SplashScreenUC());
		TitleBar.ExtendsContentIntoTitleBar = true;
		TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
	}
}
