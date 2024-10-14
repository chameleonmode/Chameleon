

using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Interfaces.Repository {
	public class InsertEntityEvent
        : PubSubEvent<EntityEventArgs>
    { }
}