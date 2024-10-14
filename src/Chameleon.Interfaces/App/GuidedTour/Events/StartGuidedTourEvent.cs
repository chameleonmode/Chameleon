using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Interfaces.App.GuidedTour.Events {
	public class StartGuidedTourEventArgs 
        : EventArgs
    {
        public bool ValidateUser { get; }

        public StartGuidedTourEventArgs(bool validateUser)
        {
            ValidateUser = validateUser;
        }
    }

    public class StartGuidedTourEvent 
        : PubSubEvent<StartGuidedTourEventArgs>
    {
    }
}
