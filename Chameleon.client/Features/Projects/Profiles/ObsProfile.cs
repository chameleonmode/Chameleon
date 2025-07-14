using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Chameleon.client.Features.Projects.Profiles.Dialogs;
using Chameleon.client.MvvM;
using Chameleon.client.Services;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Dto;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.Util;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.WebBrowser.Browsers;
using Chameleon.lib.WebBrowser.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Microsoft.Playwright;
using System.Collections.ObjectModel;
using System.Text.Json;
using BrowserType = Chameleon.lib.WebBrowser.BrowserType;

namespace Chameleon.client.Features.Projects.Profiles;

public partial class ObsProfile : OODTOVM<UserProfileDto>, IProfileUIContextAware {
	[ObservableProperty] string isChromeRunning = "False";
	[ObservableProperty] string isBraveRunning = "False";
	[ObservableProperty] string isFFRunning = "False";
	[ObservableProperty] bool isShowGlyph = true;
	[ObservableProperty] bool isShowCheckboxColumn = true;
	[ObservableProperty] bool isActionOptionsVisible = true;
	[ObservableProperty] bool isSelectionEnabled = true;

	public ProfileUIContext CurrentContext { get; private set; } = ProfileUIContext.Profiles;
	public ProfileUIContext? PreviousContext { get; private set; }

	public Dictionary<BrowserType, IBrowserInstance?> SBI { get; } = new() {
		[BrowserType.Chrome] = null,
		[BrowserType.Firefox] = null,
		[BrowserType.Brave] = null
	};

