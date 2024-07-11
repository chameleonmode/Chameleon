using Avalonia.Controls;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Av.Fluent.Common.Models;

public class MainPageModelBase
{
    public Type Tag { get; set; }

    public string? NavHeader { get; set; }

    public string? IconKey { get; set; }

    public bool ShowsInFooter { get; set; }
    public NavigationViewItemBase GetNavigationViewItemBase(UserControl c)
    {
        var nvi = new NavigationViewItem
        {
            Content = NavHeader,
            Tag = this,
            IconSource = (IconSource)c.FindResource(IconKey),
        };
        nvi.Classes.Add("SampleAppNav");
        return nvi;
    }
}
