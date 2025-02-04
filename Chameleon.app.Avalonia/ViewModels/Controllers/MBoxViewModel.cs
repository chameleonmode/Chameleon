using Chameleon.lib.Common.Constants;
using Chameleon.lib.Const;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class MBoxViewModel : ObservableObject {
	[ObservableProperty]
	private string title = Variables.AppName;
	[ObservableProperty]
	private string glyph = "E946";
}
