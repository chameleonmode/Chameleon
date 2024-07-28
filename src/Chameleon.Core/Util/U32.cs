using Chameleon.Interfaces.WebBrowser;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Chameleon.Core.Util;

/**
 * This is a subset of events from winuser.h.
 * See: https://docs.microsoft.com/en-us/windows/win32/winauto/event-constants
 */
public enum User32Events : uint
{
    EVENT_MIN = 0x00000001,
    EVENT_MAX = 0x7FFFFFFF,
    EVENT_SYSTEM_FOREGROUND = 0x0003,
    EVENT_SYSTEM_MENUSTART = 0x0004,
    EVENT_SYSTEM_MENUEND = 0x0005,
    EVENT_SYSTEM_MENUPOPUPSTART = 0x0006,
    EVENT_SYSTEM_MENUPOPUPEND = 0x0007,
    EVENT_SYSTEM_CAPTURESTART = 0x0008,
    EVENT_SYSTEM_CAPTUREEND = 0x0009,
    EVENT_SYSTEM_MOVESIZESTART = 0x000A,
    EVENT_SYSTEM_MOVESIZEEND = 0x000B,
    EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C,
    EVENT_SYSTEM_CONTEXTHELPEND = 0x000D,
    EVENT_SYSTEM_DRAGDROPSTART = 0x000E,
    EVENT_SYSTEM_DRAGDROPEND = 0x000F,
    EVENT_SYSTEM_DIALOGSTART = 0x0010,
    EVENT_SYSTEM_DIALOGEND = 0x0011,
    EVENT_SYSTEM_SCROLLINGSTART = 0x0012,
    EVENT_SYSTEM_SCROLLINGEND = 0x0013,
    EVENT_SYSTEM_SWITCHSTART = 0x0014,
    EVENT_SYSTEM_SWITCHEND = 0x0015,
    EVENT_SYSTEM_MINIMIZESTART = 0x0016,
    EVENT_SYSTEM_MINIMIZEEND = 0x0017,
    EVENT_SYSTEM_DESKTOPSWITCH = 0x0020,
    EVENT_SYSTEM_SWITCHER_APPGRABBED = 0x0024,
    EVENT_SYSTEM_SWITCHER_APPOVERTARGET = 0x0025,
    EVENT_SYSTEM_SWITCHER_APPDROPPED = 0x0026,
    EVENT_SYSTEM_SWITCHER_CANCELLED = 0x0027,
    EVENT_SYSTEM_IME_KEY_NOTIFICATION = 0x0029,
    EVENT_SYSTEM_END = 0x00FF,

    EVENT_OBJECT_IME_SHOW = 0x8027,
    EVENT_OBJECT_FOCUS = 0x8005,
    EVENT_OBJECT_DESTROY = 0x8001,
    EVENT_OBJECT_REORDER = 0x8004,
    EVENT_OBJECT_LOCATIONCHANGE = 0x800B,
    EVENT_OBJECT_NAMECHANGE = 0x800C,

    WINEVENT_OUTOFCONTEXT = 0x0000,
    WINEVENT_SKIPOWNTHREAD = 0x0001,
    WINEVENT_SKIPOWNPROCESS = 0x0002,
    WINEVENT_INCONTEXT = 0x0004
}

[SupportedOSPlatform("windows")]
public static partial class U32
{
    #region delegates
    public delegate IntPtr MouseHookHandler(
        int nCode, uint wParam, IntPtr lParam);

