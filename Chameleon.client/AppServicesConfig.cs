using Chameleon.app.Avalonia;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity;

namespace Chameleon.client;
public static class AppServicesConfig {
	public static void Configure() {
		Navigator.Instance.RegisterView(nameof(IdentityView), typeof(IdentityView));
	}
}
