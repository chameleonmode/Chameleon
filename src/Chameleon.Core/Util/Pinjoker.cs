using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace Chameleon.Core.Util;

public static class Pinjoker
{
    public static bool MakeForeground(Process p, IntPtr h)
    {
        if (p != null)
        {
            if (!OperatingSystem.IsMacOS())
            {
                if (h == IntPtr.Zero)
                    return false;

                if (U32.IsWindow(h))
                {
                    U32.SetForegroundWindow(h);
                    U32.SetActiveWindow(h);
                }
            }
            else
            {
                if (MacOSUtil.SetForegroundWindow(p.Id))
                {
                    //Brocess.EnableRaisingEvents = false;
                    //Brocess.Exited -= OnProcessExited; 
                    p.Refresh();
                    //Brocess.Exited += OnProcessExited; 
                    //Brocess.EnableRaisingEvents = true;
                    //await Process.Start(BrowserExeFilePath, GetCommandLineArgumentsList()).WaitForExitAsync();
                }
            }

            return true;
        }

        return false;
    }

    public static async Task<Tuple<Process,IntPtr>> OnStartedProcess(Process process,IntPtr Handle, TaskCompletionSource<bool> optcs, Action onexit, Action OnForeground)
    {
        if (OperatingSystem.IsMacOS())
        {
            Handle = process.Handle;
            process.Exited += (s, e) => 
            {
                MacOSWindowListener.Instance.RemPid(process.Id);
                onexit(); 
            };
            int tryCount = 0;
            while (process?.HasExited == false &&
                    MacOSUtil.FindWindowByPID(process.Id) == null &&
                    tryCount++ < 36)
                await Task.Delay(1000);

            MacOSWindowListener.Instance.AddPid(process.Id);

            MacOSWindowListener.Instance.WindowForegroundChanged += (i) =>
            {
                if (i == process.Id)
                    OnForeground();
            };
        }
        else
        {
            MWHandleTrackerUtility windowTracker = new(process);
            var newHandle = await windowTracker.WaitForMainWindowHandleChangeAsync();
            process = newHandle.Item2;
            if (process == null)
            {
                optcs.TrySetResult(false);
            }
            else
            {
                Handle = process.MainWindowHandle;
            }

            //windowTracker.OnForegroundEvent += OnForeground;
            //windowTracker.OnExitEvent += onexit;
        }

        if (process?.HasExited == false)
            process.Refresh();

        return new Tuple<Process, nint>(process, Handle);
    }
}
