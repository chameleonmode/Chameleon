using Prism.Events;

namespace Chameleon.Interfaces.Repository
{
    public class InsertEntityEvent
        : PubSubEvent<EntityEventArgs>
    { }
}