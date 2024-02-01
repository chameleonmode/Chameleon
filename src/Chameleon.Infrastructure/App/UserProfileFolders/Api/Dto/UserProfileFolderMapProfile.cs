using AutoMapper;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Infrastructure.UserProfileFolders
{
    public class UserProfileFolderMapProfile : Profile
    {
        public UserProfileFolderMapProfile()
        {
            UserProfileDtoMap();
            CreateUserProfileDtoMap();
        }

        private void UserProfileDtoMap()
        {
            var map = CreateMap<UserProfileFolderDto, IUserProfileFolder>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.IsFavorite, options => options.MapFrom(dto => dto.IsFavorite))
                .ForMember(model => model.ProfilesCount, options => options.MapFrom(dto => dto.ProfilesCount))
                .ForMember(model => model.CreatorUserId, options => options.MapFrom(dto => dto.CreatorUserId))
                ;
            
            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateUserProfileDtoMap()
        {
            CreateMap<IUserProfileFolder, CreateUserProfileFolderDto>()
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.IsFavorite, options => options.MapFrom(dto => dto.IsFavorite))
                .ForMember(model => model.ProfilesCount, options => options.MapFrom(dto => dto.ProfilesCount))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
