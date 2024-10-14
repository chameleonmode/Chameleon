
using System;

using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Interfaces.OutReach {
	public class UnselectAllTemplateEvent
        : PubSubEvent<UnselectAllTemplateEventArgs>
    { }

    public class UnselectAllTemplateEventArgs : EventArgs
    {
        public int OutReachTemplateId { get; }
        public UnselectAllTemplateEventArgs(int outReachTemplateId)
        {
            OutReachTemplateId = outReachTemplateId;
        }
    }
}
