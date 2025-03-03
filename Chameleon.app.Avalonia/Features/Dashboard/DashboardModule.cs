using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.app.Avalonia.Features.Dashboard;
public static class DashboardModule {
	public static IServiceCollection WithDashboard(this IServiceCollection services) => services
		.AddSingleton<DashboardView>()
		.AddSingleton<DashboardViewModel>();
}
