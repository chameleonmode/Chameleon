using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.ShareFolders.Api.Dto;
using Chameleon.Interfaces.Api;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.ShareFolders.Api
{
    public class ShareFoldersApi
        : ApiLayer<ShareFolderDto, 
            int, 
            CreateShareFolderDto, 
            UpdateShareFolderDto
            >
        , IShareFoldersApi
    {
        public ShareFoldersApi(
            IApiClient apiClient
            ) : base(apiClient, "ShareFolders")
        {
        }

        public void DeleteFolder(int id)
        {
            _apiClient.Delete(GetEndpointUrl($"Delete?id={id}"));
        }

        public void AddPermission(CreateShareFolderPermissionDto input)
        {
            _apiClient.Post(GetEndpointUrl("AddPermission"), input);
        }

        public void DeletePermission(int userFolderId, int permissionId)
        {
            _apiClient.Delete(GetEndpointUrl($"DeletePermission?userFolderId={userFolderId}&permissionId={permissionId}"));
        }

        public IList<ShareFolderDto> Share(CreateShareFolderDto input)
        {
            return _apiClient.Post<ShareFolderDto[]>(GetEndpointUrl("Share"), input);
        }

        public string[] GetAllProfilePermissionNames(long userId, int profileId, int? folderId)
        {
            return _apiClient.Get<string[]>(GetEndpointUrl($"GetAllProfilePermissionNames?userId={userId}&profileId={profileId}&folderId={folderId}"));
        }
    }
}
