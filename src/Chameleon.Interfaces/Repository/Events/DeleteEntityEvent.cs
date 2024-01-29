using Prism.Events;

namespace Chameleon.Interfaces.Repository
{
    public class DeleteEntityEvent
        : PubSubEvent<EntityEventArgs>
    { }
}