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
        return Process.GetProcesses().FirstOrDefault(p =>
        {
            try
            {
                return p.Id != 0 && p.ParentProcessId() == parentId;
            }
            catch
            {
                return false;
            }
        });
    }
}

// Extension method to get the parent process ID
public static partial class Procvoke
{
    [LibraryImport("ntdll.dll", SetLastError = true)]
    private static partial int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, uint processInformationLength, out uint returnLength);

    public static int ParentProcessId(this Process process)
    {
        var pbi = new PROCESS_BASIC_INFORMATION();
        int status = NtQueryInformationProcess(process.Handle, 0, ref pbi, (uint)Marshal.SizeOf(pbi), out _);
        if (status != 0)
        {
            throw new Exception("NtQueryInformationProcess failed with status: " + status);
        }
        return pbi.InheritedFromUniqueProcessId.ToInt32();
    }

    private struct PROCESS_BASIC_INFORMATION
    {
        public IntPtr ExitStatus;
        public IntPtr PebBaseAddress;
        public IntPtr AffinityMask;
        public IntPtr BasePriority;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }
}
