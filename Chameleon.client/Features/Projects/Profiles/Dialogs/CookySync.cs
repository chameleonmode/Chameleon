using System.Collections.ObjectModel;
using Chameleon.client.MvvM;
using Chameleon.lib.Browzio;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright;
using Chameleon.lib.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public partial class CookySyncDialog : OOVM {
  [ObservableProperty] AvailableBrowser intoBrowserOption;
  [ObservableProperty] AvailableBrowser fromBrowserOption;
  [ObservableProperty] CookieOp operation = CookieOp.Sync;
  [ObservableProperty] bool isSync = true;

  public ObservableCollection<AvailableBrowser> BrowserOptions { get; } = [];
  public IEnumerable<CookieOp> CookieOps { get; } = Enum.GetValues<CookieOp>();

  public CookySyncDialog() {
		BrowserOptions.AddRange(
			Browzio.Utilities.DetectBrowsers()
			.Where(b => b.Type != BrowserType.Vivaldi)
			.Select(b => new AvailableBrowser(b))
		);
    FromBrowserOption = BrowserOptions[0];
    IntoBrowserOption = BrowserOptions[1];
  }

	partial void OnOperationChanged(CookieOp value) => IsSync = value == CookieOp.Sync;

  public static async Task<CookySyncDialog?> Show() {
    return await MessageBox.Show<CookySyncView, CookySyncDialog>(new(
        Header: "Sync Cookies",
        SubHeader: $"Select browser(s) to sync cookies:",
        Symbas: Symbas.Sync
      ));
  }
}
