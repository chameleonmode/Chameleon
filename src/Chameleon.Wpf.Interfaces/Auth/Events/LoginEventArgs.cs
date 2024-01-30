using System;

namespace Chameleon.Interfaces.Auth
{
    public class LoginEventArgs : EventArgs
    {
        public IAuthSession Session { get; }

        public LoginEventArgs(IAuthSession session)
        {
            Session = session;
        }
    }
}
