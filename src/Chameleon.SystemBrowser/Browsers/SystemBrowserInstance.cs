using Chameleon.CT.Common.Models;
using Chameleon.lib.Common.Enums;
using Chameleon.lib.ThirdParty.GeoIp;

using Newtonsoft.Json.Linq;

using System.Net.WebSockets;
using System.Text;

namespace Chameleon.SystemBrowser.Common;

public abstract class SystemBrowserInstance(
		IEventAggregator eventAggregator,
		ISystemBrowserLaunchOptions options,
		IUserDefaultSettingsService userDefaultsSettingsService,
		string browserDataFolderPath,
		string browserExeFilePath)
		: ISystemBrowserInstance {
	public event Action<ISystemBrowserLaunchOptions>? OnProcessClosed;
	public event Action<ISystemBrowserLaunchOptions>? OnProcessOpenError;

	private Dictionary<string, ExtensionDirectory>? extensions;
	public Dictionary<string, ExtensionDirectory> ExtensionDirectories {
		get {
			extensions ??= new Dictionary<string, ExtensionDirectory>
				{
										{
												AddonsUtilv1.ChameleonAddon,
												new ExtensionDirectory(BrowserProfileAddonsDir, AddonsUtilv1.ChameleonAddon)
										},
										{
												AddonsUtilv1.GeoAddon,
												new ExtensionDirectory(BrowserProfileAddonsDir, AddonsUtilv1.GeoAddon + (BrowserType == SystemBrowserType.Firefox ? "v2" : "v3"))
										},
										{
												AddonsUtilv1.NavigatorAddon,
												new ExtensionDirectory(BrowserProfileAddonsDir,  AddonsUtilv1.NavigatorAddon)
										},
										{
											 AddonsUtilv1.ProxyAddonUtil,
												new ExtensionDirectory(BrowserProfileAddonsDir, AddonsUtilv1.ProxyAddonUtil)
										},
										{
												AddonsUtilv1.TimezoneAddon,
												new ExtensionDirectory(BrowserProfileAddonsDir, AddonsUtilv1.TimezoneAddon +  "v2")
										}
								};

			return extensions;
		}
	}

	private readonly IExtensionLoaderService? _extensionLoaderService = ContainerServiceHelper.Resolve<IExtensionLoaderService>();

	readonly string destinationExtentionsDirv2Base = Path.Combine(Path.GetTempPath(), "Chameleon");

	readonly string destinationExtentionsDirv2 = Path.Combine(Path.GetTempPath(), "Chameleon", Guid.NewGuid().ToString());
	public Dictionary<ExtensionType, string> ExtentionsDirv2 { get; } = [];

	public TaskCompletionSource<bool> OPtcs { get; } = new();
	protected abstract SystemBrowserType BrowserType { get; }

	string starturl = "";
	public string Starturl {
		get { return starturl; }
		private set {
			if (starturl != value) {
				starturl = value.Contains(ProxyAddonUtil.UrlSchemeEnd) ?
				value : $"{ProxyAddonUtil.HTTPSScheme}{value}";
			}
		}
	}
	public int Port { get; private set; }
	public Process? Brocess { get; set; }
	public IntPtr Handle { get; private set; } = IntPtr.Zero;

	public string BrowserExeFilePath =>
			browserExeFilePath;

	public string BrowserProfileFolderPath =>
			Path.Combine(browserDataFolderPath, BrowserType.ToString(), UserProfile.Id.ToString());


	public string GettmpFname => Guid.NewGuid().ToString() + ".zip";
	protected string BrowserExtensionsFolderPath =>
			Path.Combine(AddonsUtilv1.BrowserExtensionsRootFolderPath, BrowserType.ToString());

	public string BrowserProfileAddonsDir =>
			Path.Combine(BrowserProfileFolderPath, "Chameleon-addons");

	public IUserProfile UserProfile =>
			options.UserProfile;

	public bool IsRunning => Brocess?.HasExited == false;

	public static bool IsMao =>
			OperatingSystem.IsMacOS();

	public UserProfileSystemBrowserProcessEventArgs GetArgs =>
			new(UserProfile,
					Brocess,
					options.Url,
					options.SignIn);

	public bool HasProxyLogin =>
			HasProxy &&
			UserProfile.Proxy.UserName.HasAny() &&
			UserProfile.Proxy.Password.HasAny();

	public bool HasProxy =>
		UserProfile.Proxy?.CanUse == true &&
		UserProfile.Proxy.Host.HasAny();

	public virtual async Task Open()
	{
		if (Brocess is null || Handle == IntPtr.Zero) {
			Starturl = await userDefaultsSettingsService.GetRandomUrlAsync();
			Port = Netil.NextFreePort(9613);

			await EnsureProfileFolderCreated();
			await InitializeProfileFolder();
			await IOtil.DC(BrowserProfileAddonsDir);
			await InitializeExtensionPath();
			if (OPtcs.Task.IsCompleted)
				return;
			await StartProcess();
		}

		//MakeForeground();
	}

	public void MakeForeground()
	{
		if (Brocess != null) {
			if (!IsMao) {
				if (Handle == IntPtr.Zero)
					return;
#pragma warning disable CA1416 // Validate platform compatibility
				if (U32.IsWindow(Handle)) {
					if (U32til.BringWindowToForeground(Handle)) {
						eventAggregator.Pub<ForegroundUserSystemBrowserEvent>(GetArgs);
					}
				}
#pragma warning restore CA1416 // Validate platform compatibility
			} else {
				if (MacOSUtil.SetForegroundWindow(Brocess.Id)) {
					Brocess.Refresh();
					eventAggregator.Pub<ForegroundUserSystemBrowserEvent>(GetArgs);
				}
			}
		}
	}

	protected virtual async Task InitializeExtensionPath()
	{
		await IOtil.DC(destinationExtentionsDirv2Base);

		var theseOptions = await BrowserDefaultLaunchSettings.Instance();
		var settingsBuilder = new StringBuilder();

		_ = settingsBuilder.AppendLine("const initIt = () => {");
		if (theseOptions.Options.AutoTimezone && UserProfile.Proxy != null && UserProfile.Proxy.Server.HasAny()) {
			try {
				var ipapi = await GeoIpApi.GetIpapi(UserProfile.Proxy.ServerForRequest, ToasterHelper.ShowErr,
						UserProfile.Proxy.UserName, UserProfile.Proxy.Password).ConfigureAwait(false);
				if (ipapi != null) {
					_ = settingsBuilder.AppendLine(
$@"
	chrome.storage.local.set({{
	  timezone: '{ipapi.timezone}',
	  random: false,
	  update: false
	}}, () => {{
		OnLoad();
	}});
");
				}
			} catch (Exception ex) {
				ToasterHelper.ShowErr($"Request for timezone failed {UserProfile.Proxy.Server} - {ex.Message}");
				OnProcessOpenError?.Invoke((ISystemBrowserLaunchOptions)theseOptions);
				eventAggregator.Pub<OpenedUserSystemBrowserErrorEvent>(GetArgs);
				Cleanup();
				return;
			}
		}else {
			_ = settingsBuilder.AppendLine("OnLoad();");
		}
		_ = settingsBuilder.AppendLine("};");
		_ = settingsBuilder.AppendLine("chrome.runtime.onInstalled.addListener(initIt);");
		_ = settingsBuilder.AppendLine("chrome.runtime.onStartup.addListener(initIt);");

		//TODO edit for ff
		//if (theseOptions.Options.SpoofClientRects)
		//    await AddonsUtilv1.LoadFromInternal(ExtensionDirectories[AddonsUtilv1.ClientRectsAddon]);

		HashSet<KeyValuePair<string, string>> options = 
		[
			new ("webglSpoofing", theseOptions.Options.SpoofWebGLFingerprint.ToLwrStr()),
			new ("canvasProtection", theseOptions.Options.SpoofCanvasFingerprint.ToLwrStr()),
			new ("clientRectsSpoofing", theseOptions.Options.SpoofClientRects.ToLwrStr()),
			new ("fontsSpoofing", theseOptions.Options.SpoofFontFingerprint.ToLwrStr()),
			new ("dAPI", theseOptions.Options.DisableWebRTC.ToLwrStr()),
			new ("geoSpoofing", theseOptions.Options.SpoofGeoLocation.ToLwrStr()),
			new ("timezoneSpoofing", theseOptions.Options.AutoTimezone.ToLwrStr())
		];
		_ = settingsBuilder.AppendLine("let settings = {");
		_ = settingsBuilder.AppendLine($"enabled: {options.Any(o => o.Value == "true").ToLwrStr()},");
		options.ForEach(o => settingsBuilder.AppendLine($"{o.Key}: {o.Value},"));
		_ = settingsBuilder.AppendLine("eMode: 'disable_non_proxied_udp',"); //isFirefox ? 'proxy_only'
		_ = settingsBuilder.AppendLine("dMode: 'default_public_interface_only',");
		_ = settingsBuilder.AppendLine("noiseLevel: 'medium',");
		_ = settingsBuilder.AppendLine("debug: 3");
		_ = settingsBuilder.AppendLine("};");
		ExtentionsDirv2.Add(ExtensionType.chromeleon_addon, settingsBuilder.ToString());

		if (BrowserType == SystemBrowserType.Chrome) {
			await NavigatorAddon.InitializeExtension(ExtensionDirectories[AddonsUtilv1.NavigatorAddon].AddonDir, browserSettings: await BrowserDefaultLaunchSettings.Instance());
		}

		var enabled = HasProxy ? "true" : "false";
		ExtentionsDirv2.Add(ExtensionType.chromeleon_auto_proxy, @$"
                let settings = {{
                    enabled: {enabled},
                    type: 'http',
                    host: '{UserProfile.Proxy?.Host}',
                    port: {UserProfile.Proxy?.Port},
                    username: '{UserProfile.Proxy?.UserName}',
                    password: '{UserProfile.Proxy?.Password}',
                    url: '{Starturl}',
                    debug: false,
                }};
            ");

		foreach (var (ext, setting) in ExtentionsDirv2) {
			await _extensionLoaderService!.LoadExtension(ext, destinationExtentionsDirv2, setting);
		}
	}

	protected virtual async Task StartProcess()
	{
		// var tcs = new TaskCompletionSource<string>();

		Brocess = ProUtil.Createa(BrowserExeFilePath, GetCommandLineArguments());
		Brocess.Start();

		if (IsMao) {
			Handle = Brocess.Handle;
			Brocess.Exited += (s, e) => { Cleanup(); };
			var tryCount = 0;
			while (Brocess?.HasExited == false &&
							MacOSUtil.FindWindowByPID(Brocess.Id) == null &&
							tryCount++ < 36)
				await Task.Delay(1000);

			if (Brocess?.Id is int id)
			 MacOSWindowListener.Instance.AddPid(id);

			MacOSWindowListener.Instance.WindowForegroundChanged += OnWindowForeground;
		} else {
#pragma warning disable CA1416 // Validate platform compatibility
			await Task.Delay(1800);

			if (BrowserType != SystemBrowserType.Firefox) {
				string? windowHandle = null;
				while (IsRunning) {
					windowHandle = await GetWebSocketDebuggerUrlAsync().ConfigureAwait(false);
					if (windowHandle.HasAny())
						break;

					await Task.Delay(250);
				}
				if (windowHandle?.HasAny() == false) {
					Cleanup();
					return;
				}

				await TaskUtil.AwaitFor(() => Brocess?.MainWindowHandle != IntPtr.Zero, 18);
				Handle = Brocess?.MainWindowHandle ?? IntPtr.Zero;
			} else {
				TaskCompletionSource<Process?> thisTcs = new();
				new Thread(() => {
					for (var i = 0; i < 18; i++) {
						ExUtil.TryCatch(() => {
							var currentProcesses = Process.GetProcessesByName("firefox");
							foreach (var p in currentProcesses) {
								if (Brocess != null && p.ParentProcessId() == Brocess.Id) {
									var childProcess = Process.GetProcessById(p.Id);
									if (childProcess?.HasExited == false) {
										var thishandle = U32til.FindMainWindowHandle(childProcess.Id);
										if (U32.IsWindow(thishandle)) {
											thisTcs.TrySetResult(childProcess);
											break;
										}
									}
								}
							}
						});
						if (Handle != IntPtr.Zero)
							break;
						Thread.Sleep(100);
					}
					if (Handle == IntPtr.Zero)
						thisTcs.TrySetResult(null);
				}).Start();
				Brocess = await thisTcs.Task;
				Handle = Brocess?.MainWindowHandle ?? IntPtr.Zero;
			}
#pragma warning restore CA1416 // Validate platform compatibility
		}

		if (Brocess?.HasExited == false)
			OPtcs.TrySetResult(true);
		else
			Cleanup();
	}

	void OnWindowForeground(int i)
	{
		if (i == Brocess?.Id)
			eventAggregator.Pub<ForegroundUserSystemBrowserEvent>(GetArgs);
	}

	public void Cleanup()
	{
		if (IsMao) {
			MacOSWindowListener.Instance.WindowForegroundChanged -= OnWindowForeground;
			if (Brocess?.Id is int id)
				MacOSWindowListener.Instance.RemPid(id);
		}

		var r = OPtcs.TrySetResult(false);
		Brocess = null;
		Handle = IntPtr.Zero;
		OnProcessClosed?.Invoke(options);
	}


	protected async Task EnsureProfileFolderCreated()
	{
		await IOtil.CreateDirectory(BrowserProfileFolderPath);
	}

	protected virtual Task InitializeProfileFolder()
	{
		return Task.CompletedTask;
	}

	protected virtual List<string> GetClearCommandLineArgumentsList()
	{
		// "--in-process-gpu","--disable-software-rasterizer",
		List<string> args =
				[
						"--disable-session-crashed-bubble",
								"--hide-crash-restore-bubble",
								"--restore-last-session",
								"--profile-directory=Default",
								"--ash-no-nudges",
								"--disable-domain-reliability",
								"--no-default-browser-check",
								"--no-first-run",
								"--disable-field-trial-config",
								$"--remote-debugging-port={Port}",
                //$"--window-name=\"{UserProfile.Title}\"",
            ];

		if (HasProxy) {
			args.Add($"--proxy-server={UserProfile.Proxy.ServerForRequest}");
		} else {
			args.Add("--no-proxy-server");
		}

		if (!UserProfile.WebBrowser.Tracking) {
			// not disable tracking totally, but disable for hyperlink
			args.Add("--disable-hyperlink-auditing");
		}

		return args;
	}

	protected virtual List<string> GetCommandLineArgumentsList()
	{
		var args = GetClearCommandLineArgumentsList();

		args.Add($"--user-data-dir=\"{BrowserProfileFolderPath}\"");

		return args;
	}

	protected virtual string GetCommandLineArguments()
	{
		var args = GetCommandLineArgumentsList();

		if (GetLoadExtensionsArgument().Get() is string exts)
			args.Add($"--load-extension=\"{exts}\"");

		args.Add($"about:blank");

		return string.Join(" ", args);
	}

	public virtual string GetLoadExtensionsArgument()
	{
		List<string> extsdestinationExtentionsDirv2 = [];
		if (Directory.Exists(destinationExtentionsDirv2)) {
			foreach (var item in Directory.GetDirectories(destinationExtentionsDirv2)) {
				extsdestinationExtentionsDirv2.Add(item);
			}
		}
		List<string> exts = new(extsdestinationExtentionsDirv2);
		foreach (var dir in ExtensionDirectories) {
			if (Directory.Exists(dir.Value.AddonDir))
				exts.Add(dir.Value.AddonDir);
		}

		if (Directory.Exists(BrowserExtensionsFolderPath))
			exts.AddRange(Directory.GetDirectories(BrowserExtensionsFolderPath));

		return exts.ToCommaSeparatedString();
	}


	private async Task<string?>? GetWebSocketDebuggerUrlAsync()
	{
		var url = $"http://localhost:{Port}/json";
		using var client = new HttpClient {
			Timeout = TimeSpan.FromSeconds(5) // Set a timeout of 5 seconds
		};

		try {
			var jsonResponse = await client.GetStringAsync(url);
			var targets = JArray.Parse(jsonResponse);

			foreach (JObject target in targets) {
				if (target["type"]?.ToString() == "page") // Assuming you want to debug a page
				{
					return target["webSocketDebuggerUrl"]?.ToString();
				}
			}

			return null; // No suitable debugger URL found
		} catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException) {
			// Handle timeout
			Console.WriteLine("The request timed out.");
			return null;
		} catch (HttpRequestException ex) {
			// Handle other HTTP request exceptions
			Console.WriteLine($"HttpRequestException: {ex.Message}");
			return null;
		} catch (Exception ex) {
			// Handle any other exceptions
			Console.WriteLine($"Exception: {ex.Message}");
			return null;
		}
	}

	//TODO implement
	//private async Task<IntPtr?>? GetWindowHandleAsync()
	//{
	//	var webSocketDebuggerUrl = await GetWebSocketDebuggerUrlAsync();
	//	using var webSocket = new ClientWebSocket();
	//	var uri = new Uri(webSocketDebuggerUrl);
	//	await webSocket.ConnectAsync(uri, CancellationToken.None);
	//	Console.WriteLine("Connected to WebSocket.");

	//	var command = "{\"id\": 1, \"method\": \"Browser.getWindowForTarget\"}";
	//	await SendCommandAsync(webSocket, command);

	//	var response = await ReceiveResponseAsync(webSocket);
	//	Console.WriteLine($"Received response: {response}");

	//	var jr = JObject.Parse(response);
	//	return jr["result"]?["windowId"]?.ToObject<int>();
	//}

	//private async Task<string> GetTargetIdAsync(ClientWebSocket webSocket)
	//{
	//	var command = "{\"id\": 2, \"method\": \"Target.getTargets\"}";
	//	await SendCommandAsync(webSocket, command);

	//	var response = await ReceiveResponseAsync(webSocket);
	//	Console.WriteLine($"Received response: {response}");

	//	var jr = JObject.Parse(response);
	//	return jr["result"]["targetInfos"]
	//			.FirstOrDefault(t => t["type"].ToString() == "page")?["targetId"]
	//			.ToString();
	//}

	//private async Task AttachToTargetAsync(ClientWebSocket webSocket, string targetId)
	//{
	//	var command = $"{{\"id\": 3, \"method\": \"Target.attachToTarget\", \"params\": {{\"targetId\": \"{targetId}\"}}}}";
	//	await SendCommandAsync(webSocket, command);

	//	var response = await ReceiveResponseAsync(webSocket);
	//	Console.WriteLine($"Received response: {response}");
	//}

	//private async Task<int> GetProcessIdAsync(ClientWebSocket webSocket, string targetId)
	//{
	//	var command = $"{{\"id\": 4, \"method\": \"Target.sendMessageToTarget\", \"params\": {{\"targetId\": \"{targetId}\", \"message\": \"{{\\\"id\\\": 5, \\\"method\\\": \\\"Browser.getWindowForTarget\\\"}}\"}}}}";
	//	await SendCommandAsync(webSocket, command);

	//	var response = await ReceiveResponseAsync(webSocket);
	//	Console.WriteLine($"Received response: {response}");

	//	var jr = JObject.Parse(response);
	//	return jr["result"]["processId"].ToObject<int>();
	//}

	private async Task SendCommandAsync(ClientWebSocket webSocket, string command)
	{
		var bytesToSend = Encoding.UTF8.GetBytes(command);
		await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);
	}

	private async Task<string> ReceiveResponseAsync(ClientWebSocket webSocket)
	{
		var responseBuilder = new StringBuilder();
		var buffer = new byte[1024];
		WebSocketReceiveResult result;

		do {
			result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			responseBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
		} while (!result.EndOfMessage);

		return responseBuilder.ToString();
	}
}