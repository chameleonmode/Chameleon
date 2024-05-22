namespace Chameleon.AppKitty.Interpops;

public static class WindowPops
{
    public static IntPtr GetWindowHandle(IntPtr pid)
    {
#if MACOS
        NSWindow nSWindow = new NSWindow(pid);
        nSWindow.DidUpdate += (s,e) => 
        {
        };
#endif

        return IntPtr.Zero;
    }
}
