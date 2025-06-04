using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
public partial class UserProfileViewModel : ObservableObjectBase {

	public UserProfileViewModel(UserProfileDto profile) {
		Id = profile.id;
		Title = profile.title;
		Notes = profile.notes;
		Tags = profile.Tags;
		FolderId = profile.folderId;
		IsFavourite = profile.isFavourite;
		ProxyId = profile.proxyId;
		LimitCache = profile.limitCache;
		YoutubeApiKey = profile.youtubeApiKey;
		YoutubeClientId = profile.youtubeClientId;
		YoutubeClientSecret = profile.youtubeClientSecret;
		WordPressSettings = profile.wordPressSettings;
		Proxy = new(profile.proxy);
		WebBrowser = new(profile.webBrowser);
	}

	[ObservableProperty] int id;
	[ObservableProperty] string? title;
	[ObservableProperty]  int creatorUserId;
	[ObservableProperty] int? folderId;
	[ObservableProperty] bool isFavourite;
	[ObservableProperty]  string? notes;
	[ObservableProperty] int proxyId;
	[ObservableProperty] float limitCache;
	[ObservableProperty] object? youtubeApiKey;
	[ObservableProperty] object? youtubeClientId;
	[ObservableProperty] object? youtubeClientSecret;
	[ObservableProperty] object? wordPressSettings;
	[ObservableProperty] ProxyViewModel proxy = new(new ProxDto());
	[ObservableProperty] WebrowserViewModel webBrowser;

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<UserProfileViewModel>();

		_ = builder.RuleFor(vm => vm.Title)
		.NotEmpty()
		.MaxLength(50)
		.WithMessage("Title is requried");

		return builder.Build(this);
	}

	public UserProfileDto ToDto() {
		return new UserProfileDto() {
			id = Id,
			title = Title,
			Tags = Tags,
			notes = Notes,
			folderId = FolderId,
			isFavourite = IsFavourite,
			proxyId = ProxyId,
			limitCache = LimitCache,
			youtubeApiKey = YoutubeApiKey,
			youtubeClientId = YoutubeClientId,
			youtubeClientSecret = YoutubeClientSecret,
			wordPressSettings = WordPressSettings,
			proxy = Proxy.ToDto(),
			webBrowser = WebBrowser.ToDto()
		};
	}
}

public partial class ProxyViewModel : ObservableObjectBase {

	public ProxyViewModel(ProxDto proxy) {
		Id = proxy.id;
		Title = proxy.title;
		Tags = proxy.Tags;
		Host = proxy.host;
		Port = "" + proxy.port;
		UserName = proxy.userName;
		Password = proxy.password;
	}

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	public string? host;

	[ObservableProperty]
	public string port;

	[ObservableProperty]
	public string? userName;

	[ObservableProperty]
	public string? password;

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<ProxyViewModel>();

		_ = builder.RuleFor(vm => vm.Host)
								.NotEmpty()
								.WithMessage("Consider using a proxy");

		_ = builder.RuleFor(vm => vm.Port)
							 .Must(x => Host.Is() || (Host.IsNot() && x.PropertyValue is string s && int.TryParse(s, out var port) && port > 0))
							 .WithMessage("Valid port is requried when using a proxy");

		return builder.Build(this);
	}

	partial void OnHostChanged(string? value) {
		if (value.Is()) {
			Port = string.Empty;
		} else {
			if (Port.Is() || !int.TryParse(Port, out var port) || port <= 0) {
				Port = "0000";
			}
		}
	}

	public ProxDto ToDto() {
		return new ProxDto() {
			id = Id,
			title = Title,
			Tags = Tags,
			host = Host,
			port = int.TryParse(Port, out var port) ? port : 0,
			userName = UserName,
			password = Password
		};
	}
}

public partial class WebrowserViewModel : ObservableObjectBase {
	public WebrowserViewModel(WebrowserDto webrowser) {
		WebRTC = webrowser.webRTC;
		WebGL = webrowser.webGL;
		Tracking = webrowser.tracking;
		Flash = webrowser.flash;
		Canvas = webrowser.canvas;
		UserAgentId = webrowser.userAgentId;
	}

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

	public WebrowserDto ToDto() {
		return new WebrowserDto() {
			webRTC = WebRTC,
			webGL = WebGL,
			tracking = Tracking,
			flash = Flash,
			canvas = Canvas,
			userAgentId = UserAgentId
		};
	}
}