using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Chameleon.Core.Util;

enum NSApplicationActivateOptions : uint
{
    ActivatingIgnoringOtherApps = 1 << 0
}

public static class MacOSUtil
{
    public static bool SetForegroundWindow(int pid)
    {
        try
        {
            // Get the list of all windows
            IntPtr windowListInfo = MacOSInterop.CGWindowListCopyWindowInfo(0x00000001, 0);
            if (windowListInfo == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get window list.");
                return false;
            }

            // Here you would filter windowListInfo by the PID to find your application's window
            // This step is simplified and would require additional implementation to parse windowListInfo
            // For demonstration, assume we have the window and its owner application's PID
            using (var windowList = new CFArray(windowListInfo))
            {
                for (int i = 0; i < windowList.Count; i++)
                {
                    var dict = new CFDictionary(windowList[i]);
                    if (dict.ContainsKey("kCGWindowOwnerPID"))
                    {
                        int windowProcessId = dict.GetInt32Value("kCGWindowOwnerPID");
                        if (windowProcessId == pid)
                        {
                            int windowId = dict.GetInt32Value("kCGWindowNumber");
                            // Set the window to the foreground
                            // Use NSRunningApplication to bring the application to the foreground
                            IntPtr nsRunningApplicationClass = 
                                    ObjectiveCRuntime.ObjCGetClass("NSRunningApplication");

                            IntPtr runningApp = 
                                ObjectiveCRuntime.ObjCMsgSend(
                                    nsRunningApplicationClass, 
                                    ObjectiveCRuntime.SelRegisterName("runningApplicationWithProcessIdentifier:"), 
                                    new IntPtr(pid));

                            if (runningApp != IntPtr.Zero)
                            {
                                ObjectiveCRuntime.ObjCMsgSend(runningApp, ObjectiveCRuntime.SelRegisterName("activateWithOptions:"), new IntPtr((int)NSApplicationActivateOptions.ActivatingIgnoringOtherApps));
                            }
                            else
                            {
                                Console.WriteLine("Failed to find running application with specified PID.");
                                return false;
                            }
                        }
                    }
                }
            }


            // Release the window list info object
            //MacOSInterop.CFRelease(windowListInfo);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to set foreground window: {ex.Message}");
        }

        return false;
    }
    // public static bool SetForegroundWindow(Process process)
    // {
    //     IntPtr windowInfoList = MacOSInterop.CGWindowListCopyWindowInfo(0x00000001, 0);

    //     if (windowInfoList != IntPtr.Zero)
    //     {
    //         using (var windowList = new CFArray(windowInfoList))
    //         {
    //             for (int i = 0; i < windowList.Count; i++)
    //             {
    //                 var dict = new CFDictionary(windowList[i]);
    //                 if (dict.ContainsKey("kCGWindowOwnerPID"))
    //                 {
    //                     int windowProcessId = dict.GetInt32Value("kCGWindowOwnerPID");
    //                     if (windowProcessId == process.Id)
    //                     {
    //                         int windowId = dict.GetInt32Value("kCGWindowNumber");
    //                         // Set the window to the foreground
    //                         MacOSInterop.CFRelease(windowInfoList);
    //                         BringWindowToFrontWithAppleScript(windowId);
    //                         return true;
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     IntPtr windowInfo = MacOSWindowManipulator.GetWindowInfo(process.Id);
    //     if (windowInfo != IntPtr.Zero)
    //     {
    //         IntPtr nsApplicationClass = ObjectiveCRuntime.ObjCGetClass("NSApplication");
    //         IntPtr sharedApplicationSelector = ObjectiveCRuntime.SelRegisterName("sharedApplication");
    //         IntPtr sharedApplication = ObjectiveCRuntime.ObjCMsgSend(nsApplicationClass, sharedApplicationSelector, IntPtr.Zero);
    //         IntPtr activateIgnoringOtherAppsSelector = ObjectiveCRuntime.SelRegisterName("activateIgnoringOtherApps:");

    //         ObjectiveCRuntime.ObjCMsgSend(sharedApplication, activateIgnoringOtherAppsSelector, (IntPtr)1);
    //     }
    //     else
    //     {
    //         Console.WriteLine($"No window found for process {process.Id}");
    //     }

    //     return false;
    // }

    // public static void BringWindowToFrontWithAppleScript(int windowId)
    // {
    //     var script = $@"
    //         tell application ""System Events""
    //             set frontmost of the first process whose unix id is {windowId} to true
    //         end tell";
    //     var startInfo = new ProcessStartInfo
    //     {
    //         FileName = "/usr/bin/osascript",
    //         Arguments = $"-e \"{script}\"",
    //         RedirectStandardOutput = true,
    //         UseShellExecute = false,
    //         CreateNoWindow = true,
    //     };
    //     using (var process = Process.Start(startInfo))
    //     {
    //         process.WaitForExit();
    //     }
    // }

    // Define necessary macOS constants and types
    public const int kAXErrorSuccess = 0;
    public delegate void AXObserverCallback(IntPtr observer, IntPtr element, string notification, IntPtr refcon);

