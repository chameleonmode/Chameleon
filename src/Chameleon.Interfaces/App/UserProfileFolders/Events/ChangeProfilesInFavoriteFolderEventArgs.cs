using System;

namespace Chameleon.Interfaces.App.UserProfileFolders.Events
{
    public class ChangeProfilesInFavoriteFolderEventArgs : EventArgs
    {
        public int FolderId { get; }

        public ChangeProfilesInFavoriteFolderEventArgs(int folderId)
        {
            FolderId = folderId;
        }
    }
}
