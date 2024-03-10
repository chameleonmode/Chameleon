using AutoMapper;
using Chameleon.Domain.Entities;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public class UserProfilePersonBindableMapProfile : Profile
    {
        public UserProfilePersonBindableMapProfile()
        {
            // TODO: Use strong type and mapping
            var map = CreateMap<UserProfilePersonBindable, UserProfilePerson>();
            map.ReverseMap().AfterMap((entity, model) => model.IsPropertyChanged = false);
        }
    }
}
