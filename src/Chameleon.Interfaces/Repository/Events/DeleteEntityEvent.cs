

using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Interfaces.Repository {
	public class DeleteEntityEvent
        : PubSubEvent<EntityEventArgs>
    { }
}