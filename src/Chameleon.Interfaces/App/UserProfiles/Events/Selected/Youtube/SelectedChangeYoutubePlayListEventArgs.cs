using System;

namespace Chameleon.Interfaces.App.UserProfiles.Events.Selected.Youtube
{
    public class SelectedChangeYoutubePlayListEventArgs 
        : EventArgs
    {
        public SelectedChangeYoutubePlayListEventArgs(string id, bool isSelected)
        {
            Id = id;
            IsSelected = isSelected;
        }

        public string Id { get; }
        public bool IsSelected { get; }
    }
}
