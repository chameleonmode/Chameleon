using Chameleon.app.Avalonia.Interfaces;
using Microsoft.Extensions.DependencyInjection;

using IdentityView = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.View;
using IdentityViewModel = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModel;

using MyProfilesView = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.View;
using MyProfilesViewModel = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.ViewModel;

using ProjectView = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Projects.View;
using ProjectViewModel = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Projects.ViewModel;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders;
public class ProfilesAndFolderModule : IBaseModule {
	public void ConfigureServices(IServiceCollection services) {
		_ = services
			.AddSingleton<IdentityView>()
			.AddSingleton<IdentityViewModel>()
			.AddSingleton<MyProfilesView>()
			.AddSingleton<MyProfilesViewModel>()
			.AddSingleton<ProjectView>()
			.AddSingleton<ProjectViewModel>();
	}
}
