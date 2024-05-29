//using System.Runtime.InteropServices;
//using Windows.Win32;
//using static Chameleon.Common.WinApiBridge.User32;
//
//namespace Chameleon.Common.WinApiBridge;
//
//public static class PInjoker
//{
//    public static UnhookWinEventSafeHandle SetWinEventHook(User32Events eventMin, User32Events eventMax,
//        SafeHandle hmodWinEventProc,
//        Windows.Win32.UI.Accessibility.WINEVENTPROC lpfnWinEventProc,
//        uint idProcess,
//        uint idThread,
//        uint dwFlags)
//    {
//        return PInvoke.SetWinEventHook((uint)eventMin, (uint)eventMax, hmodWinEventProc, lpfnWinEventProc, idProcess, idThread, dwFlags);
//    }
//}
