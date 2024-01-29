using Prism.Events;
using System;

namespace Chameleon.Interfaces.OutReach
{
    public class OpenChangeProxiesEvent
        : PubSubEvent<OpenChangeProxiesEventArgs>
    {
    }

    public class OpenChangeProxiesEventArgs
        : EventArgs
    {
        public int FolderId { get; private set; }

        public OpenChangeProxiesEventArgs(int folderId)
        {
            FolderId = folderId;
        }
    }
}
