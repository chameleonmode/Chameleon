using System;

namespace Chameleon.Interfaces.CookiesExcluded
{
    public class CookiesExcludedDomainEventArgs : EventArgs
    {
        public ICookiesExcludedDomain CookiesExcludedDomain { get; }

        public CookiesExcludedDomainEventArgs(
            ICookiesExcludedDomain cookiesExcludedDomain
            )
        {
            CookiesExcludedDomain = cookiesExcludedDomain;
        }

    }
}
