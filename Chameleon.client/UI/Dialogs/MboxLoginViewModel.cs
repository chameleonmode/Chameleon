using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.UI.Dialogs;
public partial class MboxLoginViewModel : ObservableObject {
	[ObservableProperty] string? licenceKey;

	[ObservableProperty] string? userName;

	[ObservableProperty] bool autoLogin;
}
