using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Environments
{
    public interface IApplicationEnvironment : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        string ApplicationDataFolderPath { get; }
        string TempDataFolderPath { get; }
    }
}
