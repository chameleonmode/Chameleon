using Chameleon.client.Features.Automation.Actors;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.client.MvvM;
using Chameleon.lib.Browzio;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation;
public abstract partial class Automatior : OOVM {
	[ObservableProperty] AvailableBrowser selectedBrowserOption;
	[ObservableProperty] string selectedModel = ActorsViewModel.Models.First();
	
	public IEnumerable<AvailableBrowser> BrowserOptions { get; } = Browzio.Utilities.DetectBrowsers()
		.Where(b => b.Type == BrowserType.Chrome)
		.Select(b => new AvailableBrowser(b));

	public Automatior(string? title = null): base(title) {
		SelectedBrowserOption = BrowserOptions.First();
	}
}
public class ViewModel : OOVM {
  public override Task OnNavigatedTo(object? param) {
    return base.OnNavigatedTo(param);
  } 
}
