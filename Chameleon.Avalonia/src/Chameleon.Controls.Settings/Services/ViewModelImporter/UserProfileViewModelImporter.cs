using Chameleon.Core.Extensions;
using Chameleon.Controls.ImportExport.ViewModels;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.UserProfiles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.Controls.ImportExport.Services
{
    public class UserProfileViewModelImporter : IUserProfileViewModelImporter
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileViewModelImporter(
            IUserProfileService userProfileService
            )
        {
            _userProfileService = userProfileService;
        }

        public Task ImportAsync(ImportColumnViewModels viewModels, int? folderId = null)
        {
            var columns = viewModels.Selected;
            var itemsCount = viewModels.ItemsCount;

            var profiles = ListExtensions.CreateNew<UserProfile>(itemsCount);
            if(folderId != null)
            {
                profiles.ForEach(profile => profile.FolderId = folderId);
            }

            foreach (var column in columns)
            {
                for (int i = 0; i < itemsCount; ++i)
                {
                    column.MapTo(profiles[i], i);
                }
            }

            return _userProfileService.ImportAsync(profiles.ConvertAll(profile => (IUserProfile)profile));
        }
    }
}
