using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Interfaces.App.ShareFolders
{
    public interface IShareFoldersService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IList<IShareFolder> GetAll(long userId);
        IList<IShareFolder> Share(long userId, IList<int> folderIds, IList<int> permissionIds);
        void Delete(int userFolderId);
        void AddPermission(int userFolderId, int permissionId);
        void DeletePermission(int userFolderId, int permissionId);
        string[] GetAllProfilePermissionNames(long userId, int profileId, int? folderId);
    }
}
