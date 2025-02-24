using Avalonia.Collections;

using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.lib;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.CommunityToolkit.MvvM;

using Chameleon.app.Avalonia.Extensions;
using Chameleon.app.Features.Automation.Playwright.ViewModels;

namespace Chameleon.app.Features.Automation.Playwright;
public partial class PlaywrightViewModel : ViewModelObjectBase {
	readonly PlaywrightScriptRepository repository;
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

	public PlaywrightViewModel() : base("Playwright Taskforce") {
		repository = PlaywrightScriptRepository.Instance;

		BundlesScripts.AddMapped(repository.GetBundledScrits(), o => {
			var viewModel = new ScriptViewModel(o);
			if (o.Description?.Parameters != null)
				viewModel.Parameters.AddRange(o.Description.Parameters);
			viewModel.OnOpenEdit += scriptTitle => {
				SelectedBundledScript = BundlesScripts.FirstOrDefault(s => s.Title == scriptTitle);
			};

			return viewModel;
		});

		SelectedBundledScript = BundlesScripts.FirstOrDefault();
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
		foreach (var param in SelectedBundledScript?.Parameters!) {
			if (IoC.GetValue(SelectedBundledScript.Title!, param.Key!) != param.Value)
				IoC.SetValue(param.Value, SelectedBundledScript.Title!, param.Key!);
		}
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
			if (!Directory.Exists(UserScriptsDirectory)) {
				return;
			}

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
				await repository.GetUserScripts(UserScriptsDirectory),
			 	s => new(s),
				 (x, y) => x.Filepath == y.Description!.FilePath
			);
			await Task.Delay(250);
		} finally {
			_ = semaphore.Release();
		}
	}
}