    public delegate bool MonitorEnumDelegate(
        IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    public delegate void WinEventDelegate(
        IntPtr hWinEventHook, User32Events eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    public delegate bool EnumWindowsProc(
        IntPtr hWnd, IntPtr lParam);
    #endregion

    [LibraryImport("user32.dll")]
    public static partial IntPtr SetWinEventHook(
        User32Events eventMin, User32Events eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnhookWinEvent(
        IntPtr hWinEventHook);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial IntPtr SetActiveWindow(
        IntPtr hWnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsWindow(
        IntPtr hWnd);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr FindWindow(
        string lpClassName, string lpWindowName);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial uint GetWindowThreadProcessId(
        IntPtr hWnd, out uint lpdwProcessId);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial IntPtr GetWindow(
        IntPtr hWnd, uint uCmd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetForegroundWindow(
        IntPtr hWnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumWindows(
        EnumWindowsProc lpEnumFunc, IntPtr lParam);

}

[SupportedOSPlatform("windows")]
public static class U32til
{
    public static IntPtr FindMainWindowHandle(int processId)
    {
        IntPtr foundWindow = IntPtr.Zero;
        U32.EnumWindows((hWnd, lParam) =>
        {
            U32.GetWindowThreadProcessId(hWnd, out uint windowProcessId);
            if (windowProcessId == processId)
            {
                foundWindow = hWnd;
                return false; // Stop enumeration
            }
            return true; // Continue enumeration
        }, IntPtr.Zero);
        return foundWindow;
    }
}

[SupportedOSPlatform("windows")]
public class MWHandleTrackerUtility(Process aprocess, SystemBrowserType systemBrowserType, CancellationTokenSource cts)
{
    private readonly List<int> _childProcessIds = [];

    private IntPtr _mainWindowHandle = IntPtr.Zero;
    private TaskCompletionSource<Tuple<IntPtr, Process>> _tcs = new();

    private Process _process =  aprocess ?? throw new ArgumentNullException(nameof(aprocess));
    public void StartTracking()
    {

        new Thread(() => TrackMainWindowHandle(cts.Token)) { IsBackground = true }.Start();
        //if (moniterChild)
        //{
        //    await Task.Delay(250);
        //    new Thread(() => MonitorChildProcesses(cts.Token)) { IsBackground = true }.Start();
        //}
    }

    private void TrackMainWindowHandle(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (_process.HasExited || _mainWindowHandle == IntPtr.Zero)
                {
                    if (systemBrowserType == SystemBrowserType.Firefox)
                    {
                        Thread.Sleep(500);
                        var currentProcesses = Process.GetProcessesByName(
                            systemBrowserType == SystemBrowserType.Firefox ? "firefox"
                            : systemBrowserType == SystemBrowserType.Chrome ? "chrome"
                            : "chrome").Where(p => p.Id != 0);
                        foreach (var p in currentProcesses)
                        {
                            if (!_childProcessIds.Contains(p.Id) && p.ParentProcessId() == _process.Id)
                            {
                                _childProcessIds.Add(p.Id);
                                var childProcess = Process.GetProcessById(p.Id);
                                if (childProcess != null && !childProcess.HasExited)
                                {
                                    _process = childProcess;
                                    break;
                                }
                            }
                        }
                    }
                }

                IntPtr handle = U32til.FindMainWindowHandle(_process.Id);
                if (handle != _mainWindowHandle && U32.IsWindow(handle))
                {
                    _mainWindowHandle = handle;
                    var tcs = _tcs;
                    _tcs = new();
                    tcs.SetResult(new(_mainWindowHandle, _process));
                    break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                _tcs.SetResult(new(0, null));
                break;
            }

            Thread.Sleep(1000);  // Poll every second
        }
    }

    private void MonitorChildProcesses(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var currentProcesses = Process.GetProcessesByName("firefox").Where(p => p.Id != 0);
                foreach (var process in currentProcesses)
                {
                    if (!_childProcessIds.Contains(process.Id) && process.ParentProcessId() == process.Id)
                    {
                        _childProcessIds.Add(process.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            
            Thread.Sleep(500);  // Poll every half seconds
        }
    }

    public void StopTracking()
    {
        cts.Cancel();
    } 
    
    public Task<Tuple<IntPtr, Process>> WaitForMainWindowHandleChangeAsync() => _tcs.Task;
}

/*
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowsPosition
    {
        public IntPtr hwnd;
        public IntPtr hwndInsertAfter;
        public int Left;
        public int Top;
        public int Width;
        public int Height;
        public int Flags;
    }
    */

// workaround LiteDB compatibility issue in RECT data structure
[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int X;
    public int Y;
    public POINT(int x, int y)
    {
        X = x;
        Y = y;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }

    public readonly int Height
    {
        get
        {
            return Bottom - Top;
        }
    }
    public readonly int Width
    {
        get
        {
            return Right - Left;
        }
    }

    public override readonly string ToString()
    {
        return string.Format("({0}, {1}), {2} x {3}", Left, Top, Width, Height);
    }
}
