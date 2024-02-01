using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public class UpdateLimitCacheEventArgs
        : EventArgs
    {
        public int ProfileId { get; private set; }
        public double? LimitCache { get; private set; }

        public UpdateLimitCacheEventArgs(int profileId, double? limitCache)
        {
            ProfileId = profileId;
            LimitCache = limitCache;
        }
    }
}