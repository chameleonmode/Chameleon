using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.Assistants;
using System;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.Users
{
    public class UserAssistantService 
        : IUserAssistantService
    {
        private readonly IUserAssistantRepository _userAssistantRepository;
        private readonly IAssistantProfileRepository _assistantProfileRepository;
        private readonly IAssistantProfilePermissionRepository _assistantProfilePermissionRepository;

        public UserAssistantService(
            IUserAssistantRepository userAssistantRepository,
            IAssistantProfileRepository assistantProfileRepository,
            IAssistantProfilePermissionRepository assistantProfilePermissionRepository
            )
        {
            _userAssistantRepository = userAssistantRepository;
            _assistantProfileRepository = assistantProfileRepository;
            _assistantProfilePermissionRepository = assistantProfilePermissionRepository;
        }

        public ICollection<IUserAssistant> Get()
        {
            return _userAssistantRepository.GetAll();
        }

        public void Save(IUserAssistant userAssistant)
        {
            if (userAssistant == null)
            {
                throw new ArgumentNullException(nameof(userAssistant));
            }

            if (userAssistant.Id == 0)
            {
                _userAssistantRepository.Insert(userAssistant);
            }
            else
            {
                _userAssistantRepository.Update(userAssistant);
            }
        }

        public void DeleteAssistant(IUserAssistant userAssistant)
        {
            _userAssistantRepository.Delete(userAssistant.Id);
        }

        public IList<IAssistantProfile> GetAllAssistantProfilesById(long userAssistantId)
        {
            return _assistantProfileRepository.GetAllAssistantProfilesById(userAssistantId);
        }

        public void DeleteAssistantProfile(IAssistantProfile assistantProfile)
        {
            _assistantProfileRepository.DeleteAssistantProfile(assistantProfile);
        }

        public IList<IAssistantProfilePermission> GetAllProfilePermissions(long assistantId, int profileId)
        {
            return _assistantProfilePermissionRepository.GetAllProfilePermissions(assistantId, profileId);
        }

        public void UpdateProfilePermission(IAssistantProfilePermission assistantProfilePermission)
        {
            if (assistantProfilePermission == null)
            {
                throw new ArgumentNullException(nameof(assistantProfilePermission));
            }

            if (assistantProfilePermission.IsGranted)
            {
                _assistantProfilePermissionRepository.InsertProfilePermission(assistantProfilePermission);
            }
            else
            {
                _assistantProfilePermissionRepository.DeleteProfilePermission(assistantProfilePermission);
            }
        }

        public void ShareUserProfile(int profileId, IList<long> assistantUserIds, IList<string> permissionNames)
        {
            _assistantProfileRepository.ShareUserProfile(profileId, assistantUserIds, permissionNames);
        }

        public void AddProfiles(IUserAssistant userAssistant)
        {
            _userAssistantRepository.AddProfiles(userAssistant);
        }
        public void SetCanCreateProfiles(long assistantId, bool canCreateProfiles) 
        {
            _userAssistantRepository.SetCanCreateProfiles(assistantId, canCreateProfiles);
        }
    }
}
