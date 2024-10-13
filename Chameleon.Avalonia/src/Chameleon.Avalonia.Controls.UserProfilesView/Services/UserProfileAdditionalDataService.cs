using AutoMapper;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Country;
using Chameleon.Controls.UserProfileView.Models.Additional;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Country;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.UserProfiles.Additional;
using Chameleon.Core.Extensions;

namespace Chameleon.Avalonia.Controls.UserProfileView.Services;

/// <summary>
/// https://davidprice.atlassian.net/browse/CHAM-455
/// For now, we got rid of the option to get data from cache completely, because the generic repository(concurrent dictionary cache)
/// does not know to which profile the profile additiona data belongs to.As a result, we get incorrect data, in other words,
/// we can see the profile-related data that comes from a different profile.
/// </summary>
public class UserProfileAdditionalDataService : IUserProfileAdditionalDataService
{
    private readonly IMapper _mapper;
    private readonly IUserProfilePersonRepository _personRepository;
    private readonly IUserProfileBusinessRepository _businessRepository;
    private readonly IUserProfileAddressRepository _addressRepository;
    private readonly IUserProfileLoginRepository _loginRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IUserProfileOutReachRssRepository _outReachRssRepository;

    public UserProfileAdditionalDataService(
        IMapper mapper,
        IUserProfilePersonRepository personRepository,
        IUserProfileBusinessRepository businessRepository,
        IUserProfileAddressRepository addressRepository,
        IUserProfileLoginRepository loginRepository,
        ICountryRepository countryRepository,
        IUserProfileOutReachRssRepository outReachRssRepository)
    {
        _mapper = mapper;
        _personRepository = personRepository;
        _businessRepository = businessRepository;
        _addressRepository = addressRepository;
        _loginRepository = loginRepository;
        _countryRepository = countryRepository;
        _outReachRssRepository = outReachRssRepository;
    }

    public int AddOutReachRss(UserProfileOutReachRssBindable outReachRss)
    {
        var outreachRss = _mapper.Map<UserProfileOutReachRss>(outReachRss);
        return _outReachRssRepository.Insert(outreachRss);
    }

    public void SaveOutReachRss(UserProfileOutReachRssBindable outReachRss)
    {
        var outreachRss = _mapper.Map<UserProfileOutReachRss>(outReachRss);
        Task.Run(() => _outReachRssRepository.Update(outreachRss));
    }

    public void DeleteOutReachRss(UserProfileOutReachRssBindable outReachRss)
    {
        Task.Run(() => _outReachRssRepository.Delete(outReachRss.Id));
    }

    public int AddPerson(UserProfilePersonBindable personBindable)
    {
        var person = _mapper.Map<UserProfilePerson>(personBindable);
        return _personRepository.Insert(person);
    }

    public void SavePerson(UserProfilePersonBindable personBindable)
    {
        var person = _mapper.Map<UserProfilePerson>(personBindable);
        Task.Run(() => _personRepository.Update(person));
    }

    public void DeletePerson(UserProfilePersonBindable personBindable)
    {
        Task.Run(() => _personRepository.Delete(personBindable.Id));
    }

    public IEnumerable<UserProfilePersonBindable> GetPersons(int profileId, bool ignoreCache = true)
    {
        var request = new UserProfileGetAllRequestDto(profileId);
        var persons = _personRepository.GetAll(ignoreCache, request);
        //return _mapper.Map<UserProfilePersonBindable[]>(persons);

        // Validate the source data before mapping
        for (int i = 0; i < persons.Length; i++)
        {
           if (persons[i].BirthDate <= DateTimeOffset.MinValue.DateTime || persons[i].BirthDate >= DateTimeOffset.MaxValue.DateTime)
                persons[i].BirthDate = DateTimeOffset.Now.AddYears(-20).DateTime;
        }
        return _mapper.Map<UserProfilePersonBindable[]>(persons);

        // Validate the source data before mapping
        //List<UserProfilePersonBindable> result = [];
        //foreach (var person in persons)
        //{
        //    if (person.BirthDate <= DateTimeOffset.MinValue.DateTime || person.BirthDate >= DateTimeOffset.MaxValue.DateTime)
        //        person.BirthDate = DateTimeOffset.Now.AddYears(-20).DateTime;

        //    //result.Add(new UserProfilePersonBindable()
        //    //{
        //    //    Id = person.Id,
        //    //    ProfileId = person.ProfileId,
        //    //    Title = person.Title ?? $"{result.Count}",
        //    //    JobTitle = person.JobTitle,
        //    //    FirstName = person.FirstName,
        //    //    MiddleName = person.MiddleName,
        //    //    LastName = person.LastName,
        //    //    Email = person.Email,
        //    //    PhoneNumber = person.PhoneNumber,
        //    //    BirthPlace = person.BirthPlace,
        //    //    Notes = person.Notes,
        //    //    Gender = person.Gender,
        //    //    BirthDate = person.BirthDate,
        //    //});
        //}
        //return result;
    }


