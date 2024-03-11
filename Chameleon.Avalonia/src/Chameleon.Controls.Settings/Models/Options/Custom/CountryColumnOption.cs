using Chameleon.Interfaces.Country;
using Chameleon.Interfaces.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chameleon.Controls.ImportExport.Models
{
    public class CountryColumnOption : 
        ImportColumnOption
    {
        private readonly ICountryRepository _countryRepository;
        public CountryColumnOption(ICountryRepository countryRepository)
            : base(ImportColumnOptionType.Country)
        {
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

        public override void Map(IUserProfile profile, string input)
        {            
            var country = Countries.FirstOrDefault(
                        entity => entity.Name.Equals(input)
                        || entity.IsoCode2.Equals(input, StringComparison.InvariantCultureIgnoreCase)
                        || entity.IsoCode3.Equals(input, StringComparison.InvariantCultureIgnoreCase)
                        );
           
            if (country == null)
            {
                return;
            }
            GetAddress(profile).CountryId = country.Id;
        }
    }
}
