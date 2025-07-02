using Chameleon.client.MvvM;
using Chameleon.lib.WebBrowser;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation;
public abstract partial class Automatior : ViewModelObjectBase {
	[ObservableProperty] BrowserOption selectedBrowserOption;
	public IEnumerable<BrowserOption> BrowserOptions { get; } = [
		new (SystemBrowserType.Chrome),
		new (SystemBrowserType.Brave),
	];

	public Automatior(string? title = null): base(title) {
		SelectedBrowserOption = BrowserOptions.First();
	}
}
public class ViewModel : ViewModelObjectBase {
  public override Task OnNavigatedTo(object? param) {
    return base.OnNavigatedTo(param);
  } 
}
