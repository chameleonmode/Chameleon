using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class CredentialMapProfile : AutoMapper.Profile
    {
        public CredentialMapProfile()
        {
            CreateEntityDtoMap<CredentialDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<CredentialBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateCredentialDto>();
            CreateEntityDtoMap<UpdateCredentialDto>();
        }

        private IMappingExpression<TDto, Credential> CreateEntityDtoMap<TDto>()
            where TDto : CredentialEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, Credential> CreateBaseDtoMap<TDto>()
            where TDto : CredentialBaseDto
        {
            return CreateMap<TDto, Credential>()
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.WebSite, options => options.MapFrom(dto => dto.WebSite))
                .ForMember(model => model.UserName, options => options.MapFrom(dto => dto.UserName))
                .ForMember(model => model.Password, options => options.MapFrom(dto => dto.Password))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                ;
        }
    }
}
