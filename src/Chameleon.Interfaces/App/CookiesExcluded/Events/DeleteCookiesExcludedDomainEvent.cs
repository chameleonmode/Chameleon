using Prism.Events;

namespace Chameleon.Interfaces.CookiesExcluded
{
    public class DeleteCookiesExcludedDomainEvent 
        : PubSubEvent<CookiesExcludedDomainEventArgs>
    { }
}
