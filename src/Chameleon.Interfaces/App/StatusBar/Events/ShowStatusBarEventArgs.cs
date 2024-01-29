using System;

namespace Chameleon.Interfaces.App.StatusBar.Events
{
    public class ShowStatusBarEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int ProfileId { get; set; }
        public ShowStatusBarEventArgs(string message, int profileId)
        {
            Message = message;
            ProfileId = profileId;
        }
    }
}
