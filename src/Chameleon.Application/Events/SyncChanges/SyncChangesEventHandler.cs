using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Windows;
using Chameleon.lib.Common.Interfaces.Sys;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.Application.Events.SyncChanges {
	public class SyncChangesEventHandler
        : ISyncChangesEventHandler
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserProfileFolderService _userProfileFolderService;
        private readonly IUserProfileService _userProfileService;

        public SyncChangesEventHandler(
            IEventAggregator eventAggregator,
            IUserProfileFolderService userProfileFolderService,
            IUserProfileService userProfileService)
        {
            _eventAggregator = eventAggregator;
            _userProfileFolderService = userProfileFolderService;
            _userProfileService = userProfileService;

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

            Toaster.ShowSuccess("Synchronization is completed");
        }
    }
}
