using System;

namespace Chameleon.Interfaces.App.Verification.Events
{
    public class OpenPhoneVerificationEventArgs
        : EventArgs
    {
        public int ProfileId { get; set; }

        public OpenPhoneVerificationEventArgs(int profileId)
        {
            ProfileId = profileId;
        }
    }
}
