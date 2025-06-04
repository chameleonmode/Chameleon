using Microsoft.Extensions.DependencyInjection;

using Chameleon.lib.Api.Repos;
using Chameleon.lib.Abs.Platformatic;

using Chameleon.client.Features.Projects.Profiles.Identity;
using Chameleon.client.Features.Settings.Featured;

namespace Chameleon.client.Features;

public static class Modules {
  public static IServiceCollection Automation(this IServiceCollection services) => services
  .AddSingleton<Automation.View>()
  .AddSingleton<Automation.ViewModel>()
  .AddSingleton<Automation.AI.ChameleonAIR.View>()
  .AddSingleton<Automation.AI.ChameleonAIR.ViewModel>()
  .AddSingleton<Automation.Playwright.PlaywrightView>()
  .AddSingleton<Automation.Playwright.PlaywrightViewModel>()
  .AddSingleton<Automation.Actors.ActorsViewModel>()
  .AddSingleton<Automation.Actors.ActorsView>();

  public static IServiceCollection WithProfilesAndFolders(this IServiceCollection services) => services
  .AddSingleton<IdentityView>()
  .AddSingleton<IdentityViewModel>()
  .AddSingleton<Projects.ProjectsView>()
  .AddSingleton<Projects.ProjectsViewModel>();

  public static IServiceCollection WithAllFeatures(this IServiceCollection services) => services
  .Automation()
  .WithProfilesAndFolders()
  .AddSingleton<Dashboard.View>()
  .AddSingleton<Dashboard.ViewModel>()
  .AddSingleton<Tenants.ViewModel>()
  .AddSingleton<Tenants.View>()
  .AddSingleton<Tenants.Members.TenantMembersView>()
  .AddSingleton<Tenants.Members.TenantMembersViewModel>()
  .AddSingleton<Settings.View>()
  .AddSingleton<Settings.ViewModel>()
  //FunctionalSettings
  .AddSingleton<FunctionalSettingsView>()
  .AddSingleton<UserProxySettingsView>()
  .AddSingleton<UserDefaultSettingsView>()
  .AddSingleton<PhoneVerificationView>()
  .AddSingleton<ProxyCreditView>()
  .AddSingleton<FunctionalSettingsViewModel>()
  .AddSingleton<UserProxySettingsViewModel>()
  .AddSingleton<UserDefaultSettingsViewModel>()
  .AddSingleton<PhoneVerificationViewModel>()
  .AddSingleton<ProxyCreditViewModel>();

  public static async Task Sync() {
    await DB.Instance.EnsureUser();
    var tasks = new List<Task>() {
      UserProfilesRepo.Instance.Load(),
      UserProfilesFolderRepo.Instance.Load(),
      TagsRepo.Instance.Load(),
      UPAdditionalDataRepo.Instance.Load(),
    };
    await Task.WhenAll(tasks);
  }
}
