using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.OutReach;

namespace Chameleon.Controls.UserProfileView.Models.Additional.OutReach
{
    public class UserProfileOutReachRssBindableMap : Profile
    {
        public UserProfileOutReachRssBindableMap()
        {
            // TODO: Use strong type and mapping
            var map = CreateMap<UserProfileOutReachRssBindable, UserProfileOutReachRss>();
            map.ReverseMap();
        }
    }
}
