using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class ObsProxySetting : ViewModelObjectBase{
	[ObservableProperty]
	private string? host;
	[ObservableProperty]
	private int port = 80;
	[ObservableProperty]
	private string? userName;
	[ObservableProperty]
	private string? password;
	[ObservableProperty]
	private ObsProfile obsProfile;

	public ObsProxySetting(ObsProfile profile)
	{
		obsProfile = profile;
		host = obsProfile.Dto!.proxy!.host;
		port = obsProfile.Dto.proxy.port;
		userName = obsProfile.Dto.proxy.userName;
		password = obsProfile.Dto.proxy.password;
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

