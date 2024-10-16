using Chameleon.lib.CommunityToolkit.MvvM;

namespace Chameleon.app.Avalonia.ViewModels;
internal class FunctionalSettingsViewModel : ViewModelObjectBase {
	public int LastSelectedIndex { get; set; } = -1;
}
