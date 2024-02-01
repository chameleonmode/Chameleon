using System;

namespace Chameleon.Interfaces.App.UserProfileFolders.Events
{
    public class SetFavoriteFolderEventArgs : EventArgs
    {
        public int FolderId { get; }
        public SetFavoriteFolderEventArgs(int folderId)
        {
            FolderId = folderId;
        }
    }
}
