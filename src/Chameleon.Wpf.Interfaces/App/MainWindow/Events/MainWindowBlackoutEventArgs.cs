using System;

namespace Chameleon.Interfaces.MainWindow
{
    public class MainWindowBlackoutEventArgs : EventArgs
    {
        public bool IsBlackout { get; set; }

        public MainWindowBlackoutEventArgs(bool isBlackout)
        {
            IsBlackout = isBlackout;
        }
    }
}
