using Chameleon.Avalonia.Prism.Module.Base;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public class ProfileFolderViewModel
    : ViewModelBase
{
    public ProfileFolderViewModel(int id, string title)
    {
        Id = id;
        Title = title;
    }

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}
