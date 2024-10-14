
using System;

using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Interfaces.UserProfileFolders {
	public class CreateUserProfileFolderEvent : PubSubEvent 
    { }

    public class AfterCreateOrRemoveFolderEvent : PubSubEvent
    {

    }

    public class RenameFolderEvent : PubSubEvent<RenameFolderEventArgs>
    {

    }

    public class RenameFolderEventArgs : EventArgs
    {
        public int FolderId { get; private set; }
        public string Title { get; private set; }

        public RenameFolderEventArgs(int folderId, string title)
        {
            FolderId = folderId;
            Title = title;
        }
    }
}