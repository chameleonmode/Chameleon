using System;

namespace Chameleon.Interfaces.Settings
{
    public class ChangeSelectedTabIndexEventArgs
        : EventArgs
    {
        public int SelectedIndex { get; set; }
    }
}
