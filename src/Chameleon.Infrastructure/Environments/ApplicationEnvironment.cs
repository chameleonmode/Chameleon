using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Environments;
using System.IO;

namespace Chameleon.Infrastructure.Environments
{
    public class ApplicationEnvironment : IApplicationEnvironment
    {
        public string ApplicationName => "Chameleon";

        public string ApplicationDataFolderPath
        {
            get
            {
                var localApplicationData = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.LocalApplicationData
                    );

                var applicationLocalFolder = Path
                    .Combine(localApplicationData, ApplicationName)
                    .EnsureDirectoryExists();
                return applicationLocalFolder;
            }
        }

        public string TempDataFolderPath
        {
            get
            {
                var tempDataFolderPath = Path
                    .Combine(Path.GetTempPath(), ApplicationName)
                    .EnsureDirectoryExists();
                return tempDataFolderPath;
            }
        }
    }
}
