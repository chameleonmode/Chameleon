using AutoMapper;
using Chameleon.Interfaces.App.Assistants;

namespace Chameleon.Infrastructure.Users
{
    public class UserAssistantMapProfile : Profile
    {
        public UserAssistantMapProfile()
        {
            UserAssistantDtoMap();
            CreateUserAssistantDtoMap();
        }

        private void UserAssistantDtoMap()
        {
            var map = CreateMap<UserAssistantDto, IUserAssistant>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.UserName, options => options.MapFrom(dto => dto.UserName))
                .ForMember(model => model.EmailAddress, options => options.MapFrom(dto => dto.EmailAddress))
                .ForMember(model => model.Password, options => options.MapFrom(dto => dto.Password))
                .ForMember(model => model.CanCreateProfiles, options => options.MapFrom(dto => dto.CanCreateProfiles))
                .ForMember(model => model.ProfileIds, options => options.MapFrom(dto => dto.ProfileIds))
                .ForMember(model => model.ProfilePermissionIds, options => options.MapFrom(dto => dto.ProfilePermissionIds));

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateUserAssistantDtoMap()
        {
            CreateMap<IUserAssistant, CreateUserAssistantDto>()
                .ForMember(model => model.UserName, options => options.MapFrom(dto => dto.UserName))
                .ForMember(model => model.EmailAddress, options => options.MapFrom(dto => dto.EmailAddress))
                .ForMember(model => model.ProfileIds, options => options.MapFrom(dto => dto.ProfileIds))
                .ForMember(model => model.ProfilePermissionIds, options => options.MapFrom(dto => dto.ProfilePermissionIds))
                .ForMember(model => model.FolderIds, options => options.MapFrom(dto => dto.FolderIds))
                .ForMember(model => model.FolderPermissionIds, options => options.MapFrom(dto => dto.FolderPermissionIds))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