    public Task<IEnumerable<UserProfilePersonBindable>> GetPersonsAsync(int profileId, bool ignoreCache = true)
           => Task.Run(() => GetPersons(profileId, ignoreCache));
    public int AddBusiness(UserProfileBusinessBindable businessBindable)
    {
        var business = _mapper.Map<UserProfileBusiness>(businessBindable);
        return _businessRepository.Insert(business);
    }

    public void SaveBusiness(UserProfileBusinessBindable businessBindable)
    {
        var business = _mapper.Map<UserProfileBusiness>(businessBindable);
        Task.Run(() => _businessRepository.Update(business));
    }

    public void DeleteBusiness(UserProfileBusinessBindable businessBindable)
    {
        Task.Run(() => _businessRepository.Delete(businessBindable.Id));
    }

    public IEnumerable<UserProfileBusinessBindable> GetBusinesses(int profileId)
    {
        var request = new UserProfileGetAllRequestDto(profileId);
        var businesses = _businessRepository.GetAll(true, request);
        return _mapper.Map<UserProfileBusinessBindable[]>(businesses);
    }
    public Task<IEnumerable<UserProfileBusinessBindable>> GetBusinessesAsync(int profileId)
       => Task.Run(() => GetBusinesses(profileId));

    public int AddAddress(UserProfileAddressBindable addressBindable)
    {
        var address = _mapper.Map<UserProfileAddress>(addressBindable);
        return _addressRepository.Insert(address);
    }

    public void SaveAddress(UserProfileAddressBindable addressBindable)
    {
        var address = _mapper.Map<UserProfileAddress>(addressBindable);
        Task.Run(() => _addressRepository.Update(address));
    }

    public void DeleteAddress(UserProfileAddressBindable addressBindable)
    {
        Task.Run(() => _addressRepository.Delete(addressBindable.Id));
    }

    public IEnumerable<UserProfileAddressBindable> GetAddresses(int profileId, bool ignoreCache = true)
    {
        var request = new UserProfileGetAllRequestDto(profileId);
        var addresses = _addressRepository.GetAll(ignoreCache, request);
        return _mapper.Map<UserProfileAddressBindable[]>(addresses);
    }
    public Task<IEnumerable<UserProfileAddressBindable>> GetAddressesAsync(int profileId, bool ignoreCache = true)
        => Task.Run(()=> GetAddresses(profileId, ignoreCache));

    public int AddLogin(UserProfileLoginBindable loginBindable)
    {
        var login = _mapper.Map<UserProfileLogin>(loginBindable);
        return _loginRepository.Insert(login);
    }

    public void SaveLogin(UserProfileLoginBindable loginBindable)
    {
        var login = _mapper.Map<UserProfileLogin>(loginBindable);
        Task.Run(() => _loginRepository.Update(login));
    }

    public void DeleteLogin(UserProfileLoginBindable loginBindable)
    {
        Task.Run(() => _loginRepository.Delete(loginBindable.Id));
    }

    public IEnumerable<UserProfileLoginBindable> GetLogins(int profileId, bool ignoreCache = true)
    {
        var request = new UserProfileGetAllRequestDto(profileId);
        var logins = _loginRepository.GetAll(ignoreCache, request);
        return _mapper.Map<UserProfileLoginBindable[]>(logins);
    }
    public Task<IEnumerable<UserProfileLoginBindable>> GetLoginsAsync(int profileId, bool ignoreCache = true)
        => Task.Run(() => GetLogins(profileId, ignoreCache));

    public IEnumerable<CountryBindable> GetCountries()
    {
        var countries = _countryRepository.GetAll();
        return _mapper.Map<CountryBindable[]>(countries)
            .OrderBy(coutry => coutry.Name);
    }

    public IReadOnlyList<UserProfileOutReachRssBindable> GetOutReachRsses(int profileId)
    {
        var request = new UserProfileGetAllRequestDto(profileId);
        var outReachRsses = _outReachRssRepository.GetAll(true, request);
        return _mapper.Map<UserProfileOutReachRssBindable[]>(outReachRsses)
            .ToReadOnlyList();
    }
}
