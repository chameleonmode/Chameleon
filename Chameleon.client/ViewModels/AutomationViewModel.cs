using Chameleon.lib.CommunityToolkit.MvvM;

namespace Chameleon.app.ViewModels;
public class AutomationViewModel : ViewModelObjectBase {
	public int LastSelectedIndex { get; set; } = -1;
}
