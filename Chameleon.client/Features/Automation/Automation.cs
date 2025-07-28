using System.Collections.ObjectModel;
using Chameleon.client.Features.Automation.Actors;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.client.MvvM;
using Chameleon.lib.Browzio;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;

namespace Chameleon.client.Features.Automation;

public abstract partial class Automatior : OOVM {
	[ObservableProperty] AvailableBrowser selectedBrowserOption;
	[ObservableProperty] string selectedModel = ActorsViewModel.Models.First();

	public ObservableCollection<AvailableBrowser> BrowserOptions { get; } = [];
	public Automatior(string? title = null) : base(title) {
		BrowserOptions.AddRange(
			Browzio.Utilities.DetectBrowsers()
			.Where(b => b.Type == BrowserType.Chrome)
			.Select(b => new AvailableBrowser(b))
		);
		SelectedBrowserOption = BrowserOptions.First();
	}

	public Automatior() : this(null) { }
}
public class ViewModel : OOVM {
  public override Task OnNavigatedTo(object? param) {
    return base.OnNavigatedTo(param);
  } 
}
