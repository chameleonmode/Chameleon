
using System;

namespace Chameleon.Interfaces.App.Assistants.Events
{
    public class DeletedUserAssistantEventArgs 
        : EventArgs
    {
        public long Id { get; }

        public DeletedUserAssistantEventArgs(long id)
        {
            Id = id;
        }
    }

    public class DeletedUserAssistantEvent 
        : PubSubEvent<DeletedUserAssistantEventArgs>
    {
    }
}
