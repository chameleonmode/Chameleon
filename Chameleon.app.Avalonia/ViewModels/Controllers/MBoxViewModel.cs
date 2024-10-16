using Chameleon.lib.Common.Constants;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class MBoxViewModel : ObservableObject {
	[ObservableProperty]
	private string title = Consts.AppName;
	[ObservableProperty]
	private string glyph = "E946";
}
