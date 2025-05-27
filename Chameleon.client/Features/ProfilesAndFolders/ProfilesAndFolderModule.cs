using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity;
using Chameleon.client.Features.ProfilesAndFolders.Projects;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.client.Features.ProfilesAndFolders;
public static class ProfilesAndFolderModule {
	public static IServiceCollection WithProfilesAndFolders(this IServiceCollection services) => services
		.AddSingleton<IdentityView>()
		.AddSingleton<IdentityViewModel>()
		.AddSingleton<ProjectsView>()
		.AddSingleton<ProjectsViewModel>();
}