    // AX API functions
    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    public static extern int AXObserverCreate(int application, AXObserverCallback callback, out IntPtr observer);

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    public static extern int AXObserverAddNotification(IntPtr observer, IntPtr element, string notificationName, IntPtr refcon);

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    public static extern IntPtr CFRunLoopGetCurrent();

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    public static extern void CFRunLoopAddSource(IntPtr rl, IntPtr source, IntPtr mode);

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    public static extern IntPtr AXObserverGetRunLoopSource(IntPtr observer);

public static void SetupWindowChangeNotification(int pid)
{
    AXObserverCallback callback = WindowChangeCallback; // Ensure the callback delegate is kept alive
    IntPtr observer;
    int result = AXObserverCreate(pid, callback, out observer);

    if (result != kAXErrorSuccess)
    {
        Console.WriteLine($"Failed to create AXObserver: {result}");
        return;
    }

    IntPtr appElement = AXUIElementCreateApplication(pid);
    if (appElement == IntPtr.Zero)
    {
        Console.WriteLine("Failed to create AXUIElementRef for application");
        return;
    }

    result = AXObserverAddNotification(observer, appElement, "AXMainWindowChanged", IntPtr.Zero);
    if (result != kAXErrorSuccess)
    {
        Console.WriteLine($"Failed to add notification: {result}");
        return;
    }

   IntPtr runLoopSource = AXObserverGetRunLoopSource(observer);
    CFRunLoopAddSource(CFRunLoopGetCurrent(), runLoopSource, IntPtr.Zero);
}

    private static void WindowChangeCallback(IntPtr observer, IntPtr element, string notification, IntPtr refcon)
    {
        // This method is called when the main window changes
        Console.WriteLine("Window change detected");

        // Implement further logic here, e.g., checking the window ID
    }

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    private static extern IntPtr AXUIElementCreateApplication(int pid);
}


internal static partial class MacOSInterop
{
    // Import Quartz functions for window manipulation
    [LibraryImport("/System/Library/Frameworks/Quartz.framework/Quartz")]
    internal static partial IntPtr CGWindowListCopyWindowInfo(uint option, uint relativeToWindow);

    [LibraryImport("/System/Library/Frameworks/Quartz.framework/Quartz")]
    internal static partial void CFRelease(IntPtr cfRef);
}

public class ObjectiveCRuntime
{
    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
    public static extern IntPtr SelRegisterName(string name);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_getClass")]
    public static extern IntPtr ObjCGetClass(string name);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr ObjCMsgSend(IntPtr receiver, IntPtr selector, IntPtr arg);
}


internal partial class MacOSWindowManipulator
{
    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    internal static partial IntPtr CGWindowListCopyWindowInfo(uint option, uint relativeToWindow);

    public static IntPtr GetWindowInfo(int processId)
    {
        return CGWindowListCopyWindowInfo(0, (uint)processId);
    }
}

public partial class CFArray : IDisposable
{
    private IntPtr _array;

    public CFArray(IntPtr array)
    {
        _array = array;
    }

    public int Count => CFArrayGetCount(_array);

    public IntPtr this[int index] => CFArrayGetValueAtIndex(_array, index);

    public void Dispose()
    {
        if (_array != IntPtr.Zero)
        {
            MacOSInterop.CFRelease(_array);
            _array = IntPtr.Zero;
        }
    }

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    internal static partial int CFArrayGetCount(IntPtr array);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    internal static partial IntPtr CFArrayGetValueAtIndex(IntPtr array, int index);
}

public partial class CFDictionary
{
    private IntPtr _dict;

    public CFDictionary(IntPtr dict)
    {
        _dict = dict;
    }

    public bool ContainsKey(string key)
    {
        var cfKey = CFString.Create(key);
        return CFDictionaryContainsKey(_dict, cfKey);
    }

    public int GetInt32Value(string key)
    {
        var cfKey = CFString.Create(key);
        var value = CFDictionaryGetValue(_dict, cfKey);
        return CFNumber.ToInt32(value);
    }

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    internal static partial IntPtr CFDictionaryGetValue(IntPtr dict, IntPtr key);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CFDictionaryContainsKey(IntPtr dict, IntPtr key);
}

public static partial class CFString
{
    public static IntPtr Create(string str)
    {
        return CFStringCreateWithCString(IntPtr.Zero, str, 0x08000100); // kCFStringEncodingUTF8
    }

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr CFStringCreateWithCString(IntPtr alloc, string cStr, int encoding);
}

public static partial class CFNumber
{
    public static int ToInt32(IntPtr number)
    {
        if (number == IntPtr.Zero)
            throw new ArgumentNullException(nameof(number));

        if (!CFNumberGetValue(number, 9, out int value)) // kCFNumberIntType = 9
            throw new InvalidOperationException("Could not convert CFNumber to Int32.");

        return value;
    }

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CFNumberGetValue(IntPtr number, int theType, out int value);
}
