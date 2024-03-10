using AutoMapper;
using Chameleon.Domain.Entities;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public class UserProfileBusinessBindableMapProfile : Profile
    {
        public UserProfileBusinessBindableMapProfile()
        {
            // TODO: Use strong type and mapping
            var map = CreateMap<UserProfileBusinessBindable, UserProfileBusiness>();
            map.ReverseMap().AfterMap((entity, model) => model.IsPropertyChanged = false);
        }
    }
}
