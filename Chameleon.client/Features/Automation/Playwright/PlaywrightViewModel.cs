using Avalonia.Collections;

using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.lib;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.CommunityToolkit.MvvM;

using Chameleon.client.Features.Automation.Playwright.ViewModels;
using Chameleon.lib.Util;
using Chameleon.lib.Storage;
using System.Data;

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

	public PlaywrightViewModel() : base("Playwright AIR Configurations") {
		BundlesScripts.AddMapped(BundledScriptsService.Instance.GetBundledScrits(), o => {
			var viewModel = new ScriptViewModel(o, [.. o.Description!.Parameters.Select(p => new ScriptParametersValues( Key: p.Key, Value: p.Value ))]);
			viewModel.OnOpenEdit += scriptTitle => {
				var tableName = viewModel.TableName;
				Console.WriteLine($"\nParameters saved in table '{viewModel}':");
        
        var parametersTable = SqliteStorageService.Instance.Query($"SELECT * FROM {tableName}");
        
        if (parametersTable.Rows.Count == 0)
        {
            Console.WriteLine("No parameters found.");
            return;
        }
        
        // Calculate column widths for display
        var keyWidth = parametersTable.Columns.Cast<DataColumn>()
            .Max(col => col.ColumnName.Length) + 2;
        
        //var valueWidth = 20; // Default value width
        
        // Print header
        foreach (DataColumn column in parametersTable.Columns)
        {
            Console.Write($"{column.ColumnName.PadRight(keyWidth)}");
        }
        Console.WriteLine();
        Console.WriteLine(new string('-', parametersTable.Columns.Count * keyWidth));
        
        // Print rows
        foreach (DataRow row in parametersTable.Rows)
        {
            foreach (DataColumn column in parametersTable.Columns)
            {
                Console.Write($"{row[column]?.ToString()?.PadRight(keyWidth)}");
            }
            Console.WriteLine();
        }
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
		if (SelectedBundledScript?.Parameters == null) {
			return;
		}
		// Convert parameters to dictionary as in your original code
		var data = SelectedBundledScript.Parameters.ToDictionary(p => p.Key, p => (object)p.Value);

		// Fixed version of your code - notice the logic inversion
		if (!SqliteStorageService.Instance.TableExists(SelectedBundledScript.TableName)) {
			// Table doesn't exist - create it first
			Console.WriteLine("Table doesn't exist, creating it...");
			SqliteStorageService.Instance.CreateTable(
					SelectedBundledScript.TableName,
					SelectedBundledScript.Parameters.ToDictionary(p => p.Key, p => "TEXT"),
					true
			);

			// Now insert the data
			var rowId = SqliteStorageService.Instance.Insert(SelectedBundledScript.TableName, data);
			Console.WriteLine($"Inserted new row with ID: {rowId}");
		} else {
			// Table exists - update existing data
			Console.WriteLine("Table exists, updating data...");
			var rowsAffected = SqliteStorageService.Instance.Update(SelectedBundledScript.TableName, data);
			Console.WriteLine($"Updated {rowsAffected} rows");

			// If no rows were affected by the update, we need to insert instead
			if (rowsAffected == 0) {
				Console.WriteLine("No rows updated, inserting new row...");
				var rowId = SqliteStorageService.Instance.Insert(SelectedBundledScript.TableName, data);
				Console.WriteLine($"Inserted new row with ID: {rowId}");
			}
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
				await BundledScriptsService.GetUserScripts(UserScriptsDirectory), s => new(s, []), (x, y) => x.Filepath == y.Description!.FilePath
			);
			await Task.Delay(250);
		} finally {
			_ = semaphore.Release();
		}
	}
}

