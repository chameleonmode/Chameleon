using Chameleon.Interfaces.Country;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.Country;

public class CountryBindableMapProfile : AutoMapper.Profile
{
    public CountryBindableMapProfile()
    {
        var map = CreateMap<CountryBindable, ICountry>();

        map.ReverseMap();
    }
}
