using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity;
using Chameleon.app.Avalonia.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders;
public class ProfilesAndFolderModule : IBaseModule {
	public void ConfigureServices(IServiceCollection services) {
		_ = services
			.AddSingleton<View>()
			.AddSingleton<ViewModel>();
	}
}
