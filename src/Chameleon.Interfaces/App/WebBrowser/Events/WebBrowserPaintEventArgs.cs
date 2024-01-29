using System;
using System.Windows;

namespace Chameleon.Interfaces.WebBrowser
{
    public class WebBrowserPaintEventArgs : EventArgs
    {
        public WebBrowserPaintEventArgs(bool isPopup, Rect dirtyRect, IntPtr buffer, int width, int height)
        {
            IsPopup = isPopup;
            DirtyRect = dirtyRect;
            Buffer = buffer;
            Width = width;
            Height = height;
        }

        //
        // Summary:
        //     Is the OnPaint call for a Popup or the Main View
        public bool IsPopup { get; }

        //
        // Summary:
        //     contains the set of rectangles in pixel coordinates that need to be repainted
        public Rect DirtyRect { get; }

        //
        // Summary:
        //     Pointer to the unmanaged buffer that holds the bitmap. The buffer shouldn't be
        //     accessed outside the scope of CefSharp.Wpf.ChromiumWebBrowser.Paint event. A
        //     copy should be taken as the buffer is reused internally and may potentialy be
        //     freed.
        //
        // Remarks:
        //     The bitmap will be width * height * 4 bytes in size and represents a BGRA image
        //     with an upper-left origin
        public IntPtr Buffer { get; }

        //
        // Summary:
        //     Width
        public int Width { get; }

        //
        // Summary:
        //     Height
        public int Height { get; }

        //
        // Summary:
        //     Gets or sets a value indicating whether the event is handled.
        public bool Handled { get; set; }
    }
}
