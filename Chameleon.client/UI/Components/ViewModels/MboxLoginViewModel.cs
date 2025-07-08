using Chameleon.lib.Auth;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.UI.Components.ViewModels;

public partial class MboxLoginViewModel(LoginSettings loginSettings) : ObservableObject {
	[ObservableProperty] string licenceKey = loginSettings.LicenseKey;
	[ObservableProperty] string userName = loginSettings.LoginName;
	[ObservableProperty] bool autoLogin = true;
	public LoginSettings Settings => new(UserName, LicenceKey, AutoLogin);
}
