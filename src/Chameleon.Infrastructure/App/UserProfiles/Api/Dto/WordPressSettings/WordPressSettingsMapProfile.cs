using AutoMapper;
using Chameleon.Interfaces.WordPress;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.WordPressSettings
{
    public class WordPressSettingsMapProfile : Profile
    {
        public WordPressSettingsMapProfile()
        {
            var map = CreateMap<WordPressSettingsDto, IWordPressSettings>()
                .ForMember(model => model.BaseUrl, options => options.MapFrom(dto => dto.BaseUrl))
                .ForMember(model => model.Username, options => options.MapFrom(dto => dto.Username))
                .ForMember(model => model.Password, options => options.MapFrom(dto => dto.Password))
                ;

            map.ReverseMap();
        }
    }
}
