using Abp.Dependency;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public interface IWebBrowserSettingsManager
        : ITransientDependency
    {
        int Insert(WebBrowserSetting entity);
    }
}
