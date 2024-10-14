
using System;

using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Interfaces.UserProfiles {
	public class UserProxySetFolderIdEvent
        : PubSubEvent<UserProxySetFolderIdEventArgs>
    {

    }

    public class UserProxySetFolderIdEventArgs
        : EventArgs
    {
        public int FolderId { get; private set; }

        public UserProxySetFolderIdEventArgs(int folderId)
        {
            FolderId = folderId;
        }
    }
}
