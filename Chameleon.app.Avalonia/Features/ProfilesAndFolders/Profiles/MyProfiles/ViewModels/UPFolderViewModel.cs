using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using System.Reactive.Linq;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.ViewModels;
public partial class UPFolderViewModel : ObservableObjectBase {

	public UPFolderViewModel() {

		_ = this.WhenValueChanged(x => x.Tags)
			.Throttle(TimeSpan.FromSeconds(1))
			.Subscribe(x => TagsChanged?.Invoke(this, x));

	}

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	public string? title;

	[ObservableProperty]
	public bool isFavorite;

	[ObservableProperty]
	public int profilesCount;

	[ObservableProperty]
	public long? creatorUserId;

	[ObservableProperty]
	private string? tags;


	public event EventHandler<string?>? TagsChanged;
}
