

namespace Chameleon.Interfaces.OutReach
{
    public class OutReachOpenEvent
        : PubSubEvent<OutReachEventArgs>
    { }

    public class DeleteOutReachRssEvent
        : PubSubEvent<OutReachRssEventArgs>
    {
    }
}
