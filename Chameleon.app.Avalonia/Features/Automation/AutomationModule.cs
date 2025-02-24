using Chameleon.app.Avalonia.Features.Automation.Playwright;
using Chameleon.app.Avalonia.Interfaces;
using Chameleon.app.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.app.Avalonia.Features.Automation;

public class AutomationModule: IBaseModule {
	public void ConfigureServices(IServiceCollection services) {
		_ = services
			.AddSingleton<PlaywrightView>()
			.AddSingleton<PlaywrightViewModel>();
	}
}