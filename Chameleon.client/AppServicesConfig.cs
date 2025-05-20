using Chameleon.app.Avalonia.Services;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.client;
public static class AppServicesConfig {
	public static void Configure() {

		ServiceProvider.RegisterSingletonInstance(UserProfilesRepo.Instance);
		ServiceProvider.RegisterSingletonInstance(UserProfilesFolderRepo.Instance);
		ServiceProvider.RegisterSingleton<IProfileManagementService>(() =>
				new ProfileManagementService(ServiceProvider.GetService<UserProfilesRepo>())
		);
		ServiceProvider.RegisterSingleton<IFolderManagementService>(() =>
				new FolderManagementService(ServiceProvider.GetService<UserProfilesFolderRepo>())
		);

		ServiceProvider.RegisterSingleton<INavigatorService>(() => {
			var navigationService = new NavigatorService();
			navigationService.RegisterView(nameof(IdentityView), typeof(IdentityView));
			return navigationService;
		});
	}
}
