using Chameleon.app.Features.Automation.Playwright;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.app.Features.Automation;

public static class AutomationModule {
	public static IServiceCollection UseAutomation(this IServiceCollection services) => services
			.AddSingleton<PlaywrightView>()
			.AddSingleton<PlaywrightViewModel>();
	
}