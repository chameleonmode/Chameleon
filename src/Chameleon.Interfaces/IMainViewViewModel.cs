using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces;

public class MainAppSearchItem
{
    public MainAppSearchItem() { }

    public MainAppSearchItem(string pageHeader, Type pageType)
    {
        Header = pageHeader;
        PageType = pageType;
    }

    public string Header { get; set; }

    public object ViewModel { get; set; }

    public string Namespace { get; set; }

    public Type PageType { get; set; }
}

public interface IMainViewViewModel : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
    void BuildSearchTerms(List<MainAppSearchItem> items);
}
