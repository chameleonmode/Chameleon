using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Environments
{
    public interface IApplicationConfigurationManager : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        string Get(string key, string defaultValue = "");
        T Get<T>(string key, T defaultValue = default);
        void Set(string key, object value, bool save = true);
        void Save();
    }
}
