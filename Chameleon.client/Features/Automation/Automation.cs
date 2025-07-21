using Chameleon.client.Features.Automation.Actors;
using Chameleon.client.MvvM;
using Chameleon.lib.Browzio;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation;
public abstract partial class Automatior : OOVM {
	[ObservableProperty] BrowserOption selectedBrowserOption;
	[ObservableProperty] string selectedModel = ActorsViewModel.Models.First();

	public Automatior(string? title = null): base(title) {
		SelectedBrowserOption = ActorsViewModel.BrowserOptions.First();
	}
}
public class ViewModel : OOVM {
  public override Task OnNavigatedTo(object? param) {
    return base.OnNavigatedTo(param);
  } 
}
