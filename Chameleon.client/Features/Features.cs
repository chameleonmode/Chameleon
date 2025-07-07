using Microsoft.Extensions.DependencyInjection;

using Chameleon.lib.Api.Repos;
using Chameleon.lib.Abs.Platformatic;

using Chameleon.client.Features.Settings.Featured;

namespace Chameleon.client.Features;
	public enum ChangeComparereOption { Ascending, Descending }
public static class Modules {
  public static IServiceCollection Automation(this IServiceCollection services) => services
  .AddSingleton<Automation.View>()
  .AddSingleton<Automation.ViewModel>()
  .AddSingleton<Automation.AI.ChameleonAIR.View>()
  .AddSingleton<Automation.AI.ChameleonAIR.ViewModel>()
  .AddSingleton<Automation.Playwright.PlaywrightView>()
  .AddSingleton<Automation.Playwright.PlaywrightViewModel>()
  .AddSingleton<Automation.Actors.ActorsView>();

  public static IServiceCollection Basic(this IServiceCollection services) => services
  .AddSingleton<Dashboard.View>()
  .AddSingleton<Dashboard.ViewModel>()
  .AddSingleton<Projects.View>()
  .AddSingleton<Settings.View>()
  .AddSingleton<Projects.Profiles.Identity.IdentityView>()
  .AddSingleton<Projects.Profiles.Identity.IdentityViewModel>()
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

  public static IServiceCollection Tenants(this IServiceCollection services) => services
  .AddSingleton<Tenants.ViewModel>()
  .AddSingleton<Tenants.View>()
  .AddSingleton<Tenants.Members.TenantMembersView>()
  .AddSingleton<Tenants.Members.TenantMembersViewModel>();

  public static IServiceCollection All(this IServiceCollection services) => services
  .Basic()
  .Automation()
  .Tenants();

  public static async Task Sync() {
    await DB.I.EnsureUser();
    var tasks = new List<Task>() {
      UserProfilesRepo.Instance.Load(),
      UserProfilesFolderRepo.Instance.Load(),
      TagsRepo.Instance.Load(),
      UPAdditionalDataRepo.Instance.Load(),
    };
    await Task.WhenAll(tasks);
  }
}
