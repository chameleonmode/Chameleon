using Prism.Events;

namespace Chameleon.Interfaces.Repository
{
    public class SavedEntityEvent
        : PubSubEvent<EntityEventArgs>
    { }
}