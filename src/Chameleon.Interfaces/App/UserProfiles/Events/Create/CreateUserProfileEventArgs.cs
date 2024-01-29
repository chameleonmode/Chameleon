using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class CreateUserProfileEventArgs : EventArgs
    {
        public int? FolderId { get; }

        public CreateUserProfileEventArgs(int? folderId = null)
        {
            FolderId = folderId;
        }
    }
}