using AutoMapper;
using Chameleon.Domain.Entities;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public class UserProfileLoginBindableMapProfile : Profile
    {
        public UserProfileLoginBindableMapProfile()
        {
            // TODO: Use strong type and mapping
            var map = CreateMap<UserProfileLoginBindable, UserProfileLogin>();
            map.ReverseMap().AfterMap((entity, model) => model.IsPropertyChanged = false);
        }
    }
}
