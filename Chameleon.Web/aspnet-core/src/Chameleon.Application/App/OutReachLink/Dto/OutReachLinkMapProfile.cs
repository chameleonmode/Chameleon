using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class OutReachLinkMapProfile : AutoMapper.Profile
    {
        public OutReachLinkMapProfile()
        {
            CreateEntityDtoMap<OutReachLinkDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<OutReachLinkBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateOutReachLinkDto>();
            CreateEntityDtoMap<UpdateOutReachLinkDto>();
        }

        private IMappingExpression<TDto, OutReachLink> CreateEntityDtoMap<TDto>()
            where TDto : OutReachLinkEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, OutReachLink> CreateBaseDtoMap<TDto>()
            where TDto : OutReachLinkBaseDto
        {
            return CreateMap<TDto, OutReachLink>()
                .ForMember(model => model.ContactEmail, options => options.MapFrom(dto => dto.ContactEmail))
                .ForMember(model => model.ContactName, options => options.MapFrom(dto => dto.ContactName))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.UrlName, options => options.MapFrom(dto => dto.UrlName))
                .ForMember(model => model.Status, options => options.MapFrom(dto => dto.Status))
                .ForMember(model => model.UrlType, options => options.MapFrom(dto => dto.UrlType))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                .ForMember(model => model.Twitter, options => options.MapFrom(dto => dto.Twitter))
                .ForMember(model => model.Linkedin, options => options.MapFrom(dto => dto.Linkedin))
                .ForMember(model => model.Facebook, options => options.MapFrom(dto => dto.Facebook))
                .ForMember(model => model.OtherSocial, options => options.MapFrom(dto => dto.OtherSocial))
                .ForMember(model => model.ReminderNotes, options => options.MapFrom(dto => dto.ReminderNotes))
                .ForMember(model => model.ReminderDatetime, options => options.MapFrom(dto => dto.ReminderDatetime))
                ;
        }
    }
}