	public bool IsSharedProfile => Dto.creatorUserId != Auther.AuthSession?.UserId;
	public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];
	public bool IsFavorite => Dto.isFavourite;

	public BrowserProfile SystemBrowserProfile => new() {
		Id = Dto.id,
		Proxy = new BrowserProxy() {
			Host = Dto.proxy?.host,
			Port = Dto.proxy?.port ?? 0,
			UserName = Dto.proxy?.userName,
			Password = Dto.proxy?.password
		}
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
		AsyncCommandMap["OpenFirefox"] = async () => await OpenBrowser(BrowserType.Firefox);
		AsyncCommandMap["OpenChrome"] = async () => await OpenBrowser(BrowserType.Chrome);
		AsyncCommandMap["OpenBrave"] = async () => await OpenBrowser(BrowserType.Brave);

		AsyncCommandMap["SyncCookiesChrome"] = async () => await HandleCookieOperation("ImportCookiesChrome", BrowserType.Chrome);
		AsyncCommandMap["SyncCookiesBrave"] = async () => await HandleCookieOperation("ImportCookiesBrave", BrowserType.Brave);
		AsyncCommandMap["SyncCookiesFirefox"] = async () => await HandleCookieOperation("ImportCookiesFirefox", BrowserType.Firefox);

		AsyncCommandMap["ExportCookiesChrome"] = async () => await HandleCookieOperation("ExportCookiesChrome", BrowserType.Chrome);
		AsyncCommandMap["ExportCookiesBrave"] = async () => await HandleCookieOperation("ExportCookiesBrave", BrowserType.Brave);
		AsyncCommandMap["ExportCookiesFirefox"] = async () => await HandleCookieOperation("ExportCookiesFirefox", BrowserType.Firefox);

		AsyncCommandMap["Favorite"] = async () => {
			_ = await UserProfilesRepo.SetProfileIsFavorite(profile);
			OnPropertyChanged(nameof(IsFavorite));
		};
		AsyncCommandMap["DeleteUserProfile"] = async () => {
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

		CommandMap["OpenTopmostController"] = () => SnapCracklePopViewModel.Open(this);
		CommandMap["ShowViewProfile"] = () => DialogBox.ShowTopmost<UserProfileSidePanelUserControl, UserProfileSidePanelViewModel>(
			vm: new UserProfileSidePanelViewModel(profile),
			title: "Copy Pasta",
			width: 156
		);

		_ = SystemBrowser.I.HasInstanceOf(Dto.id, (sender, args) => {
			var isRunning = args.Event switch {
				Event.Foreground or Event.Background or Event.Opened => true.ToString(),
				Event.Closed => false.ToString(),
				_ => "Error"
			};

			switch (args.Settings.BrowserType) {
				case BrowserType.Chrome: IsChromeRunning = isRunning; break;
				case BrowserType.Firefox: IsFFRunning = isRunning; break;
				case BrowserType.Brave: IsBraveRunning = isRunning; break;
			}

			if (args.Event is not Event.Foreground or Event.Background or Event.Opened) SBI[args.Settings.BrowserType] = null;
		});
		//@ TODO:  _.ForEach(b => _ = SetRunning(b, true)); n remove
	}

	public void Navigate() {
		if (!IsActionOptionsVisible) return;
		Navigator.Instance.NavigateTo("IdentityView", Dto);
	}

	public async Task<IBrowserInstance?> OpenBrowser(BrowserType browserType, bool foreground = true) {
		if (SBI[browserType] is IBrowserInstance browser && foreground) browser.InvokeEvent(Event.Foreground);
		else if (SBI[browserType] is null) return SBI[browserType] = await SystemBrowser.I.Open(new BrowserSetting(browserType, SystemBrowserProfile));
		return SBI[browserType];
	}

	public Task<IReadOnlyList<BrowserContextCookiesResult>?> GetCookiesAsync(BrowserType browserType) =>
		ExecuteBrowserActionAsync(
			browserType,
			"cookie extraction",
			port => Util.GetCookies(new(new(browserType, SystemBrowserProfile), port))
		);

	private async Task HandleCookieOperation(string operation, BrowserType browserType) {
		var isImport = operation.StartsWith("Import");
		var browserName = browserType.ToString();
		if (isImport) {
			var file = await App.MainWindow!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
				Title = $"Import Cookies for {browserName}",
				AllowMultiple = false,
				FileTypeFilter = [new FilePickerFileType("JSON files") { Patterns = ["*.json"] }]
			});

			if (file.Count == 1) {
				try {
					await using var stream = await file[0].OpenReadAsync();
					using var reader = new StreamReader(stream);
					var json = await reader.ReadToEndAsync();
					var pwCookies = JsonSerializer.Deserialize<List<BrowserContextCookiesResult>>(json);
					if (pwCookies != null) {
						var cookies = pwCookies.Select(c => new Cookie {
							Name = c.Name,
							Value = c.Value,
							Domain = c.Domain,
							Path = c.Path,
							Expires = c.Expires,
							HttpOnly = c.HttpOnly,
							Secure = c.Secure,
							SameSite = Enum.TryParse<SameSiteAttribute>(c.SameSite.ToString(), true, out var sameSiteEnum) ? sameSiteEnum : SameSiteAttribute.Lax
						}).ToList();

						await ExecuteBrowserActionAsync(
							browserType,
							"cookie import",
							port => Util.SetCookies(
									new(new(browserType, SystemBrowserProfile), port),
									cookies.Select(c => new Cookie {
										Name = c.Name,
										Value = c.Value,
										Domain = c.Domain,
										Path = c.Path,
										Expires = (float?)c.Expires,
										HttpOnly = c.HttpOnly,
										Secure = c.Secure,
										SameSite =
											Enum.TryParse<SameSiteAttribute>(c.SameSite?.ToString(), true, out var sameSiteEnum)
											? sameSiteEnum 
											: SameSiteAttribute.Lax
									})
							)
						);
						Toaster.Success($"Successfully imported {cookies.Count} cookies for {browserName}.");
					} else {
						Toaster.Error($"Failed to deserialize cookies for {browserName}.");
					}
				} catch (Exception ex) {
					Toaster.Error($"Error importing cookies for {browserName}: {ex.Message}");
				}
			}
		} else {
			var cookiesToExport = await GetCookiesAsync(browserType) ?? [];
			cookiesToExport.Any().ThrowFalse($"No cookies found to export for {browserName}.");

			var file = await App.MainWindow!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
				Title = $"Export Cookies for {browserName}",
				SuggestedFileName = $"{browserName}Cookies_{DateTime.Now:yyyyMMddHHmmss}.json",
				DefaultExtension = "json",
				FileTypeChoices = [new FilePickerFileType("JSON files") { Patterns = ["*.json"] }]
			});

			if (file != null) {
				try {
					var json = JsonSerializer.Serialize(cookiesToExport, new JsonSerializerOptions { WriteIndented = true });
					await using var stream = await file.OpenWriteAsync();
					await using var writer = new StreamWriter(stream);
					await writer.WriteAsync(json);
					Toaster.Success($"Successfully exported {cookiesToExport.Count()} cookies for {browserName} to {file.Name}.");
				} catch (Exception ex) {
					Toaster.Error($"Error exporting cookies for {browserName}: {ex.Message}");
				}
			}
		}
	}

	private async Task<T?> ExecuteBrowserActionAsync<T>(BrowserType browserType, string actionName, Func<int, Task<T>> action) where T : class {
		var wasOpen = SBI.TryGetValue(browserType, out var browser) && browser != null;
		browser ??= await OpenBrowser(browserType, foreground: false);

		try {
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
			var isLoaded = await browser!.LoadedTCS.Task.WaitAsync(cts.Token);
			if (!isLoaded) throw new Exception($"Failed to load");

			var port = browser.Settings.Profile.Port;
			return port <= 0 ? throw new Exception($"Invalid debugging port") : await action(port);
		} catch (Exception ex) {
			var message = ex is TimeoutException or OperationCanceledException 
				? $"{browserType} initialization timed out for {actionName}."
				: $"{actionName} on {browserType}: {ex.Message}";
			Toaster.Error(message);
		} finally {
			if (!wasOpen && browser != null) {
				await Processez.TryKillProcess(browser.Brocess);
				browser.Close();
			}
		}
		return default;
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
