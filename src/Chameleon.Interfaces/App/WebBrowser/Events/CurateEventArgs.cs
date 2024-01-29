using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public class CurateEventArgs : EventArgs
    {
        public int ProfileId { get; set; }
        public string SelectionText { get; set; }
        public string Url { get; set; }

    }
}
