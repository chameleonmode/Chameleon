using Chameleon.CT.Common.Base;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public class ProfileFolderViewModel
    : ObservableObject
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
