using Chameleon.lib.Abs;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class CookieSyncViewModel : ViewModelObjectBase {
	[ObservableProperty]
	private bool? hasCookiesToSync;

	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);
		var cookies = await ABService.Instance.GetCookiesAsync<object>();
		HasCookiesToSync = cookies?.Data!.Count > 0;
	}
}
