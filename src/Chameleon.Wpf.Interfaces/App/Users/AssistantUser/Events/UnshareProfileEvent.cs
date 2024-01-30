using Chameleon.Interfaces.Assistants;

using System;

namespace Chameleon.Interfaces.App.Assistants.Events
{
    public class UnshareProfileEventArgs 
        : EventArgs
    {
        public IAssistantProfile AssistantProfile { get; }

        public UnshareProfileEventArgs(
            IAssistantProfile assistantProfile)
        {
            AssistantProfile = assistantProfile;  
        }
    }
    public class UnshareProfileEvent
        : PubSubEvent<UnshareProfileEventArgs>
    {
    }
}
