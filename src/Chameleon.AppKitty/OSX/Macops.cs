using System.Runtime.InteropServices;

namespace Chameleon.AppKitty.OSX;

public static partial class Macops
{
    static event Action<bool> onFocusedCallback;

    // AXObserver callback signature
    public delegate void AXObserverCallback(IntPtr observer, IntPtr element, IntPtr notificationName, IntPtr contextData);

    // AXValueGetValue function
    [LibraryImport(Interop.Libraries.ApplicationServices, EntryPoint = "AXValueGetValue")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AXValueGetValue(IntPtr value, uint type, out CGPoint point);

    // AXObserverCreate function
    [LibraryImport(Interop.Libraries.ApplicationServices, EntryPoint = "AXObserverCreate")]
    public static partial int AXObserverCreate(int applicationPid, AXObserverCallback callback, out IntPtr observer);

    // AXObserverAddNotification function
    [LibraryImport(Interop.Libraries.ApplicationServices, EntryPoint = "AXObserverAddNotification")]
    public static partial int AXObserverAddNotification(IntPtr observer, IntPtr element, IntPtr notification, IntPtr context);

    // CFRunLoopAddSource function
    [LibraryImport(Interop.Libraries.ApplicationServices, EntryPoint = "CFRunLoopAddSource")]
    public static partial void CFRunLoopAddSource(IntPtr runLoop, IntPtr source, IntPtr mode);

    // AXUIElementCopyElementAtPosition function
    [LibraryImport(Interop.Libraries.ApplicationServices, EntryPoint = "AXUIElementCopyElementAtPosition")]
    public static partial int AXUIElementCopyElementAtPosition(IntPtr element, float x, float y, out IntPtr elementOut);

    // AXUIElementCreateApplication function
    [LibraryImport(Interop.Libraries.ApplicationServices, EntryPoint = "AXUIElementCreateApplication")]
    public static partial IntPtr AXUIElementCreateApplication(int pid);

    // Floors to point
    [StructLayout(LayoutKind.Sequential)]
    public struct CGPoint
    {
        public double x;
        public double y;
    }

    // Constants
    public static readonly IntPtr kCFRunLoopDefaultMode = CFSTR("kCFRunLoopDefaultMode");

    // CFString creation function
    [LibraryImport(Interop.Libraries.ApplicationServices, EntryPoint = "CFSTR")]
    public static partial IntPtr CFSTR([MarshalAs(UnmanagedType.LPWStr)] string strin);

    // Notification constants
    public static readonly IntPtr kAXFocusedWindowChangedNotification = CFSTR("AXFocusedWindowChanged");
    public static readonly IntPtr kAXFocusedUIElementChangedNotification = CFSTR("AXFocusedUIElementChanged");

    // Example callback to handle focus events
    public static void FocusChangedCallback(IntPtr observer, IntPtr element, IntPtr notificationName, IntPtr contextData)
    {
        if (notificationName == kAXFocusedWindowChangedNotification)
        {
            onFocusedCallback.Invoke(true);
            Console.WriteLine("Focused window changed!");
        }
        else if (notificationName == kAXFocusedUIElementChangedNotification)
        {
            Console.WriteLine("Focused UI element changed!");
        }
    }

    // Method to register the observer
    public static void RegisterFocusObserver(int pid, Action<bool> callback)
    {
        onFocusedCallback += callback;
        if (AXObserverCreate(pid, FocusChangedCallback, out IntPtr observer) == 0)
        {
            IntPtr appElement = AXUIElementCreateApplication(pid);

            if (AXObserverAddNotification(observer, appElement, kAXFocusedWindowChangedNotification, IntPtr.Zero) == 0)
            {
                CFRunLoopAddSource(CFRunLoopGetCurrent(), AXObserverGetRunLoopSource(observer), kCFRunLoopDefaultMode);
            }
        }
    }

    // Get and return the current CFRunLoop
    [DllImport(Interop.Libraries.ApplicationServices, EntryPoint = "CFRunLoopGetCurrent")]
    public static extern IntPtr CFRunLoopGetCurrent();

    // Get source from AXObserver
    [DllImport(Interop.Libraries.ApplicationServices, EntryPoint = "AXObserverGetRunLoopSource")]
    public static extern IntPtr AXObserverGetRunLoopSource(IntPtr observer);

    // Function to set active window (using AppKit framework)
    [LibraryImport(Interop.Libraries.AppKitLibrary, EntryPoint = "NSApp")]
    public static partial IntPtr NSApp();

    [LibraryImport(Interop.Libraries.AppKitLibrary, EntryPoint = "orderFront:")]
    [SuppressGCTransition]
    public static partial void OrderFront(IntPtr nsWindow);

    [LibraryImport(Interop.Libraries.ApplicationServices)]
    [SuppressGCTransition]
    public static partial int AXUIElementCopyAttributeValue(IntPtr element, IntPtr attribute, out IntPtr value);

    [DllImport(Interop.Libraries.CoreFoundationLibrary)]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public static extern IntPtr CFStringCreateWithCString(IntPtr allocator, [MarshalAs(UnmanagedType.LPWStr)] string str, int encoding);

    public static void SetActiveWindow(IntPtr nsWindow)
    {
        OrderFront(nsWindow);
    }

    // Function to check if a window is valid (using CoreFoundation framework)
    public static bool IsWindow(IntPtr nsWindow)
    {
        // Implement logic to validate window handle
        return nsWindow != IntPtr.Zero;
    }

    public static IntPtr GetMainWindow(int processId)
    {
        //if(kAXWindowsAttribute == IntPtr.Zero)
         kAXWindowsAttribute = CFStringCreateWithCString(IntPtr.Zero, "AXWindows", 0);
        IntPtr appElement = AXUIElementCreateApplication(processId);
        if (appElement == IntPtr.Zero)
        {
            Console.WriteLine("Failed to create AXUIElement for application.");
            return IntPtr.Zero;
        }

        IntPtr windowListPtr;
        int result = AXUIElementCopyAttributeValue(appElement, kAXWindowsAttribute, out windowListPtr);
        if (result != 0 || windowListPtr == IntPtr.Zero)
        {
            Console.WriteLine("Failed to copy attribute value for AXWindows.");
            return IntPtr.Zero;
        }

        // Normally, windowListPtr would be an array of AXUIElementRefs
        // Here we assume it is a CFArrayRef containing AXUIElementRefs
        // For simplicity, pick the first window
        IntPtr[] windowList = new IntPtr[1]; // Define a sufficient length array if multiple windows are expected
        Marshal.Copy(windowListPtr, windowList, 0, windowList.Length);
        
        if (windowList.Length > 0)
        {
            return windowList[0];
        }
        
        return IntPtr.Zero;
    }

    private static IntPtr kAXWindowsAttribute = IntPtr.Zero;
}


public static class WindowPops
{
    public static IntPtr GetWindowHandle(IntPtr pid)
    {
#if __MACOS__
        NSWindow nSWindow = new NSWindow(pid);
        nSWindow.DidUpdate += (s,e) => 
        {
        };
#endif

        return IntPtr.Zero;
    }
}
