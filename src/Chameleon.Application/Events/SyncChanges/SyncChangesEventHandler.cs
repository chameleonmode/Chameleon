using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Windows;
using Chameleon.Prism.Events;

namespace Chameleon.Application.Events.SyncChanges
{
    public class SyncChangesEventHandler
        : ISyncChangesEventHandler
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserProfileFolderService _userProfileFolderService;
        private readonly IUserProfileService _userProfileService;
        private readonly IToastNotificationService _toastNotificationService;

        public SyncChangesEventHandler(
            IEventAggregator eventAggregator,
            IUserProfileFolderService userProfileFolderService,
            IUserProfileService userProfileService,
            IToastNotificationService toastNotificationService)
        {
            _eventAggregator = eventAggregator;
            _userProfileFolderService = userProfileFolderService;
            _userProfileService = userProfileService;
            _toastNotificationService = toastNotificationService;

            _eventAggregator
                .GetEvent<SyncChangesEvent>()
                .Subscribe(OnSyncChanges);
        }

        private void OnSyncChanges()
        {        
            _userProfileService.Sync();
            _userProfileFolderService.Sync();

            _eventAggregator
                .GetEvent<UpdateStaleDataEvent>()
                .Publish();

            _toastNotificationService.ShowSuccess("Synchronization is completed");
        }
    }
}
