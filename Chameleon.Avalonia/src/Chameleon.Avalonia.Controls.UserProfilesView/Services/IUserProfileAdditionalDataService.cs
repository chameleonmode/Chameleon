using Chameleon.Avalonia.Controls.UserProfileView.Models.Country;
using Chameleon.Controls.UserProfileView.Models.Additional;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.OutReach;

namespace Chameleon.Avalonia.Controls.UserProfileView.Services;

public interface IUserProfileAdditionalDataService
    : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
    int AddAddress(UserProfileAddressBindable addressBindable);
    int AddBusiness(UserProfileBusinessBindable businessBindable);
    int AddLogin(UserProfileLoginBindable loginBindable);
    int AddPerson(UserProfilePersonBindable personBindable);
    void DeleteAddress(UserProfileAddressBindable addressBindable);
    void DeleteBusiness(UserProfileBusinessBindable businessBindable);
    void DeleteLogin(UserProfileLoginBindable loginBindable);
    void DeletePerson(UserProfilePersonBindable personBindable);
    IEnumerable<CountryBindable> GetCountries();
    IEnumerable<UserProfileAddressBindable> GetAddresses(int profileId, bool ignoreCache = true);
    Task<IEnumerable<UserProfileAddressBindable>> GetAddressesAsync(int profileId, bool ignoreCache = true);
    IEnumerable<UserProfileBusinessBindable> GetBusinesses(int profileId);
    Task<IEnumerable<UserProfileBusinessBindable>> GetBusinessesAsync(int profileId);
    IEnumerable<UserProfileLoginBindable> GetLogins(int profileId, bool ignoreCache = true);
    Task<IEnumerable<UserProfileLoginBindable>> GetLoginsAsync(int profileId, bool ignoreCache = true);
    IEnumerable<UserProfilePersonBindable> GetPersons(int profileId, bool ignoreCache = true);
    Task<IEnumerable<UserProfilePersonBindable>> GetPersonsAsync(int profileId, bool ignoreCache = true);
    void SaveAddress(UserProfileAddressBindable addressBindable);
    void SaveBusiness(UserProfileBusinessBindable businessBindable);
    void SaveLogin(UserProfileLoginBindable loginBindable);
    void SavePerson(UserProfilePersonBindable personBindable);
    int AddOutReachRss(UserProfileOutReachRssBindable outReachRss);
    void SaveOutReachRss(UserProfileOutReachRssBindable outReachRss);
    void DeleteOutReachRss(UserProfileOutReachRssBindable outReachRss);
    IReadOnlyList<UserProfileOutReachRssBindable> GetOutReachRsses(int profileId);
}
