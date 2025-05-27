using Chameleon.lib.CommunityToolkit.MvvM;

namespace Chameleon.client.Features.Settings.Featured;
public class FunctionalSettingsViewModel : ViewModelObjectBase {
	public int LastSelectedIndex { get; set; } = -1;
}
