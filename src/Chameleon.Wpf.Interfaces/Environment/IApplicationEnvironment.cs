using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Environments
{
    public interface IApplicationEnvironment : ISingletonDependency
    {
        string ApplicationDataFolderPath { get; }
        string TempDataFolderPath { get; }
    }
}
