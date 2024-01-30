using Chameleon.Interfaces.Auth;
using System;

namespace Chameleon.Interfaces.Common
{
    public class RestrictContentEventArgs : EventArgs
    {
        public ILimits Limits { get; }
        public string[] Permissions { get;  }

        public RestrictContentEventArgs(ILimits limits, string[] permissions)
        {
            Limits = limits;
            Permissions = permissions;
        }
    }
}
