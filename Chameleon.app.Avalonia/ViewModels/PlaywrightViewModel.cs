using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.app.Avalonia.Models;
using Avalonia.Collections;
using Chameleon.app.Avalonia.Extensions;
using Chameleon.lib;
using Chameleon.lib.Playwright.Services;

namespace Chameleon.app.Avalonia.ViewModels;

public partial class PlaywrightViewModel
			: ViewModelObjectBase {
	private readonly PlaywrightScriptRepository repository;
	private FileSystemWatcher? watcher;
	private readonly SemaphoreSlim semaphore = new(1, 1);

	public AvaloniaList<PlaywrightScript> UserScripts { get; } = [];
	public AvaloniaList<PlaywrightScript> BundlesScripts { get; } = [];

	[ObservableProperty]
	private PlaywrightScript? _selectedBundledScript;

	[ObservableProperty]
	private int _totalCount;

	[ObservableProperty]
	private string _userScriptsDirectory = "";

	public PlaywrightViewModel() : base("Bundled Playwright Scripts")
	{
		repository = PlaywrightScriptRepository.Instance;
		AsyncCommandMap["Save"] = Save;
	}

	public Task Save() => Task.Run(() => {
		foreach (var param in SelectedBundledScript?.Parameters!) {
			if(IoC.GetValue(SelectedBundledScript.Title!, param.Key!) != param.Value)
				IoC.SetValue(param.Value, SelectedBundledScript.Title!, param.Key!);
		}
	});

	[RelayCommand]
	private async Task SelectUserScriptFolder()
	{
		var dialog = AppLayers.GetMainWindow()?.StorageProvider;
		if (dialog == null) {
			return;
		}
		var selected = await dialog.OpenFolderPickerAsync(new() { AllowMultiple = false });
		if (selected == null || selected.Count == 0) {
			return;
		}

		if (selected[0]?.Path?.AbsolutePath != null)
			IoC.Instance.Config?.SetValue("UserScriptsDirectory", selected[0]?.Path?.AbsolutePath);

		await InitializeUserScripts();
	}

	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);
		if (!Loaded) {
			Initialize();
			await InitializeUserScripts();
		}
	}

	private void Initialize()
	{
		BundlesScripts.Clear();

		BundlesScripts.AddMapped(repository.GetBundledScrits(), b => {
			var viewModel = new PlaywrightScript(b);
			viewModel.Parameters.AddRange(b.Description?.Parameters!);
			viewModel.OnOpenEdit += scriptTitle => {
				SelectedBundledScript = BundlesScripts.FirstOrDefault(s => s.Title == scriptTitle);
			};

			return viewModel;
		});

		SelectedBundledScript = BundlesScripts.FirstOrDefault();
	}

	private async Task InitializeUserScripts()
	{
		await semaphore.WaitAsync();
		try {
			UserScriptsDirectory = IoC.GetValue<string>("UserScriptsDirectory") ?? "";

			if (!UserScriptsDirectory.Is() ||
				!Directory.Exists(UserScriptsDirectory)) {
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
					Filter = "*.cs",
					EnableRaisingEvents = true
				};

				watcher.Changed += OnChanged;
				watcher.Deleted += OnChanged;
				watcher.Renamed += OnRenamed;
				watcher.Created += OnChanged;
			}

			UserScripts.UpdateMapped(await repository.GetUserScripts(UserScriptsDirectory), s => new(s), (x, y) => x.Filepath == y.Description!.FilePath);
			await Task.Delay(250);
		} finally {
			_ = semaphore.Release();
		}
	}

	private async void OnChanged(object sender, FileSystemEventArgs e)
	{
		await InitializeUserScripts();
	}
	private async void OnRenamed(object sender, RenamedEventArgs e)
	{
		await InitializeUserScripts();
	}
}
