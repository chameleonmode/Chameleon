using Microsoft.Extensions.DependencyInjection;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders;
using Chameleon.client.Features.Automation.AI.ChameleonAIR;
using Chameleon.client.Features.Automation.Playwright;
using Chameleon.client.Features.Tenants.Members;
using Chameleon.client.Pages.ViewModels;
using Chameleon.client.Pages.Views;
using Chameleon.client.Features.Automation.Actors;

namespace Chameleon.client.Pages;
public static class Modules {
  public static IServiceCollection WithActors(this IServiceCollection services) => services
      .AddSingleton<ActorsViewModel>()
      .AddSingleton<ActorsView>()
      ;
  public static IServiceCollection WithAllPagesAndFeatures(this IServiceCollection services) => services
      .WithProfilesAndFolders()
      .WithActors()
      .AddSingleton<DashboardView>()
		  .AddSingleton<DashboardViewModel>()
      .AddSingleton<AutomationViewModel>()
      .AddSingleton<AutomationView>()
			.AddSingleton<ChameleonAIRView>()
			.AddSingleton<ChameleonAIRViewModel>()
			.AddSingleton<PlaywrightView>()
			.AddSingleton<PlaywrightViewModel>()
      .AddSingleton<TenantsViewModel>()
      .AddSingleton<TenantsView>()
      .AddSingleton<TenantMembersView>()
			.AddSingleton<TenantMembersViewModel>();
}
