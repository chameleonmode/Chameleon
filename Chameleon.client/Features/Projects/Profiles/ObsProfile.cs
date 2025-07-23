using Avalonia;
using Avalonia.Platform.Storage;
using Chameleon.client.Features.Projects.Profiles.Dialogs;
using Chameleon.client.MvvM;
using Chameleon.client.Services;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Dto;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Browzio;
using Chameleon.lib.Browzio.Services.Browzas;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.Services;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using FluentAvalonia.Core;
using Microsoft.Playwright;
using System.Collections.ObjectModel;
using Event = Chameleon.lib.Browzio.Browzers.Event;
using BrowserType = Chameleon.lib.Browzio.BrowserType;

namespace Chameleon.client.Features.Projects.Profiles;

public partial class ObsProfile : OODTOVM<UserProfileDto>, IProfileUIContextAware {
	[ObservableProperty] int isChromeRunning;
	[ObservableProperty] int isBraveRunning;
	[ObservableProperty] int isFFRunning;
	[ObservableProperty] bool foreground;
	[ObservableProperty] bool isShowGlyph = true;
	[ObservableProperty] bool isShowCheckboxColumn = true;
	[ObservableProperty] bool isSelectionEnabled = true;

	public ObservableCollection<AvailableBrowser> Browsers { get; } = [];
	public ProfileUIContext CurrentContext { get; private set; } = ProfileUIContext.Profiles;
	public ProfileUIContext? PreviousContext { get; private set; }

	public Dictionary<BrowserType, IBrowserInstance?> SBI { get; } = new() {
		// [BrowserType.Chrome] = null,
		// [BrowserType.Firefox] = null,
		// [BrowserType.Brave] = null
	};

