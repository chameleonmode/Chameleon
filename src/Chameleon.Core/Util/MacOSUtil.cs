using System.Runtime.InteropServices;

namespace Chameleon.Core.Util;

public static class MacOSUtil
{
    public static bool SetForegroundWindow(int processId)
    {
        IntPtr windowInfoList = MacOSInterop.CGWindowListCopyWindowInfo(0x00000001, 0);

        if (windowInfoList != IntPtr.Zero)
        {
            using (var windowList = new CFArray(windowInfoList))
            {
                for (int i = 0; i < windowList.Count; i++)
                {
                    var dict = new CFDictionary(windowList[i]);
                    if (dict.ContainsKey("kCGWindowOwnerPID"))
                    {
                        int windowProcessId = dict.GetInt32Value("kCGWindowOwnerPID");
                        if (windowProcessId == processId)
                        {
                            int windowId = dict.GetInt32Value("kCGWindowNumber");

                            IntPtr nsApp = MacOSInterop.objc_msgSend(MacOSInterop.objc_getClass("NSApplication"), MacOSInterop.sel_registerName("sharedApplication"));
                            MacOSInterop.objc_msgSend(nsApp, MacOSInterop.sel_registerName("activateIgnoringOtherApps:"), IntPtr.Zero);

                            IntPtr nsWindow = MacOSInterop.objc_msgSend(MacOSInterop.objc_getClass("NSWindow"), MacOSInterop.sel_registerName("windowWithWindowNumber:"), new IntPtr(windowId));
                            MacOSInterop.objc_msgSend(nsWindow, MacOSInterop.sel_registerName("makeKeyAndOrderFront:"), nsApp);

                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}

internal static partial class MacOSInterop
{
    // Import Objective-C runtime functions
    [LibraryImport("/usr/lib/libobjc.A.dylib", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr objc_getClass(string className);

    [LibraryImport("/usr/lib/libobjc.A.dylib", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr sel_registerName(string selectorName);

    [LibraryImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static partial IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [LibraryImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static partial IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [LibraryImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static partial IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, int arg1);

    [LibraryImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static partial IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

    // Import Quartz functions for window manipulation
    [LibraryImport("/System/Library/Frameworks/Quartz.framework/Quartz")]
    internal static partial IntPtr CGWindowListCopyWindowInfo(uint option, uint relativeToWindow);

    [LibraryImport("/System/Library/Frameworks/Quartz.framework/Quartz")]
    internal static partial void CFRelease(IntPtr cfRef);

    //CoreFoundation
    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    internal static partial int CFArrayGetCount(IntPtr array);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    internal static partial IntPtr CFArrayGetValueAtIndex(IntPtr array, int index);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    internal static partial IntPtr CFDictionaryGetValue(IntPtr dict, IntPtr key);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CFDictionaryContainsKey(IntPtr dict, IntPtr key);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr CFStringCreateWithCString(IntPtr alloc, string cStr, int encoding);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CFNumberGetValue(IntPtr number, int theType, out int value);
}

public class CFArray : IDisposable
{
    private IntPtr _array;

    public CFArray(IntPtr array)
    {
        _array = array;
    }

    public int Count => MacOSInterop.CFArrayGetCount(_array);

    public IntPtr this[int index] => MacOSInterop.CFArrayGetValueAtIndex(_array, index);

    public void Dispose()
    {
        if (_array != IntPtr.Zero)
        {
            MacOSInterop.CFRelease(_array);
            _array = IntPtr.Zero;
        }
    }
}

public class CFDictionary
{
    private IntPtr _dict;

    public CFDictionary(IntPtr dict)
    {
        _dict = dict;
    }

    public bool ContainsKey(string key)
    {
        var cfKey = CFString.Create(key);
        return MacOSInterop.CFDictionaryContainsKey(_dict, cfKey);
    }

    public int GetInt32Value(string key)
    {
        var cfKey = CFString.Create(key);
        var value = MacOSInterop.CFDictionaryGetValue(_dict, cfKey);
        return CFNumber.ToInt32(value);
    }
}

public static class CFString
{
    public static IntPtr Create(string str)
    {
        return MacOSInterop.CFStringCreateWithCString(IntPtr.Zero, str, 0x08000100); // kCFStringEncodingUTF8
    }
}

public static class CFNumber
{
    public static int ToInt32(IntPtr number)
    {
        if (number == IntPtr.Zero)
            throw new ArgumentNullException(nameof(number));

        if (!MacOSInterop.CFNumberGetValue(number, 9, out int value)) // kCFNumberIntType = 9
            throw new InvalidOperationException("Could not convert CFNumber to Int32.");

        return value;
    }
}
