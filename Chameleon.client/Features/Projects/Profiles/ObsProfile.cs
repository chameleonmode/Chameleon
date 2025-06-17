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
using Chameleon.lib.Util;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.WebBrowser.Browsers;
using Chameleon.lib.WebBrowser.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Microsoft.Playwright;
using System.Collections.ObjectModel;
using System.Text.Json;
using PlaywrightOptions = Chameleon.lib.Playwright.Services.Options;


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
			if (await MessageBox.Show(
				title: "Delete User Profile",
				content: $"Are you sure you want to delete {profile.title}?",
				btns: MBoxButtons.OkCancel,
				fontIconInfo: "DeleteLines"
			)) {
				_ = await UserProfilesRepo.Instance.Delete(profile.id);
				if (Navigator.Instance.IsCurrentView("IdentityView")) Navigator.GoBack();

				onDeleted?.Invoke(this);
			}
		};

		CommandMap["OpenTopmostController"] = OpenTopmostController;
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

	public void View() {
		if (!IsActionOptionsVisible) return;

		Navigator.Instance.NavigateTo("IdentityView", Dto);
	}

	public void OpenTopmostController() {
		DialogBox.ShowTopmost(
			vm: SnapCracklePopViewModel.Instance,
			v: SnapCracklePopUserControl.Instance,
			initialize: vm => {
				vm.RunningList.AddIfNotExists(new ObsProfile(Dto) { IsShowGlyph = false, IsShowCheckboxColumn = false }, p => p.Dto?.id == Dto.id);
			},
			onClosed: vm => {
				vm.RunningList.Clear();
			},
			title: "SCP",
			width: 172
		);
	}

	public async Task<IBrowserInstance?> OpenSystemBrowser(SystemBrowserType browserType, bool foreground = true) {
		if (SBI[browserType] is IBrowserInstance browser) {
			if (foreground && OperatingSystem.IsMacOS()) browser.Brocessor(false).Start();
			else if (foreground) browser.InvokeEvent(SysBrowserEventType.Foreground);
		} else SBI[browserType] = await SystemBrowserService.Instance.Open(new(browserType, SystemBrowserProfile));

		return SBI[browserType];
	}

	private async Task HandleCookieOperation(string operation, SystemBrowserType browserType)
	{
		var isImport = operation.StartsWith("Import");
		var browserName = browserType.ToString();
		
		var visual = TopLevel.GetTopLevel(App.MainWindow?.GetVisualRoot() as Visual); 
		var topLevel = visual != null ? TopLevel.GetTopLevel(visual) : null;


		if (isImport)
		{
			if (topLevel == null)
			{
				Toaster.Error($"Error getting top level window for dialog. Ensure the view is active.");
				return;
			}

			var file = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
			{
				Title = $"Import Cookies for {browserName}",
				AllowMultiple = false,
				FileTypeFilter = [new FilePickerFileType("JSON files") { Patterns = ["*.json"] }]
			});

			if (file.Count == 1)
			{
				try
				{
					await using var stream = await file[0].OpenReadAsync();
					using var reader = new StreamReader(stream);
					var json = await reader.ReadToEndAsync();
					var cookies = JsonSerializer.Deserialize<List<Cookie>>(json);
					if (cookies != null)
					{
						await SetCookiesAsync(browserType, cookies);
						Toaster.Success($"Successfully imported {cookies.Count} cookies for {browserName}.");
					}
					else
					{
						Toaster.Error($"Failed to deserialize cookies for {browserName}.");
					}
				}
				catch (Exception ex)
				{
					Toaster.Error($"Error importing cookies for {browserName}: {ex.Message}");
				}
			}
		}
		else
		{
			var cookiesToExport = await GetCookiesAsync(browserType);
			if (cookiesToExport == null || !cookiesToExport.Any())
			{
				Toaster.Info($"No cookies found to export for {browserName}.");
				return;
			}

			if (topLevel == null)
			{
				Toaster.Error($"Error getting top level window for dialog. Ensure the view is active.");
				return;
			}

			var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
			{
				Title = $"Export Cookies for {browserName}",
				SuggestedFileName = $"{browserName}Cookies_{DateTime.Now:yyyyMMddHHmmss}.json",
				DefaultExtension = "json",
				FileTypeChoices = [new FilePickerFileType("JSON files") { Patterns = ["*.json"] }]
			});

			if (file != null)
			{
				try
				{
					var exportData = cookiesToExport.Select(c => new Cookie {
						Name = c.Name,
						Value = c.Value,
						Domain = c.Domain,
						Path = c.Path,
						Expires = c.Expires,
						HttpOnly = c.HttpOnly,
						Secure = c.Secure,
						SameSite = c.SameSite.ToString()
					}).ToList();
					var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
					await using var stream = await file.OpenWriteAsync();
					await using var writer = new StreamWriter(stream);
					await writer.WriteAsync(json);
					Toaster.Success($"Successfully exported {cookiesToExport.Count()} cookies for {browserName} to {file.Name}.");
				}
				catch (Exception ex)
				{
					Toaster.Error($"Error exporting cookies for {browserName}: {ex.Message}");
				}
			}
		}
	}

	private async Task<IReadOnlyList<BrowserContextCookiesResult>?> GetCookiesAsync(SystemBrowserType browserType)
	{
		var sysBrowserOpenOptions = new SysBrowserOpenOptions(browserType, SystemBrowserProfile);
		var settings = new SysBrowserSettings(sysBrowserOpenOptions, Dto.proxy?.port ?? 0);
		var userProfileDir = settings.SysBrowserProfileCachePath;

		Proxy? playwrightProxy = null;
		if (SystemBrowserProfile.Proxy != null && SystemBrowserProfile.Proxy.CanUse)
		{
			playwrightProxy = new Proxy
			{
				Server = SystemBrowserProfile.Proxy.Server!,
				Username = SystemBrowserProfile.Proxy.UserName,
				Password = SystemBrowserProfile.Proxy.Password
			};
		}

		var options = new PlaywrightOptions(
			sysBrowserOpenOptions, 
			null // Ensure Playwright launches a new browser instance
		);

		try
		{
			return await lib.Playwright.Services.Util.GetCookies(options);
		}
		catch (Exception ex)
		{
			Toaster.Error($"Error getting cookies for {browserType}: {ex.Message}");
			return null;
		}
	}

	private async Task SetCookiesAsync(SystemBrowserType browserType, IEnumerable<Cookie> cookies)
	{
		var sysBrowserOpenOptions = new SysBrowserOpenOptions(browserType, SystemBrowserProfile);
		var settings = new SysBrowserSettings(sysBrowserOpenOptions, Dto.proxy?.port ?? 0);
		var userProfileDir = settings.SysBrowserProfileCachePath;

		Proxy? playwrightProxy = null;
		if (SystemBrowserProfile.Proxy != null && SystemBrowserProfile.Proxy.CanUse)
		{
			playwrightProxy = new Proxy
			{
				Server = SystemBrowserProfile.Proxy.Server!,
				Username = SystemBrowserProfile.Proxy.UserName,
				Password = SystemBrowserProfile.Proxy.Password
			};
		}
		
		var options = new PlaywrightOptions(
			sysBrowserOpenOptions, 
			null // Ensure Playwright launches a new browser instance
		);

		using var playwright = await Playwright.CreateAsync();
		var pwBrowser = browserType == SystemBrowserType.Firefox ? playwright.Firefox : playwright.Chromium;
		
		var tempDir = Path.Combine(Path.GetTempPath(), "chameleon-cookie-import-temp", Guid.NewGuid().ToString());

		try
		{
			_ = Directory.CreateDirectory(tempDir);

			if (Directory.Exists(userProfileDir))
			{
				foreach (var dirPath in Directory.GetDirectories(userProfileDir, "*", SearchOption.AllDirectories))
					_ = Directory.CreateDirectory(dirPath.Replace(userProfileDir, tempDir));
				foreach (var newPath in Directory.GetFiles(userProfileDir, "*.*", SearchOption.AllDirectories))
					File.Copy(newPath, newPath.Replace(userProfileDir, tempDir), true);
			}

			var launchOptions = new BrowserTypeLaunchPersistentContextOptions
			{
				Headless = true, 
				Args = ["--allow-downgrade"], 
				Proxy = playwrightProxy,
			};
			
			var executablePath = await Chameleon.lib.Playwright.Services.Util.GetBrowseExecutablePath(browserType);
			if (!string.IsNullOrEmpty(executablePath) && File.Exists(executablePath))
			{
				launchOptions.ExecutablePath = executablePath;
			}
			else
			{
				Toaster.Info($"Browser executable not found for {browserType}. Using default Playwright browser.");
			}

			await using var context = await pwBrowser.LaunchPersistentContextAsync(tempDir, launchOptions);
			await context.AddCookiesAsync(cookies.Select(c => new Microsoft.Playwright.Cookie
			{
				Name = c.Name,
				Value = c.Value,
				Domain = c.Domain,
				Path = c.Path,
				Expires = (float?)c.Expires,
				HttpOnly = c.HttpOnly,
				Secure = c.Secure,
				SameSite = Enum.TryParse<SameSiteAttribute>(c.SameSite, true, out var sameSiteEnum) ? sameSiteEnum : SameSiteAttribute.Lax
			}));
			await context.CloseAsync();

			string sourceCookiesFilePath;
			string targetCookiesFilePath;

			if (browserType == SystemBrowserType.Firefox)
			{
				sourceCookiesFilePath = Path.Combine(tempDir, "cookies.sqlite");
				targetCookiesFilePath = Path.Combine(userProfileDir, "cookies.sqlite");
			}
			else
			{
				sourceCookiesFilePath = Path.Combine(tempDir, "Default", "Cookies");
				targetCookiesFilePath = Path.Combine(userProfileDir, "Default", "Cookies");
			}

			if (File.Exists(sourceCookiesFilePath))
			{
				try
				{
					_ = Directory.CreateDirectory(Path.GetDirectoryName(targetCookiesFilePath)!);
					File.Copy(sourceCookiesFilePath, targetCookiesFilePath, true);
					Toaster.Success($"Successfully imported cookies to {browserType} profile. Restart browser if running.");
				}
				catch (IOException ioEx)
				{
					Toaster.Error($"Could not copy cookies file to profile (browser might be running or file locked): {ioEx.Message}. Cookies were set in a temporary session only.");
				}
			}
			else
			{
				Toaster.Info($"Cookies file not found in temporary session for {browserType}. Cookies might have been set in-memory or path is incorrect.");
			}
		}
		catch (Exception ex)
		{
			Toaster.Error($"Error setting cookies for {browserType}: {ex.Message}");
		}
		finally
		{
			try { if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true); }
			catch { /* Ignore cleanup errors */ }
		}
	}
}

public class Cookie
{
	public string Name { get; set; } = "";
	public string Value { get; set; } = "";
	public string Domain { get; set; } = "";
	public string Path { get; set; } = "";
	public double Expires { get; set; } = -1; // Unix timestamp in seconds. Playwright uses float.
	public bool HttpOnly { get; set; }
	public bool Secure { get; set; }
	public string SameSite { get; set; } = "None"; // "Lax", "Strict", "None"
}