	public bool IsSharedProfile => Dto.creatorUserId != Auther.AuthSession?.UserId;
	public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];
	public bool IsFavorite => Dto.isFavourite;

	public BrowserProfile BP => new() {
		Id = Dto.id,
		Proxy = new (Dto.proxy?.host, Dto.proxy?.port ?? 0, Dto.proxy?.userName, Dto.proxy?.password)
	};
	public ReadOnlyObservableCollection<UPLoginDto> ProfileLogins {
		get {
			_ = UPAdditionalDataRepo.Instance.Loginz
			.Connect(i => i.ProfileId == Dto.id)
			.Bind(out var logins)
			.Subscribe();
			return logins;
		}
	}

	public ObsProfile(UserProfileDto profile, Action<ObsProfile>? selectedChanged = default, Action<ObsProfile>? onDeleted = default)
	 : base(profile, onSelectedChanged: selectedChanged != null ? (vm) => selectedChanged((ObsProfile)vm) : null) {
		//@ TODO: remove for more optimized implementation
		Browsers.AddRange(
			ProfilesViewModel.Instance.Browsers.Select(b => new AvailableBrowser(b.Info, sync: (op) => HandleCookieOperation(op, b.Info.Type)))
		);
		foreach (var browser in Browsers) {
			AsyncCommandMap[browser.Info.Type.ToString()] = () => OpenBrowser(browser.Info.Type);
			AsyncCommandMap[$"sync-in-{browser.Info.Type}"] = () => HandleCookieOperation(CookieOp.Import, browser.Info.Type);
			SBI[browser.Info.Type] = null;
		}
		Browzio.I.Browzas.AddObserver(Dto.id, (s, e) => {
			Foreground = false; // @TODO: optimization
			if (Dto.id != e.Settings.Profile.Id) return;
			Foreground = e.Event is Event.Foreground or Event.Opened;

			// Update running state based on event
			var running = e.Event switch {
				Event.Foreground or Event.Background or Event.Opened => 1,
				Event.Error => -1,
				_ => 0
			};

			int SetRunning(int current) => (current != -1 || running == 1) ? running : current;
			var browser = Browsers.FirstOrDefault(b => b.Info.Type == e.Settings.BrowserType);
			if (browser is not null) browser.Running = SetRunning(browser.Running);

			_ = e.Settings.BrowserType switch {
				BrowserType.Chrome => IsChromeRunning = SetRunning(IsChromeRunning),
				BrowserType.Firefox => IsFFRunning = SetRunning(IsFFRunning),
				BrowserType.Brave => IsBraveRunning = SetRunning(IsBraveRunning),
				_ => 0
			};
			if (running <= 0) SBI[e.Settings.BrowserType] = null;
		});
		#region  commands
		// AsyncCommandMap["brave"] = () => OpenBrowser(BrowserType.Brave);
		// AsyncCommandMap["chrome"] = () => OpenBrowser(BrowserType.Chrome);
		// AsyncCommandMap["firefox"] = () => OpenBrowser(BrowserType.Firefox);

		AsyncCommandMap["SyncCookiesChrome"] = async () => await HandleCookieOperation(CookieOp.Import, BrowserType.Chrome);
		AsyncCommandMap["SyncCookiesBrave"] = async () => await HandleCookieOperation(CookieOp.Import, BrowserType.Brave);
		AsyncCommandMap["SyncCookiesFirefox"] = async () => await HandleCookieOperation(CookieOp.Import, BrowserType.Firefox);

		AsyncCommandMap["ExportCookiesChrome"] = async () => await HandleCookieOperation(CookieOp.Export, BrowserType.Chrome);
		AsyncCommandMap["ExportCookiesBrave"] = async () => await HandleCookieOperation(CookieOp.Export, BrowserType.Brave);
		AsyncCommandMap["ExportCookiesFirefox"] = async () => await HandleCookieOperation(CookieOp.Export, BrowserType.Firefox);

		AsyncCommandMap["Favorite"] = async () => {
			_ = await UserProfilesRepo.SetProfileIsFavorite(profile);
			OnPropertyChanged(nameof(IsFavorite));
		};
		AsyncCommandMap["Delete"] = async () => {
			if (
				await MessageBox.Show("Delete", $"Are you sure you want to delete {profile.title}?",
				btns: MBoxButtons.OkCancel,
				icon: "DeleteLines")
			) {
				_ = await UserProfilesRepo.Instance.Delete(profile.id);
				if (Navigator.Instance.IsCurrentView("IdentityView")) Navigator.GoBack();

				onDeleted?.Invoke(this);
			}
		};

		CommandMap["expand"] = () => SnapCracklePopViewModel.Open(this);
		CommandMap["copy"] = () => UserProfileSidePanelViewModel.Open(this);
		#endregion
	}

	public void Navigate() {
		if (!IsActionOptionsVisible || ProfilesViewModel.Instance.AsyncCfVCommand.IsRunning) return;
		Navigator.Instance.NavigateTo("IdentityView", Dto);
	}

	public async Task<IBrowserInstance?> OpenBrowser(BrowserType bt) {
		if (SBI[bt] is IBrowserInstance browser) browser.InvokeEvent(Event.Foreground);
		else if (SBI[bt] is null) return SBI[bt] = await Browzio.I.Browzas.Open(Browzio.Factory.BrowserSettings(bt, BP));
		return SBI[bt];
	}

	public async Task<IReadOnlyList<BrowserContextCookiesResult>?> GetCookies(BrowserType browserType) {
		return await ExecuteBrowserAction(
			browserType,
			async port => await Sync.GetCookies(new(new(browserType, BP), port))
		);
	}

	private async Task HandleCookieOperation(CookieOp operation, BrowserType browserType) {
		if (operation == CookieOp.Import) {
			var file = await App.MainWindow!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
				Title = $"Import Cookies for {browserType}",
				AllowMultiple = false,
				FileTypeFilter = [new FilePickerFileType("JSON files") { Patterns = ["*.json"] }]
			});
			(file.Count == 1).ThrowFalse($"Select a single file to import cookies for {browserType}.");

			await using var stream = await file[0].OpenReadAsync();
			using var reader = new StreamReader(stream);
			var json = await reader.ReadToEndAsync();
			var pwCookies = JSON.Deserialize<List<BrowserContextCookiesResult>>(json);
			ArgumentNullException.ThrowIfNull(pwCookies, "Failed to deserialize cookies from JSON file."); ;

			await ExecuteBrowserAction(
				browserType,
				port => Sync.SetCookies(
					new(new(browserType, BP), port),
					pwCookies.Select(c => new Cookie {
						Name = c.Name,
						Value = c.Value,
						Domain = c.Domain,
						Path = c.Path,
						Expires = c.Expires,
						HttpOnly = c.HttpOnly,
						Secure = c.Secure,
						SameSite = Enum.TryParse<SameSiteAttribute>(c.SameSite.ToString(), true, out var sameSiteEnum) ? sameSiteEnum : SameSiteAttribute.Lax
					}).ToList()
			));
		} else {
			var cookiesToExport = await GetCookies(browserType) ?? [];
			cookiesToExport.Any().ThrowFalse($"No cookies found to export for {browserType}.");

			var file = await App.MainWindow!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
				Title = $"Export Cookies for {browserType}",
				SuggestedFileName = $"{browserType}Cookies_{DateTime.Now:yyyyMMddHHmmss}.json",
				DefaultExtension = "json",
				FileTypeChoices = [new FilePickerFileType("JSON files") { Patterns = ["*.json"] }]
			});
			ArgumentNullException.ThrowIfNull(file, "File selection was cancelled or failed.");

			var json = JSON.Serialize(cookiesToExport);
			await using var stream = await file.OpenWriteAsync();
			await using var writer = new StreamWriter(stream);
			await writer.WriteAsync(json);
		}

		Toaster.Success($"Successfully {operation}ed cookies for {browserType}.");
	}

	private async Task<T?> ExecuteBrowserAction<T>(BrowserType browserType, Func<int?, Task<T>> action) {
		var wasOpen = SBI.TryGetValue(browserType, out var browser) && browser != null;
		if (browserType == BrowserType.Firefox) await Processez.TryKillProcess(browser?.Brocess);
		return await action(SBI[browserType]?.Settings.Port);
	}

	public void SetUIContext(ProfileUIContext context) {
		if (CurrentContext == context) return;

		if (!ProfileUIStateMachine.CanTransition(CurrentContext, context)) {
			throw new InvalidOperationException($"Cannot transition from {CurrentContext} to {context}");
		}

		PreviousContext = CurrentContext;
		CurrentContext = context;
		var state = ProfileUIStateMachine.GetStateFor(context);

		IsShowCheckboxColumn = state.IsShowCheckboxColumn;
		IsShowGlyph = state.IsShowGlyph;

		OnContextChanged(PreviousContext, context);
	}

	protected virtual void OnContextChanged(ProfileUIContext? from, ProfileUIContext to) {

	}
}
