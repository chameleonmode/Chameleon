using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class PersonMapProfile : AutoMapper.Profile
    {
        public PersonMapProfile()
        {
            CreateEntityDtoMap<PersonDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<PersonBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreatePersonDto>();
            CreateEntityDtoMap<UpdatePersonDto>();
        }

        private IMappingExpression<TDto, Person> CreateEntityDtoMap<TDto>()
            where TDto : PersonEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, Person> CreateBaseDtoMap<TDto>()
            where TDto : PersonBaseDto
        {
            return CreateMap<TDto, Person>()
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.FirstName, options => options.MapFrom(dto => dto.FirstName))
                .ForMember(model => model.LastName, options => options.MapFrom(dto => dto.LastName))
                .ForMember(model => model.MiddleName, options => options.MapFrom(dto => dto.MiddleName))
                .ForMember(model => model.JobTitle, options => options.MapFrom(dto => dto.JobTitle))
                .ForMember(model => model.PhoneNumber, options => options.MapFrom(dto => dto.PhoneNumber))
                .ForMember(model => model.Email, options => options.MapFrom(dto => dto.Email))
                .ForMember(model => model.BirthDate, options => options.MapFrom(dto => dto.BirthDate))
                .ForMember(model => model.BirthPlace, options => options.MapFrom(dto => dto.BirthPlace))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.Gender, options => options.MapFrom(dto => dto.Gender))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                ;
        }
    }
}
