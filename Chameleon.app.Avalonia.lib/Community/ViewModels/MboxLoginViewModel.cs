using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.lib.Community.ViewModels;
public partial class MboxLoginViewModel : ObservableObject {
	[ObservableProperty]
	private string? licenceKey;

	[ObservableProperty]
	private string? userName;
}
