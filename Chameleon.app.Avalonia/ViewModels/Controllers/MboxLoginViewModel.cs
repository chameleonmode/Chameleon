using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class MboxLoginViewModel : ObservableObject {
	[ObservableProperty]
	private string? licenceKey;

	[ObservableProperty]
	private string? userName;
}
