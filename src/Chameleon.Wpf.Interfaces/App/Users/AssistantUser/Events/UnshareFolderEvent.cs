using Chameleon.Interfaces.App.ShareFolders;


namespace Chameleon.Interfaces.App.Users.AssistantUser.Events
{
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
