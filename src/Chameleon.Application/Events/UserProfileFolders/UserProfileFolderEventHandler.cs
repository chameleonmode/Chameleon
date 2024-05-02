using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using System.Linq;

namespace Chameleon.Application.Events
{
    public class UserProfileFolderEventHandler : IUserProfileFolderEventHandler
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserProfileFolderService _userProfileFolderService;
        private readonly IUserProfileService _userProfileService;

        public UserProfileFolderEventHandler(
            IUserProfileFolderService userProfileFolderService,
            IEventAggregator eventAggregator,
            IUserProfileService userProfileService
            )
        {
            _eventAggregator = eventAggregator;
            _userProfileFolderService = userProfileFolderService;
            _userProfileService = userProfileService;

            _eventAggregator
                .GetEvent<CreateUserProfileFolderEvent>()
                .Subscribe(CreateFolder);

            //_eventAggregator
            //   .GetEvent<DeleteUserProfileFolderEvent>()
            //   .Subscribe(args => DeleteFolder(args.UserProfileFolder));
        }

        public async Task DeleteFolder(IUserProfileFolder userProfileFolder)
        {
            var userProfiles = _userProfileService.GetAll()
                .Where(a => a.FolderId == userProfileFolder.Id)
                .ToList();

            foreach (var item in userProfiles)
            {
                item.FolderId = null;
                await Task.Run(()=> _userProfileService.Save(item));
            }

            _userProfileFolderService.Delete(userProfileFolder);

            _eventAggregator
                .GetEvent<AfterCreateOrRemoveFolderEvent>()
                .Publish();
        }

        private void CreateFolder()
        {
            var folder = _userProfileFolderService.Create();

            _eventAggregator
                .GetEvent<AfterCreateOrRemoveFolderEvent>()
                .Publish();

            _eventAggregator
                .GetEvent<OpenUserProfileFolderEvent>()
                .Publish(new UserProfileFolderEventArgs(folder));
        }
    }
}
