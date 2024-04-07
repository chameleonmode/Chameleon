using Chameleon.Infrastructure.App.ShareFolders.Api.Dto;
using Chameleon.Interfaces.App.ShareFolders;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.ShareFolders
{
    public class ShareFoldersService
        : IShareFoldersService
    {
        private readonly IShareFoldersRepository _shareFoldersRepository;

        public ShareFoldersService(
            IShareFoldersRepository shareFoldersRepository)
        {
            _shareFoldersRepository = shareFoldersRepository;
        }
        public void AddPermission(int userFolderId, int permissionId)
        {
            _shareFoldersRepository.AddPermission(userFolderId, permissionId);
        }

        public void Delete(int userFolderId)
        {
            _shareFoldersRepository.Delete(userFolderId);
        }

        public void DeletePermission(int userFolderId, int permissionId)
        {
            _shareFoldersRepository.DeletePermission(userFolderId, permissionId);
        }

        public IList<IShareFolder> GetAll(long userId)
        {
            try
            {
                return _shareFoldersRepository.GetAll(true, new ShareFolderGetAllRequestDto() { UserId = userId });
            }
            catch
            {
                return new List<IShareFolder>();
            }
        }

        public IList<IShareFolder> Share(long userId, IList<int> folderIds, IList<int> permissionIds)
        {
            return _shareFoldersRepository.Share(userId, folderIds, permissionIds);
        }
        public string[] GetAllProfilePermissionNames(long userId, int profileId, int? folderId)
        {
            return _shareFoldersRepository.GetAllProfilePermissionNames(userId, profileId, folderId);
        }
    }
}
