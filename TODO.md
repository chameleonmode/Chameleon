
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