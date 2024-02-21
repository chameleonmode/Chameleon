using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class AddressMapProfile : AutoMapper.Profile
    {
        public AddressMapProfile()
        {
            CreateEntityDtoMap<AddressDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<AddressBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateAddressDto>();
            CreateEntityDtoMap<UpdateAddressDto>();
        }

        private IMappingExpression<TDto, Address> CreateEntityDtoMap<TDto>()
            where TDto : AddressEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, Address> CreateBaseDtoMap<TDto>()
            where TDto : AddressBaseDto
        {
            return CreateMap<TDto, Address>()
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.AddressLine1, options => options.MapFrom(dto => dto.AddressLine1))
                .ForMember(model => model.AddressLine2, options => options.MapFrom(dto => dto.AddressLine2))
                .ForMember(model => model.City, options => options.MapFrom(dto => dto.City))
                .ForMember(model => model.State, options => options.MapFrom(dto => dto.State))
                .ForMember(model => model.Zip, options => options.MapFrom(dto => dto.Zip))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.CountryId, options => options.MapFrom(dto => dto.CountryId))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                ;
        }
    }
}
