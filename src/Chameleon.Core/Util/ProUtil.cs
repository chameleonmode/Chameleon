using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Chameleon.Core.Util;

public static class ProUtil
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

    public static Process GetChildProcess(int parentId)
    {
        return Process.GetProcessesByName("firefox").FirstOrDefault(p =>
        {
            try
            {
                return p.ParentProcessId() == parentId && p.MainWindowHandle != IntPtr.Zero;
            }
            catch
            {
                return false;
            }
        });
    }

    public static int ParentProcessId(this Process process)
    {
        var pbi = new Procvoke.PROCESS_BASIC_INFORMATION();
        int status = Procvoke.NtQueryInformationProcess(process.Handle, 0, ref pbi, (uint)Marshal.SizeOf(pbi), out _);
        if (status != 0)
        {
            throw new Exception("NtQueryInformationProcess failed");
        }
        return pbi.InheritedFromUniqueProcessId.ToInt32();
    }
}

// Extension method to get the parent process ID
public static partial class Procvoke
{
    public struct PROCESS_BASIC_INFORMATION
    {
        public IntPtr ExitStatus;
        public IntPtr PebBaseAddress;
        public IntPtr AffinityMask;
        public IntPtr BasePriority;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }

    [LibraryImport("ntdll.dll", SetLastError = true)]
    public static partial int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, uint processInformationLength, out uint returnLength);
}
