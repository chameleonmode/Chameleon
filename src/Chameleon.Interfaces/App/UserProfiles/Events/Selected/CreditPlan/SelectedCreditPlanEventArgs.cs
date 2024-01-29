
using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class SelectedCreditPlanEventArgs : EventArgs
    {
        public bool IsSelected { get; }
        public SelectedCreditPlanEventArgs(bool isSelected)
        {
            IsSelected = isSelected;
        }
    }
}
