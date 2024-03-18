using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Controls.ImportExport.Models;
using Chameleon.Controls.ImportExport.Services;
using Chameleon.Controls.ImportExport.ViewModels;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.ImportExport.Views;
using Chameleon.Interfaces.UserProfileFolders;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.ImportExport;

public class ImportViewModel : SubViewModelBase,
    IImportViewModel
{
    private readonly IUserProfileViewModelImporter _viewModelImporter;
    private readonly IUserProfileFileSystemImporter _fileSystemImporter;
    private readonly IUserProfileFolderService _userProfileFolderService;

    public ImportViewModel(
        IUserProfileViewModelImporter viewModelImporter,
        IUserProfileFileSystemImporter fileSystemImporter,
        IImportColumnViewModels importColumnViewModels,
        IUserProfileFolderService userProfileFolderService
        )
    {
        _viewModelImporter = viewModelImporter;
        _fileSystemImporter = fileSystemImporter;
        _userProfileFolderService = userProfileFolderService;

        Folders = _userProfileFolderService.GetAll();
        Columns = (ImportColumnViewModels)importColumnViewModels;

        Folders.CollectionChanged += Folders_CollectionChanged;

        ImportCommand = new DelegateCommand(OnImport);
        DiscardCommand = new DelegateCommand(OnDiscard);
        OpenFileCommand = new DelegateCommand(OnOpenFile);
        RemoveColumnCommand = new DelegateCommand<ImportColumnViewModel>(OnRemoveColumn);
        SaveCommand = new DelegateCommand(OnSave).ObservesCanExecute(() => Columns.HasSelected);
    }

    private ObservableCollection<IUserProfileFolder> _displayFolders;
    public ObservableCollection<IUserProfileFolder> DisplayFolders
    {
        get => _displayFolders;
        set => SetProperty(ref _displayFolders, value);
    }
    private void Folders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        InitDisplayFolders();
    }

    private void InitDisplayFolders()
    {
        var selectFolderId = SelectedFolder?.Id ?? 0;
        ClearDisplayFolders();
        AddFolders();
        SelectFolder(selectFolderId);
    }

    private void ClearDisplayFolders()
    {
        if (DisplayFolders == null)
        {
            DisplayFolders = new ObservableCollection<IUserProfileFolder>();
            DisplayFolders.Add(new UserProfileFolder { Title = "All Profiles" });
        }
        else
        {
            for (var i = 1; i < DisplayFolders.Count; ++i)
            {
                DisplayFolders.RemoveAt(i);
            }
        }
    }

    private void AddFolders()
    {
        DisplayFolders.AddRange(Folders);
        RaisePropertyChanged(nameof(SelectedFolder));
    }

    private void SelectFolder(int id)
    {
        var folder = DisplayFolders.FirstOrDefault(a => a.Id == id);
        if (folder != null)
        {
            SelectedFolder = folder;
        }
        else
        {
            SelectedFolder = DisplayFolders[0];
        }
    }

    private IUserProfileFolders _folders;
    public IUserProfileFolders Folders
    {
        get => _folders;
        set
        {
            if (SetProperty(ref _folders, value))
            {
                InitDisplayFolders();
                RaisePropertyChanged(nameof(DisplayFolders));

            }
        }
    }

    private IUserProfileFolder _selectedFolder;
    public IUserProfileFolder SelectedFolder
    {
        get => _selectedFolder;
        set => SetProperty(ref _selectedFolder, value);
    }

    private void OnRemoveColumn(ImportColumnViewModel item)
    {
        Columns.Remove(item);
        RaisePropertyChanged(nameof(HasItems));
    }

    private string _filePath;
    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }

    private static char DefaultDelimiter = ',';
    private char _delimiter = DefaultDelimiter;
    public char Delimiter
    {
        get => _delimiter;
        set => SetProperty(ref _delimiter, value);
    }

    public DelegateCommand<ImportColumnViewModel> RemoveColumnCommand { get; }
    public ImportColumnViewModels Columns { get; }

    private const string FileDialogFilter = "Csv files|*.csv|Text files|*.txt";
    public DelegateCommand OpenFileCommand { get; }
    private void OnOpenFile()
    {
       // OpenFileDialog dialog = new OpenFileDialog
       // {
       //     Multiselect = false,
       //     Filter = FileDialogFilter
       // };
       //
       // if (dialog.ShowDialog().GetValueOrDefault())
       // {
       //     OnOpenFile(dialog.FileName);
       // }
    }

    private void OnOpenFile(string filePath)
    {
        FilePath = filePath;
        Columns.Clear();

        var rows = File.ReadAllLines(FilePath);
        for (int i = 0; i < rows.Length; ++i)
        {
            AddRowToImport(rows[i]);
        }
    }

    private void AddRowToImport(string row)
    {
        var columns = row.Split(Delimiter);
        for (int i = 0; i < columns.Length; ++i)
        {
            var columnValue = columns[i];
            var columnItemViewModel = new ImportColumnItemViewModel(columnValue);

            var columnViewModel = Columns.GetOrCreateAt(i);
            columnViewModel.AddItem(columnItemViewModel);
            RaisePropertyChanged(nameof(HasItems));
        }
    }

    public DelegateCommand DiscardCommand { get; }
    private void OnDiscard()
    {
        Delimiter = DefaultDelimiter;
        FilePath = string.Empty;
        Columns.Clear();
        RaisePropertyChanged(nameof(HasItems));
    }

    public DelegateCommand SaveCommand { get; }
    private void OnSave()
    {
        var folderId = SelectedFolder?.Id;
        folderId = folderId > 0 ? folderId : null;
        _viewModelImporter.ImportAsync(Columns, folderId);
    }

    public DelegateCommand ImportCommand { get; }
    private void OnImport()
    {
        DispatcherService.InvokeOnUiThread(() => _fileSystemImporter.ImportAsync());
    }

    public bool HasItems => Columns?.Count > 0;
}
