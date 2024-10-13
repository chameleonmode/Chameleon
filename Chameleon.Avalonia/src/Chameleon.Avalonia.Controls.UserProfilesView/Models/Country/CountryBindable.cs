using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.Country;


namespace Chameleon.Avalonia.Controls.UserProfileView.Models.Country;

public class CountryBindable
    : ObservableObjectBase
    , ICountry
{
    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _isoCode2;
    public string IsoCode2
    {
        get => _isoCode2;
        set => SetProperty(ref _isoCode2, value);
    }

    private string _isoCode3;
    public string IsoCode3
    {
        get => _isoCode3;
        set => SetProperty(ref _isoCode3, value);
    }
}
