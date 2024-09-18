using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Extensions;
using System.Diagnostics;
using Chameleon.lib.Playwright.Interfaces;
using Chameleon.lib.Playwright.Models;
using Chameleon.Avalonia.Common.Collections;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.Core.Extensions;

namespace Chameleon.app.Avalonia.ViewModels.Playwright;

public partial class AutomationViewModel
			: PageViewModelBase {
	private readonly IPlaywrightScriptRepository repository;
	private FileSystemWatcher? watcher;
	private readonly SemaphoreSlim semaphore = new(1, 1);

	public AvList<AutomationScriptViewModel> UserScripts { get; } = [];
	public AvList<AutomationScriptViewModel> BundlesScripts { get; } = [];

	[ObservableProperty]
	private AutomationScriptViewModel _selectedBundledScript;

	[ObservableProperty]
	private int _totalCount;

	[ObservableProperty]
	private string _userScriptsDirectory = "";

	public AutomationViewModel(IPlaywrightScriptRepository repository) : base("Bundled Playwright Scripts")
	{
		this.repository = repository;

		AsyncCommandMap["Save"] = Save;
	}

	public Task Save() => Task.Run(() => {
		foreach (var param in SelectedBundledScript.Parameters) {
			IoC.SetValue($"{SelectedBundledScript.Title} {param.Key}", param.Value);
		}
	});

	[RelayCommand]
	private async Task SelectUserScriptFolder()
	{
		var dialog = ApplicationHelper.GetMainWindow()?.StorageProvider;
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
		BundlesScripts.AddMapped(repository.BundledScripts, b => {
			var d = new PlaywrightScriptDescription {
				Title = b.Value.Title,
				Description = b.Value.Description,
				Parameters = b.Value.parameters
						.Select(p => new PlaywrightDescriptionParam { Key = p, Value = IoC.GetValue<string>($"{b.Value.Title} {p}") ?? string.Empty })
						.ToList()
			};
			var o = new PlaywriteRunScriptOptions {
				BundledScript = b.Value,
				Description = d,
			};

			var vm = new AutomationScriptViewModel(o);
			vm.Parameters.AddRange(d.Parameters);
			vm.OnOpenEdit += (scriptTitle) => { 
				SelectedBundledScript = BundlesScripts.FirstOrDefault(s => s.Title == scriptTitle); 
			};
			return vm;
		});

		SelectedBundledScript = BundlesScripts.FirstOrDefault();
	}

	private async Task InitializeUserScripts()
	{
		await semaphore.WaitAsync();
		try {
			UserScriptsDirectory = IoC.GetValue<string>("UserScriptsDirectory") ?? "";

			if (!UserScriptsDirectory.Is() ||
				!Directory.Exists(UserScriptsDirectory))
				return;

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

			//UserScripts.Clear();
			//await Task.Delay(50);
			UserScripts.UpdateMapped(await repository.GetAll(UserScriptsDirectory), s => new(new PlaywriteRunScriptOptions {
				Description = s,
			}), (x, y) => x.Filepath == y.FilePath);
			await Task.Delay(250);
		} finally {
			semaphore.Release();
		}
	}

	private async void OnChanged(object sender, FileSystemEventArgs e)
	{
		Debug.WriteLine($"OnChanged: {e.ChangeType}");
		await InitializeUserScripts();
	}
	private async void OnRenamed(object sender, RenamedEventArgs e)
	{
		Debug.WriteLine($"Renamed:");
		Debug.WriteLine($"    Old: {e.OldFullPath}");
		Debug.WriteLine($"    New: {e.FullPath}");
		await InitializeUserScripts();
	}
}
