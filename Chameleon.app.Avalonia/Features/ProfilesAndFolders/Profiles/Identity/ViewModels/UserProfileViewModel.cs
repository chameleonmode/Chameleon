using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
public partial class UserProfileViewModel : ObservableObjectBase {

	[ObservableProperty]
	public string? title;

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	public int creatorUserId;

	[ObservableProperty]
	public int? folderId;

	[ObservableProperty]
	public bool isFavourite;

	[ObservableProperty]
	public string? notes;

	[ObservableProperty]
	public int proxyId;

	[ObservableProperty]
	public float limitCache;

	[ObservableProperty]
	public object? youtubeApiKey;

	[ObservableProperty]
	public object? youtubeClientId;

	[ObservableProperty]
	public object? youtubeClientSecret;

	[ObservableProperty]
	public object? wordPressSettings;

	[ObservableProperty]
	public ProxyViewModel proxy = new();

	[ObservableProperty]
	public WebrowserDto webBrowser = new();
}

public partial class ProxyViewModel : ObservableObjectBase {

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	public string? host;

	[ObservableProperty]
	public int port;

	[ObservableProperty]
	public string? userName;

	[ObservableProperty]
	public string? password;
}

public partial class WebrowserViewModel : ObservableObjectBase {
	[ObservableProperty]
	public bool webRTC;

	[ObservableProperty]
	public bool webGL;

	[ObservableProperty]
	public bool tracking;

	[ObservableProperty]
	public bool flash;

	[ObservableProperty]
	public float canvas;

	[ObservableProperty]
	public int? userAgentId;
}