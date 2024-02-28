using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Repository;
using Chameleon.Prism.Events;

namespace Chameleon.Application.Events
{
    public class EntityEventHandler : IEntityEventHandler
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IToastNotificationService _toastNotificationService;

        public EntityEventHandler(
            IEventAggregator eventAggregator,
            IToastNotificationService toastNotificationService
            )
        {
            _eventAggregator = eventAggregator;
            _toastNotificationService = toastNotificationService;

            _eventAggregator
                .GetEvent<DeleteEntityEvent>()
                .Subscribe(args => ShowSuccessMessage("Deleted Successfully"));

            _eventAggregator
                .GetEvent<InsertEntityEvent>()
                .Subscribe(args => ShowSuccessMessage("Created Successfully"));

            _eventAggregator
                .GetEvent<UpdateEntityEvent>()
                .Subscribe(args => ShowSuccessMessage("Updated Successfully"));

            _eventAggregator
                .GetEvent<SavedEntityEvent>()
                .Subscribe(args => ShowSuccessMessage("Saved Successfully"));
        }

        private void ShowSuccessMessage(string text)
        {
            _toastNotificationService.ShowSuccess(text);
        }
    }
}
