using AutoMapper;
using Chameleon.Domain.Entities;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public class UserProfileAddressBindableMapProfile : Profile
    {
        public UserProfileAddressBindableMapProfile()
        {
            // TODO: Use strong type and mapping
            var map = CreateMap<UserProfileAddressBindable, UserProfileAddress>();
            map.ReverseMap().AfterMap((entity, model) => model.IsPropertyChanged = false);
        }
    }
}
