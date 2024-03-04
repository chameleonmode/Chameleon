using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Chameleon.Core.Util;

public static class ProcessesUtil
{
    public static void GoToUrlDefault(string Url)
    {
        try
        {
            Process.Start(Url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {Url.Replace("&", "^&")}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", Url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", Url);
            }
            else
            {
                throw;
            }
        }
    }
}
