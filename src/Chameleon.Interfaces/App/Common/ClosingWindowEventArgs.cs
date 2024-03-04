using System;
using System.Collections.Generic;
using System.Text;

namespace Chameleon.Interfaces.Common
{
    public class ClosingWindowEventArgs : EventArgs
    {
        public bool IsClose { get; }
        public ClosingWindowEventArgs(bool isClose)
        {
            IsClose = isClose;
        }
    }
}
