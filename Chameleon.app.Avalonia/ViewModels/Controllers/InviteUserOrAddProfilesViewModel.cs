using Chameleon.app.Avalonia.Models.Observable;
using System.Collections.ObjectModel;

using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Api.Repos;
using DynamicData;
using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.app.Avalonia.DynamicData;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class InviteUserOrAddProfilesViewModel : ViewModelObjectBase
{
	[ObservableProperty]
	private string? assistantName;
	[ObservableProperty]
	private string? assistantEmail;
	[ObservableProperty]
	private bool showUserInfo = true;

	//
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ObservableCollection<ObsProfile> SelectedProfiles { get; } = [];

	//
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public ObservableCollection<ObsFolder> SelectedFolders { get; } = [];

	public InviteUserOrAddProfilesViewModel()
	{
		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new ObsProfile(
				userProfile: i,
				hasActionOptions: false,
				onSelectedChanged: p => {
					if (p.IsSelected) {
						if (!SelectedProfiles.Contains(p))
							SelectedProfiles.Add(p);
					} else {
						_ = SelectedProfiles.Remove(p);
					}
				})
			)
			.SortAndBind(out var profiles, Compares.ObsProfileCompares.AscendingComparer)
			.Subscribe(async p => {
				var pre = SelectedProfiles.ToList();
				SelectedProfiles.Clear();
				await Task.Delay(64);
				foreach (var item in pre) {
					var cp = Profiles?.First(pr => pr.Dto!.id == item.Dto!.id);
					if (cp != null) {
						cp.IsSelected = true;
						SelectedProfiles.Add(cp);
					}
				}
			});
		Profiles = profiles;

		_ = UserProfilesFolderRepo
			.Connect(i => i.id != 0)
			.Transform(i => new ObsFolder(
				folder: i,
				hasActionOptions: false,
				onSelectedChanged: p => {
					if (p.IsSelected) {
						if (!SelectedFolders.Contains(p))
							SelectedFolders.Add(p);
					} else {
						_ = SelectedFolders.Remove(p);
					}
				})
			)
			.SortAndBind(out var folders, Compares.ObsFolderCompares.AscendingComparer)
			.Subscribe(async p => {
				var pre = SelectedFolders.ToList();
				SelectedFolders.Clear();
				await Task.Delay(64);
				foreach (var item in pre) {
					var cp = Folders?.First(pr => pr.Dto!.id == item.Dto!.id);
					if (cp != null) {
						cp.IsSelected = true;
						SelectedFolders.Add(cp);
					}
				}
			});
		Folders = folders;
	}
}