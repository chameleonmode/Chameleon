using System;

namespace Chameleon.Interfaces.OutReach
{
    public class OutReachTemplateEventArgs : EventArgs
    {
        public IOutReachTemplate OutReachTemplate { get; }

        public OutReachTemplateEventArgs(
            IOutReachTemplate outReachTemplate
            )
        {
            OutReachTemplate = outReachTemplate;
        }
    }
}
