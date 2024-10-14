using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.lib.Common.Interfaces.Sys;


namespace Chameleon.Interfaces.App.Users.AssistantUser.Events {
	public class UnshareFolderEvent
        : PubSubEvent<UnshareFolderEventArgs>
    {
    }

    public class UnshareFolderEventArgs
    {
        public IShareFolder AssistantFolder { get; }

        public UnshareFolderEventArgs(
            IShareFolder assistantFolder)
        {
            AssistantFolder = assistantFolder;
        }
    }
}
