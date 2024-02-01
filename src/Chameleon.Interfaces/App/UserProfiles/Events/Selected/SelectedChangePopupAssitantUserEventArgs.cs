using Chameleon.Interfaces.App.Assistants;
using System;

namespace Chameleon.Interfaces.App.UserProfiles.Events.Selected
{
    public class SelectedChangePopupAssitantUserEventArgs 
        : EventArgs
    {
        public IUserAssistant UserAssistant { get; }
        public bool IsSelected { get; }
        public SelectedChangePopupAssitantUserEventArgs(
            IUserAssistant userAssistant
            , bool isSelected)
        {
            UserAssistant = userAssistant;
            IsSelected = isSelected;
        }
    }
}