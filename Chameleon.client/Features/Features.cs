using Microsoft.Extensions.DependencyInjection;

using Chameleon.lib.Api.Repos;
using Chameleon.lib.Abs.Platformatic;

using Chameleon.client.Features.Settings.Featured;
using Chameleon.lib.Services;
using Chameleon.client.Services;
using Chameleon.lib.Abs.Repos;
using Chameleon.lib.Api;
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

  public static IServiceCollection Services(this IServiceCollection services) => services
   .AddSingleton<IDispatchService, DispatchService>()
   .AddSingleton<IToasterService, ToasterService>()
   .AddSingleton<IMboxService, MboxService>()
   .AddSingleton<IShowWindowService, ShowWindowService>()
   .AddSingleton<ICopyPastaService, CopyPastaService>();

  public static IServiceCollection All(this IServiceCollection services) => services
   .Services()
   .Basic()
   .Automation()
   .Tenants();

  public static async Task Sync() {
    await DB.I.Userz.Load();
    var tasks = new List<Task>() {
      UserProfilesRepo.Instance.Load(),
      UserProfilesFolderRepo.Instance.Load(),
      TagsRepo.I.Load(),
      UPAdditionalDataRepo.Instance.Load(),
    };
    await Task.WhenAll(tasks);
    if (Auther.AuthSession?.CreatorUserId == null) await TagsRepo.I.CleanStaleTags(
      UserProfilesRepo.Instance.ObservableCache.Keys,
      UserProfilesFolderRepo.Instance.ObservableCache.Keys
    );
  }
}
