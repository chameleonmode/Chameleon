using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.OutReach;

namespace Chameleon.Infrastructure.OutReachLink
{
    public class ProfileOutReachLinkDtoMapProfile : Profile
    {
        public ProfileOutReachLinkDtoMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }

        private void DtoMap()
        {
            var map = CreateMap<ProfileOutReachLinkDto, ProfileOutReachLink>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.Status, options => options.MapFrom(dto => dto.Status))
                .ForMember(model => model.UrlType, options => options.MapFrom(dto => dto.UrlType))
                .ForMember(model => model.UrlName, options => options.MapFrom(dto => dto.UrlName))                
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                .ForMember(model => model.ContactName, options => options.MapFrom(dto => dto.ContactName))
                .ForMember(model => model.ContactEmail, options => options.MapFrom(dto => dto.ContactEmail))
                .ForMember(model => model.Twitter, options => options.MapFrom(dto => dto.Twitter))
                .ForMember(model => model.Linkedin, options => options.MapFrom(dto => dto.Linkedin))
                .ForMember(model => model.Facebook, options => options.MapFrom(dto => dto.Facebook))
                .ForMember(model => model.OtherSocial, options => options.MapFrom(dto => dto.OtherSocial))                
                .ForMember(model => model.ReminderNotes, options => options.MapFrom(dto => dto.ReminderNotes))
                .ForMember(model => model.ReminderDatetime, options => options.MapFrom(dto => dto.ReminderDatetime))
                ;

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateDtoMap()
        {
            CreateMap<IProfileOutReachLink, CreateProfileOutReachLinkDto>()
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.Status, options => options.MapFrom(dto => dto.Status))
                .ForMember(model => model.UrlType, options => options.MapFrom(dto => dto.UrlType))
                .ForMember(model => model.UrlName, options => options.MapFrom(dto => dto.UrlName))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                .ForMember(model => model.ContactName, options => options.MapFrom(dto => dto.ContactName))
                .ForMember(model => model.ContactEmail, options => options.MapFrom(dto => dto.ContactEmail))
                .ForMember(model => model.Twitter, options => options.MapFrom(dto => dto.Twitter))
                .ForMember(model => model.Linkedin, options => options.MapFrom(dto => dto.Linkedin))
                .ForMember(model => model.Facebook, options => options.MapFrom(dto => dto.Facebook))
                .ForMember(model => model.OtherSocial, options => options.MapFrom(dto => dto.OtherSocial))
                .ForMember(model => model.ReminderNotes, options => options.MapFrom(dto => dto.ReminderNotes))
                .ForMember(model => model.ReminderDatetime, options => options.MapFrom(dto => dto.ReminderDatetime))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
