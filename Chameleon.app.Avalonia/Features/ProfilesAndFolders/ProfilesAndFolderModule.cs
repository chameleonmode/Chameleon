using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Projects;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders;
public static class ProfilesAndFolderModule {
	public static IServiceCollection WithProfilesAndFolders(this IServiceCollection services) => services
		.AddSingleton<IdentityView>()
		.AddSingleton<IdentityViewModel>()
		.AddSingleton<ProjectsView>()
		.AddSingleton<ProjectsViewModel>();
}
