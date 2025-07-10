
``` csharp
// @TODO - ActorViewModel Play
if (EditableSettings.EachProfile) foreach (var selection in selected) {
		foreach (var profile in profiles) {
			var browser = await ExecuteScriptAsync(selection, profile);
			await BrowserShutdown(browser);
		}
	}
else foreach (var profile in profiles) {
		IBrowserInstance? browser = null;
		foreach (var selection in EditableSettings.AsQue
		? [selected.ElementAt(selectionIndex++ >= selected.Count() ? selectionIndex = 0 : selectionIndex)] : selected) {
			browser = await ExecuteScriptAsync(selection, profile);
			if (EditableSettings.AsQue) await BrowserShutdown(browser);
		}
		if (!EditableSettings.AsQue) await BrowserShutdown(browser);
	}
foreach (var profile in profiles) {
	if (++selectionIndex >= selected.Count()) selectionIndex = 0;
	var selection = selected.ElementAt(selectionIndex);
	await ExecuteScriptAsync(selection, profile);
}
```

Chameleon.client/Features/Projects/Profiles/Identity/IdentityView.axaml
[nitpick] Using a magic string for the command parameter can lead to typos; consider defining a named constant or enum for Save to improve maintainability.

``` csharp
	async Task<Ipapi> Ipapi() {
		// Ipapi? ipapi = null;
		// var dir = Resources.Assert(
		// 	Settings.Cached, "geo"
		// );
		// var file = Path.Combine(dir, "ipapi.json");
		// if (
		// 	 File.Exists(file) && await File.ReadAllTextAsync(file) is { } json &&
		// 	 JSON.Parse<BrowserProxy>((ipapi = JSON.Parse<Ipapi>(json)).proxy) is { } proxy &&
		// 	 proxy.Host == Settings.Profile.Proxy.Host &&
		// 	 proxy.Port == Settings.Profile.Proxy.Port &&
		// 	 proxy.UserName == Settings.Profile.Proxy.UserName &&
		// 	 proxy.Password == Settings.Profile.Proxy.Password
		// ) {
		// 	Toaster.Info($"Using cached timezone/geo data for {Settings.Profile.Proxy.Host}");
		// 	return ipapi;
		// }
		// Toaster.Info($"Requesting timezone/geo data for {Settings.Profile.Proxy.WebProxy?.Address?.Host ?? "local"}");
		// ipapi = await GeoIpApi.GetIpapi(Settings.Profile.Proxy.WebProxy, e => Toaster.Error(e)) ?? new() {
		// 	timezone = "Pacific/Honolulu",
		// 	lat = 34.052235,
		// 	lon = -118.243683,
		// 	tzSystem = true
		// };
		// ipapi.proxy = JSON.Serialize(Settings.Profile.Proxy);
		// await File.WriteAllTextAsync(file, JSON.Serialize(ipapi));
		// return ipapi;
		return await Api.GeoIp(Settings.Profile.Proxy.WebProxy) ?? new() {
			timezone = "Pacific/Honolulu",
			lat = 34.052235,
			lon = -118.243683,
			tzSystem = true
		};
	}
```