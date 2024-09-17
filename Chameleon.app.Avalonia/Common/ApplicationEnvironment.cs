using System.IO;

using Chameleon.lib.Common.Extensions;

namespace Chameleon.app.Avalonia.Common;
public class ApplicationEnvironment
{
    public const string ApplicationName = "Chameleon";
    public const string ApplicationFileName = "appsettings.json";

    public static string ApplicationDataFolderPath
    {
        get
        {
            var localApplicationData = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.ApplicationData
                );

            var applicationLocalFolder = Path
                .Combine(localApplicationData, ApplicationName)
                .EnsureDirectoryExists();

            return applicationLocalFolder;
        }
    }

    public static string LocalApplicationDataFolderPath
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

    public static string TempDataFolderPath
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

