using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.UserProfiles.Additional;
using System;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public class UserProfilePersonBindableMapProfile : Profile
    {
        public UserProfilePersonBindableMapProfile()
        {
            // TODO: Use strong type and mapping
            //var map = CreateMap<UserProfilePersonBindable, UserProfilePerson>();
            //map.ReverseMap();
            //Using strong type mapping
            CreateMap<UserProfilePersonBindable, UserProfilePerson>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.ProfileId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.BirthPlace, opt => opt.MapFrom(src => src.BirthPlace))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ReverseMap();
        }
    }
}
