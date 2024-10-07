
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Repository;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.Prism.Events;

namespace Chameleon.Application.Events
{
    public class EntityEventHandler : IEntityEventHandler
    {
        private readonly IEventAggregator _eventAggregator;

        public EntityEventHandler(
            IEventAggregator eventAggregator
            )
        {
            _eventAggregator = eventAggregator;

            _eventAggregator
                .GetEvent<DeleteEntityEvent>()
                .Subscribe(args => Toaster.ShowSuccess("Deleted Successfully"));

            _eventAggregator
                .GetEvent<InsertEntityEvent>()
                .Subscribe(args => Toaster.ShowSuccess("Created Successfully"));

            _eventAggregator
                .GetEvent<UpdateEntityEvent>()
                .Subscribe(args => Toaster.ShowSuccess("Updated Successfully"));

            _eventAggregator
                .GetEvent<SavedEntityEvent>()
                .Subscribe(args => Toaster.ShowSuccess("Saved Successfully"));
        }
	}
}
