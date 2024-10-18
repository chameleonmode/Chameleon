using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class ObsProxySetting : ViewModelObjectBase{
	public event Action? OnSelectedChanged;

	[ObservableProperty]
	private string? host;
	[ObservableProperty]
	private int port = 80;
	[ObservableProperty]
	private string? userName;
	[ObservableProperty]
	private string? password;
	[ObservableProperty]
	private bool isSelected;
	[ObservableProperty]
	private ObsProfile obsProfile;

	public bool CanUse => Host?.Length > 0;
	public string? Server => CanUse ? $"{HostForRequest}:{Port}" : null;
	public string? ServerForRequest => CanUse ? $"http://{Server}" : null;
	public string? HostForRequest => CanUse ? Host!.Contains(Consts.Http.ChameleonModeHost) ? Consts.Http.PacketStreamHost : Host : null;
	public string UserProfileTitle => ObsProfile.Title ?? "XX";
	public string? Code {
		get {
			var list = UserProfileTitle
						.Split(" ")
						.Select(a => a.Trim().ToUpper()[0]);

			return list != null ? list.Count() > 2 ? list.Take(2).ToString() : string.Join("", list) : UserProfileTitle;

		}
	}

	public ObsProxySetting(ObsProfile profile)
	{
		obsProfile = profile;
		host = obsProfile.Dto!.proxy!.host;
		port = obsProfile.Dto.proxy.port;
		userName = obsProfile.Dto.proxy.userName;
		password = obsProfile.Dto.proxy.password;
	}
	partial void OnIsSelectedChanged(bool value)
	{
		OnSelectedChanged?.Invoke();
	}

	partial void OnPortChanged(int value)
	{
		if (value < 0 || value >= 65535) {
			Port = 0;
		}
	}

	public void SetProfile()
	{
		ObsProfile.Dto!.proxy!.host = Host;
		ObsProfile.Dto.proxy.port = Port;
		ObsProfile.Dto.proxy.userName = UserName;
		ObsProfile.Dto.proxy.password = Password;
	}
}

public partial class ObsProxyAccess
		: ViewModelObjectBase {

	[ObservableProperty]
	private string? url;
}

