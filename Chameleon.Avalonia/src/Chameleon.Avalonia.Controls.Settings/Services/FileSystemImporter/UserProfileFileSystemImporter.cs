using Chameleon.Core.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Country;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserProfiles.Additional;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.Controls.ImportExport.Services
{
    public class UserProfileFileSystemImporter : IUserProfileFileSystemImporter
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IUserProfileFolderService _userProfileFolderService;
        private readonly ICountryRepository _countryRepository;

        public UserProfileFileSystemImporter(
            IUserProfileService userProfileService,
            IUserProfileFolderService userProfileFolderService,
            ICountryRepository countryRepository
            )
        {
            _userProfileService = userProfileService;
            _userProfileFolderService = userProfileFolderService;
            _countryRepository = countryRepository;
        }

        private IList<ICountry> _countries;
        private IList<ICountry> Countries
        {
            get
            {
                if (_countries == null)
                {
                    _countries = _countryRepository.GetAll();
                }
                return _countries;
            }
        }

        public async Task ImportAsync()
        {
            var rootFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ChameleonMode", "Projects"
                );

            await ImportProfileAsync(new DirectoryInfo(rootFolder));

            var directoriesToAnalyze = Directory
                .GetDirectories(rootFolder)
                .Select(path => new DirectoryInfo(path))
                .ToList();
            
            await ImportDirectoriesAsync(directoriesToAnalyze);
        }

        private async Task ImportProfileAsync(DirectoryInfo directory)
        {
            var profiles = ReadProfiles(directory);
            await _userProfileService.ImportAsync(profiles.ConvertAll(profile => (IUserProfile) profile));
        }

        private async Task ImportDirectoriesAsync(IList<DirectoryInfo> directories)
        {
            var existingFolders = _userProfileFolderService.GetAll();
            foreach (var directory in directories)
            {
                var folderName = GetUniqueTitle(existingFolders, directory.Name);
                var folder = _userProfileFolderService.Create(folderName);

                var profiles = ReadProfiles(directory);
                foreach (var profile in profiles)
                {
                    profile.FolderId = folder.Id;
                }

                await _userProfileService.ImportAsync(profiles.ConvertAll(profile => (IUserProfile)profile));
            }
        }

        private string GetUniqueTitle(IUserProfileFolders folders, string title)
        {
            while (folders.HasWithTitle(title))
            {
                title += "_1";
            }
            return title;
        }

        private List<UserProfile> ReadProfiles(DirectoryInfo directory)
        {
            var profiles = directory.GetFiles("*.identityconfig")
                .Select(file => ReadProfile(directory, file))
                .ToList();

            return profiles;
        }

        private UserProfile ReadProfile(DirectoryInfo directory, FileInfo identityFile)
        {
            dynamic jObject = ReadObject(identityFile);

            var title = jObject.ProfileName?.ToString();
            if (string.IsNullOrEmpty(title))
            {
                title = jObject.Title?.ToString();
            }

            var profile = new UserProfile
            {
                Title = title
            };

            profile.Proxy.Host = jObject.IP?.ToString();

            int.TryParse(jObject.Port?.ToString(), out int port);
            profile.Proxy.Port = port;
            
            profile.Proxy.UserName = jObject.ProxyUsername?.ToString();
            profile.Proxy.Password = jObject.ProxyPassword?.ToString();

            var identityID = jObject.IdentityID?.ToString();
            if (string.IsNullOrEmpty(identityID))
            {
                throw new InvalidDataException("No IdentityID in file");
            }

            IList<UserProfilePerson> persons = ReadPersons(directory, identityID);
            profile.Persons.AddRange(persons);

            IList<UserProfileAddress> addresses = ReadAddresses(directory, identityID);
            profile.Addresses.AddRange(addresses);

            IList<UserProfileBusiness> businesses = ReadBusinesses(directory, identityID);
            profile.Businesses.AddRange(businesses);

            IList<UserProfileLogin> logins = ReadLogins(directory, identityID);
            profile.Logins.AddRange(logins);
            
            return profile;
        }

        private dynamic ReadObject(FileInfo file)
        {
            var fileContent = File.ReadAllText(file.FullName);
            return JObject.Parse(fileContent);
        }

        private dynamic ReadArray(FileInfo file)
        {
            var fileContent = File.ReadAllText(file.FullName);
            return JArray.Parse(fileContent);
        }

        private IList<UserProfilePerson> ReadPersons(DirectoryInfo directory, string identityId)
        {
            var file = directory
                .GetFiles($"{identityId}.Persons.fieldsconfig")
                .FirstOrDefault();
            if (file == null)
            {
                return new UserProfilePerson[0];
            }

            var items = new List<UserProfilePerson>();
            dynamic jObjects = ReadArray(file);
            foreach (var jObject in jObjects)
            {
                var item = new UserProfilePerson
                {
                    Title = jObject.Title?.ToString(),
                    FirstName = jObject.FirstName?.ToString(),
                    LastName = jObject.LastName?.ToString(),
                    MiddleName = jObject.MiddleName?.ToString(),
                    JobTitle = jObject.JobTitle?.ToString(),
                    PhoneNumber = jObject.PhoneNumber?.ToString(),
                    Email = jObject.Email?.ToString()
                };

                if (bool.TryParse(jObject.IsMale?.ToString(), out bool isMale) && isMale)
                {
                    item.Gender = GenderType.Male;
                }
                if (bool.TryParse(jObject.IsFemale?.ToString(), out bool isFemale) && isFemale)
                {
                    item.Gender = GenderType.Female;
                }
                
                DateTime.TryParse(jObject.Birthdate?.ToString(), out DateTime birthDate);
                item.BirthDate = birthDate;

                item.BirthPlace = jObject.BirthPlace?.ToString();
                item.Notes = jObject.Note?.ToString();

                items.Add(item);
            }
            return items;
        }

        private IList<UserProfileAddress> ReadAddresses(DirectoryInfo directory, string identityId)
        {
            var file = directory
                .GetFiles($"{identityId}.Addresses.fieldsconfig")
                .FirstOrDefault();
            if (file == null)
            {
                return new UserProfileAddress[0];
            }

            var items = new List<UserProfileAddress>();
            dynamic jObjects = ReadArray(file);
            foreach (var jObject in jObjects)
            {
                var item = new UserProfileAddress
                {
                    Title = jObject.Title?.ToString(),
                    AddressLine1 = jObject.AddressLine1?.ToString(),
                    AddressLine2 = jObject.AddressLine2?.ToString(),
                    City = jObject.City?.ToString(),
                    State = jObject.State?.ToString(),
                    Zip = jObject.Zip?.ToString(),
                    Notes = jObject.Note?.ToString()
                };

                var countryName = jObject.Country?.ToString();
                if (!string.IsNullOrEmpty(countryName))
                {
                    var country = Countries.FirstOrDefault(
                        entity => entity.Name.Equals(countryName)
                        );
                    if (country != null)
                    {
                        item.CountryId = country.Id;
                    }
                }

                items.Add(item);
            }
            return items;
        }

        private IList<UserProfileBusiness> ReadBusinesses(DirectoryInfo directory, string identityId)
        {
            var file = directory
                .GetFiles($"{identityId}.Businesses.fieldsconfig")
                .FirstOrDefault();
            if (file == null)
            {
                return new UserProfileBusiness[0];
            }

            var items = new List<UserProfileBusiness>();
            dynamic jObjects = ReadArray(file);
            foreach (var jObject in jObjects)
            {
                var item = new UserProfileBusiness
                {
                    Title = jObject.Title?.ToString(),
                    CompanyName = jObject.CompanyName?.ToString(),
                    Department = jObject.Department?.ToString(),
                    PhoneNumber = jObject.PhoneNumber?.ToString(),
                    WebSite = jObject.WebSite?.ToString(),
                    Notes = jObject.Note?.ToString()
                };

                items.Add(item);
            }
            return items;
        }

        private IList<UserProfileLogin> ReadLogins(DirectoryInfo directory, string identityId)
        {
            var file = directory
                .GetFiles($"{identityId}.Logins.fieldsconfig")
                .FirstOrDefault();
            if (file == null)
            {
                return new UserProfileLogin[0];
            }

            var items = new List<UserProfileLogin>();
            dynamic jObjects = ReadArray(file);
            foreach (var jObject in jObjects)
            {
                var item = new UserProfileLogin
                {
                    Title = jObject.Title?.ToString(),
                    WebSite = jObject.Website?.ToString(),
                    UserName = jObject.Email?.ToString(),
                    Password = jObject.Password?.ToString(),
                    Notes = jObject.Note?.ToString()
                };

                items.Add(item);
            }
            return items;
        }
    }
}
