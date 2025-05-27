using Avalonia.Controls;
using Avalonia.Data.Converters;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.client.Features.ProfilesAndFolders.Folders;
using System.Diagnostics;
using System.Globalization;

namespace Chameleon.client.Features.Automation.Actors.Dialogs;

public partial class ProfileSelectorView : UserControl
{
    public ProfileSelectorView()
    {
        InitializeComponent();
    }
}

public class FolderKeyToNameConverter : IValueConverter {
	private IEnumerable<ObsFolder>? Folders => FoldersViewModel.Instance?.Folders;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is long folderIdKey && Folders != null) {
			if (folderIdKey == 0)
			{
				return "Other Profiles";
			}

			var folder = Folders.FirstOrDefault(f => f.Dto?.id == folderIdKey);
			if (folder != null) {
				return folder.Title ?? "Unnamed Folder";
			} else {
				Debug.WriteLine($"[FolderKeyToNameConverter] Folder not found for Key: {folderIdKey}");
				return $"Folder ID: {folderIdKey}";
			}
		}
		Debug.WriteLine($"[FolderKeyToNameConverter] Input value is not a valid long key or Folders collection is null. Value: {value}");
		return Avalonia.Data.BindingOperations.DoNothing;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		throw new NotImplementedException();
	}
}