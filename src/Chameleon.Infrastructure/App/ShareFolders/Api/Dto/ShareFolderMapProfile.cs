using AutoMapper;
using Chameleon.Interfaces.App.ShareFolders;

namespace Chameleon.Infrastructure.App.ShareFolders.Api.Dto
{
    public class ShareFolderMapProfile 
        : Profile
    {
        public ShareFolderMapProfile()
        {
            ShareFolderDtoMap();
            ShareFolderPermissionDtoMap();
        }

        private void ShareFolderPermissionDtoMap()
        {
            var map = CreateMap<ShareFolderPermissionDto, IShareFolderPermission>()
                .ForMember(model => model.PermissionId, options => options.MapFrom(dto => dto.PermissionId))
                .ForMember(model => model.PermissionName, options => options.MapFrom(dto => dto.PermissionName))
                .ForMember(model => model.DisplayName, options => options.MapFrom(dto => dto.DisplayName))
                .ForMember(model => model.IsGranted, options => options.MapFrom(dto => dto.IsGranted));

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void ShareFolderDtoMap()
        {
            var map = CreateMap<ShareFolderDto, IShareFolder>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.UserId, options => options.MapFrom(dto => dto.UserId))
                .ForMember(model => model.FolderId, options => options.MapFrom(dto => dto.FolderId))
                .ForMember(model => model.FolderName, options => options.MapFrom(dto => dto.FolderName))
                .ForMember(model => model.FolderPermissions, options => options.MapFrom(dto => dto.FolderPermissions));

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
