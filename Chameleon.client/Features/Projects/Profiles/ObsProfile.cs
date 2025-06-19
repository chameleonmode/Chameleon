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


namespace Chameleon.client.Features.Projects.Profiles;

public partial class ObsProfile : ObservableDtoViewModelBase<UserProfileDto> {
	[ObservableProperty] string isChromeRunning = "False";
	[ObservableProperty] string isBraveRunning = "False";
	[ObservableProperty] string isFFRunning = "False";
	[ObservableProperty] bool isShowGlyph = true;
	[ObservableProperty] bool isShowCheckboxColumn = true;

	public Dictionary<SystemBrowserType, IBrowserInstance?> SBI { get; } = new() {
		[SystemBrowserType.Chrome] = null,
		[SystemBrowserType.Firefox] = null,
		[SystemBrowserType.Brave] = null
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

	string SetRunning(SystemBrowserType args, bool running) => args switch {
		SystemBrowserType.Chrome => IsChromeRunning = running.ToString(),
		SystemBrowserType.Firefox => IsFFRunning = running.ToString(),
		SystemBrowserType.Brave => IsBraveRunning = running.ToString(),
		_ => throw new NotImplementedException()
	};
	public ObsProfile(UserProfileDto profile, Action<ObsProfile>? selectedChanged = default, Action<ObsProfile>? onDeleted = default)
	: base(profile, onSelectedChanged: selectedChanged != null ? (vm) => selectedChanged((ObsProfile)vm) : null) {
		AsyncCommandMap["OpenFirefox"] = async () => await OpenSystemBrowser(SystemBrowserType.Firefox);
		AsyncCommandMap["OpenChrome"] = async () => await OpenSystemBrowser(SystemBrowserType.Chrome);
		AsyncCommandMap["OpenBrave"] = async () => await OpenSystemBrowser(SystemBrowserType.Brave);

		AsyncCommandMap["ImportCookiesChrome"] = async () => await HandleCookieOperation("ImportCookiesChrome", SystemBrowserType.Chrome);
		AsyncCommandMap["ImportCookiesBrave"] = async () => await HandleCookieOperation("ImportCookiesBrave", SystemBrowserType.Brave);
		AsyncCommandMap["ImportCookiesFirefox"] = async () => await HandleCookieOperation("ImportCookiesFirefox", SystemBrowserType.Firefox);

		AsyncCommandMap["ExportCookiesChrome"] = async () => await HandleCookieOperation("ExportCookiesChrome", SystemBrowserType.Chrome);
		AsyncCommandMap["ExportCookiesBrave"] = async () => await HandleCookieOperation("ExportCookiesBrave", SystemBrowserType.Brave);
		AsyncCommandMap["ExportCookiesFirefox"] = async () => await HandleCookieOperation("ExportCookiesFirefox", SystemBrowserType.Firefox);

		AsyncCommandMap["Favorite"] = async () => {
			_ = await UserProfilesRepo.SetProfileIsFavorite(profile);
			OnPropertyChanged(nameof(IsFavorite));
		};
		AsyncCommandMap["DeleteUserProfile"] = async () => {
			if (
				await MessageBox.Show(title: "Delete User Profile",
				content: $"Are you sure you want to delete {profile.title}?",
				btns: MBoxButtons.OkCancel,
				fontIconInfo: "DeleteLines")
			) {
				_ = await UserProfilesRepo.Instance.Delete(profile.id);
				if (Navigator.Instance.IsCurrentView("IdentityView")) Navigator.GoBack();

				onDeleted?.Invoke(this);
			}
		};

		CommandMap["OpenTopmostController"] = () => SnapCracklePopViewModel.Open(Dto);
		CommandMap["ShowViewProfile"] = () => DialogBox.ShowTopmost<UserProfileSidePanelUserControl, UserProfileSidePanelViewModel>(
			vm: new UserProfileSidePanelViewModel(profile),
			title: "Copy Pasta",
			width: 156
		);

		var browsers = SystemBrowserService.Instance.HasInstanceOf(Dto.id, (sender, args) => {
			// TODO: ? IsForeground = args.EventType == SysBrowserEventType.Foreground;
			var runnin = args.EventType switch {
				SysBrowserEventType.Foreground or
				SysBrowserEventType.Background or
				SysBrowserEventType.Opened => SetRunning(args.OpenOptions.BrowserType, true),
				SysBrowserEventType.Closed => SetRunning(args.OpenOptions.BrowserType, false),
				_ => "Error"
			};
			if (runnin is "Error" or "False") SBI[args.OpenOptions.BrowserType] = null;
		});
		// browsers.ForEach(b => _ = SetRunning(b, true));
	}

	public void Navigate() {
		if (!IsActionOptionsVisible) return;
		Navigator.Instance.NavigateTo("IdentityView", Dto);
	}

	public async Task<IBrowserInstance?> OpenSystemBrowser(SystemBrowserType browserType, bool foreground = true) {
		if (SBI[browserType] is IBrowserInstance browser && foreground) browser.InvokeEvent(SysBrowserEventType.Foreground);
		else if (SBI[browserType] is null) SBI[browserType] = await SystemBrowserService.Instance.Open(new(browserType, SystemBrowserProfile));
		return SBI[browserType];
	}

	public async Task<IBrowserInstance?> SwitchToUIMode(SystemBrowserType browserType) {
		if (SBI[browserType] is IBrowserInstance browser) {
			if (browser.Settings.OpenOptions.Headless) {
				browser.Close();
				SBI[browserType] = null;
				await Task.Delay(500);
				return await OpenSystemBrowser(browserType, foreground: true, headless: false);
			}
			// If already in UI mode, just bring to foreground
			browser.InvokeEvent(SysBrowserEventType.Foreground);
			return browser;
		}
		// If not running, just launch in UI mode
		return await OpenSystemBrowser(browserType, foreground: true, headless: false);
	}

	public async Task<IReadOnlyList<BrowserContextCookiesResult>?> GetCookiesAsync(SystemBrowserType browserType, bool closeAfter = false) {
		var browserInstance = SBI.TryGetValue(browserType, out var inst) ? inst : null;
		var needToLaunch = browserInstance == null || browserInstance.Brocess == null || browserInstance.Brocess.HasExited;
		if (needToLaunch) {
			browserInstance = await OpenSystemBrowser(browserType, foreground: false, headless: true);
			if (browserInstance == null || browserInstance.Brocess == null || browserInstance.Brocess.HasExited) {
				Toaster.Error($"Failed to launch {browserType} for cookie extraction.");
				return null;
			}
		}

		try {
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
			var isLoaded = await browserInstance!.LoadedTCS.Task.WaitAsync(cts.Token);
			if (!isLoaded) {
				Toaster.Error($"{browserType} failed to initialize for cookie extraction.");
				return null;
			}

			var port = browserInstance.Settings.Port;
			if (port <= 0) {
				Toaster.Error($"Invalid debugging port for {browserType}.");
				return null;
			}

			var cookies = await Util.GetCookies(new(new(browserType, SystemBrowserProfile), port));
			return cookies;
		} catch (TimeoutException) {
			Toaster.Error($"{browserType} initialization timed out for cookie extraction.");
			return null;
		} catch (OperationCanceledException) {
			Toaster.Error($"{browserType} initialization was cancelled or timed out for cookie extraction.");
			return null;
		} catch (Exception ex) {
			Toaster.Error($"Failed to extract cookies from {browserType}: {ex.Message}");
			return null;
		} finally {
			var alreadyRunningInUIMode = await IsBrowserRunningInUIModeAsync(browserInstance!, browserType);//Prevent closing browser if it was already started in UI mode
			if (!alreadyRunningInUIMode && closeAfter && browserInstance != null && browserInstance.Brocess != null && !browserInstance.Brocess.HasExited) {
				browserInstance.Close();
				SBI[browserType] = null;
			}
		}
	}

	private async Task HandleCookieOperation(string operation, SystemBrowserType browserType) {
		var isImport = operation.StartsWith("Import");
		var browserName = browserType.ToString();

		var visual = TopLevel.GetTopLevel(App.MainWindow?.GetVisualRoot() as Visual);
		var topLevel = visual != null ? TopLevel.GetTopLevel(visual) : null;

		if (isImport) {
			if (topLevel == null) {
				Toaster.Error($"Error getting top level window for dialog. Ensure the view is active.");
				return;
			}

			var file = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
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
						await SetCookiesAsync(browserType, cookies);
						Toaster.Success($"Successfully imported {cookies.Count} cookies for {browserName}.");
					} else {
						Toaster.Error($"Failed to deserialize cookies for {browserName}.");
					}
				} catch (Exception ex) {
					Toaster.Error($"Error importing cookies for {browserName}: {ex.Message}");
				}
			}
		} else {
			var cookiesToExport = await GetCookiesAsync(browserType);
			if (cookiesToExport == null || !cookiesToExport.Any()) {
				Toaster.Info($"No cookies found to export for {browserName}.");
				return;
			}

			if (topLevel == null) {
				Toaster.Error($"Error getting top level window for dialog. Ensure the view is active.");
				return;
			}

			var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
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

	private async Task SetCookiesAsync(SystemBrowserType browserType, IEnumerable<Cookie> cookies,bool closeAfter = false) {
		var browserInstance = SBI.TryGetValue(browserType, out var inst) ? inst : null;
		var needToLaunch = browserInstance == null || browserInstance.Brocess == null || browserInstance.Brocess.HasExited;
		if (needToLaunch) {
			browserInstance = await OpenSystemBrowser(browserType, foreground: false, headless: true);
			if (browserInstance == null || browserInstance.Brocess == null || browserInstance.Brocess.HasExited) {
				Toaster.Error($"Failed to launch {browserType} for cookie import.");
				return;
			}
		}

		try {
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
			var isLoaded = await browserInstance!.LoadedTCS.Task.WaitAsync(cts.Token);
			if (!isLoaded) {
				Toaster.Error($"{browserType} failed to initialize for cookie import.");
				return;
			}
			var port = browserInstance.Settings.Port;
			if (port <= 0) {
				Toaster.Error($"Invalid debugging port for {browserType}.");
				return;
			}

			await Util.SetCookies(
					new(new(browserType, SystemBrowserProfile), port),
					cookies.Select(c => new Cookie {
						Name = c.Name,
						Value = c.Value,
						Domain = c.Domain,
						Path = c.Path,
						Expires = (float?)c.Expires,
						HttpOnly = c.HttpOnly,
						Secure = c.Secure,
						SameSite = Enum.TryParse<SameSiteAttribute>(c.SameSite?.ToString(), true, out var sameSiteEnum) ? sameSiteEnum : SameSiteAttribute.Lax
					})
			);
		} catch (Exception ex) {
			Toaster.Error($"Failed to set cookies in running {browserType} instance: {ex.Message}");
		} finally {
			var alreadyRunningInUIMode = await IsBrowserRunningInUIModeAsync(browserInstance!, browserType);//Prevent closing browser if it was already started in UI mode
			if (!alreadyRunningInUIMode && closeAfter && browserInstance != null && browserInstance.Brocess != null && !browserInstance.Brocess.HasExited) {
				browserInstance.Close();
				SBI[browserType] = null;
			}
		}
	}

	private async Task<bool> IsBrowserRunningInUIModeAsync(IBrowserInstance browserInstance, SystemBrowserType browserType) {

		if (browserInstance == null || browserInstance.Brocess == null || browserInstance.Brocess.HasExited)
			return false;

		var headlessArg = browserType == SystemBrowserType.Firefox ? "-headless" : "--headless";
		if (ProcessUtil.HasCommandLineArgument(browserInstance.Brocess, headlessArg))
			return false;

		var hasWindow = false;
		try {
			hasWindow = !browserInstance.Brocess.HasExited && browserInstance.Brocess.MainWindowHandle != IntPtr.Zero;
		} catch (InvalidOperationException) {
			return false;
		}

		if (!hasWindow) {
			return false;
		}

		var port = browserInstance.Settings.Port;
		if (port <= 0) {
			return true;
		}

		try {
			using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
			var playwrightBrowser = browserType == SystemBrowserType.Firefox ? playwright.Firefox : playwright.Chromium;
			await using var browser = await playwrightBrowser.ConnectOverCDPAsync($"http://localhost:{port}", new() { Timeout = 2000 });
			return browser.IsConnected;
		} catch {
			return false;
		}
	}
}
