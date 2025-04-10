using Avalonia.Collections;

using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.lib;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.CommunityToolkit.MvvM;

using Chameleon.client.Features.Automation.Playwright.ViewModels;
using Chameleon.lib.Util;
using Chameleon.client.Features.Automation.Playwright.Models;

namespace Chameleon.client.Features.Automation.Playwright;
public partial class PlaywrightViewModel : ViewModelObjectBase {
	readonly SemaphoreSlim semaphore = new(1, 1);
	FileSystemWatcher? watcher;

	public AvaloniaList<ScriptViewModel> UserScripts { get; } = [];
	public AvaloniaList<ScriptViewModel> BundlesScripts { get; } = [];

	[ObservableProperty]
	private ScriptViewModel? selectedBundledScript;
	[ObservableProperty]
	private int totalCount;
	[ObservableProperty]
	private string userScriptsDirectory = "";

	public PlaywrightViewModel() : base("Playwright AIR") {
		BundlesScripts.AddMapped(
			BundledScriptsService.Instance.GetBundledScrits(),
			script => {
				var data = IoC.GetJsonValue<Dictionary<string, string>>(script.BundledScript!.TableName);
				var options = script.BundledScript!.Parameters
					.Select(p => new ScriptParametersValues(p.Key, data?.GetValueOrDefault(p.Key) ?? p.Value))
					.ToList();
				var viewModel = new ScriptViewModel(script, options);
				viewModel.OnOpenEdit += title => {
					SelectedBundledScript = BundlesScripts.FirstOrDefault(s => s.Title == title);
				};
				return viewModel;
			});

		SelectedBundledScript = BundlesScripts[0];
		AsyncCommandMap["Save"] = Save;
		AsyncCommandMap["SelectUserScriptFolder"] = SelectUserScriptFolder;
	}

	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
		if (!Loaded) {
			await InitializeUserScripts();
		}
	}

	Task Save() => Task.Run(() => {
		if (SelectedBundledScript?.Parameters == null) {
			return;
		}
		// Convert parameters to dictionary as in your original code
		var data = SelectedBundledScript.Parameters.ToDictionary(p => p.Key, p => p.Value);
		var table = SelectedBundledScript.RunOptions.BundledScript!.TableName;
		IoC.SetJsonValue(data, table);

		// TODO: Save the data to the database
	});

	async Task SelectUserScriptFolder() {
		var dialog = App.StorageProvider;
		var selected = await dialog.OpenFolderPickerAsync(new() { AllowMultiple = false });
		if (selected == null || selected.Count == 0) {
			return;
		}

		if (selected[0]?.Path?.AbsolutePath != null)
			IoC.Instance.Config?.SetValue("UserScriptsDirectory", selected[0]?.Path?.AbsolutePath);

		await InitializeUserScripts();
	}

	private async Task InitializeUserScripts() {
		await semaphore.WaitAsync();
		try {
			async void OnChanged(object sender, FileSystemEventArgs e) =>
				await InitializeUserScripts();

			UserScriptsDirectory = IoC.GetValue<string>("UserScriptsDirectory") ?? "";
			if (!Directory.Exists(UserScriptsDirectory)) return;

			if (watcher == null) {
				watcher = new(UserScriptsDirectory) {
					NotifyFilter = NotifyFilters.Attributes
												| NotifyFilters.CreationTime
												| NotifyFilters.DirectoryName
												| NotifyFilters.FileName
												| NotifyFilters.LastAccess
												| NotifyFilters.LastWrite
												| NotifyFilters.Security
												| NotifyFilters.Size,
					Filter = "*.js",
					EnableRaisingEvents = true
				};

				watcher.Changed += OnChanged;
				watcher.Deleted += OnChanged;
				watcher.Renamed += OnChanged;
				watcher.Created += OnChanged;
			}

			UserScripts.UpdateMapped(
				await BundledScriptsService.GetUserScripts(UserScriptsDirectory), s => new(s, []), (x, y) => x.Filepath == y.Description!.FilePath
			);
			await Task.Delay(250);
		} finally {
			_ = semaphore.Release();
		}
	}
}

