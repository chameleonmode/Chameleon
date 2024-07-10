using Chameleon.Interfaces.Country;
using Chameleon.Interfaces.Ioc;
using System.Collections.ObjectModel;
using Chameleon.Core.Extensions;

namespace Chameleon.Controls.ImportExport.Models
{

    public class ImportColumnOptions : 
        ObservableCollection<ImportColumnOption>
        , IImportColumnOptions
    {
        private readonly IHaveContainerProvider _containerProvider;
        public ImportColumnOptions(IHaveContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
            var countryRepository = _containerProvider.Resolve<ICountryRepository>();
            this.AddRange(new ImportColumnOption[]
            {
                UnselectedColumnOption.Instance,
                new ProfileNameColumnOption(),
                new FullNameColumnOption(),
                new FirstNameColumnOption(),
                new MiddleNameColumnOption(),
                new LastNameColumnOption(),
                //new EmailColumnOption(),
                new ProxyHostColumnOption(),
                new ProxyPortColumnOption(),
                new ProxyUserNameColumnOption(),
                new ProxyPasswordColumnOption(),
                new BirthDateColumnOption(),
                new BirthDayColumnOption(),
                new BirthMonthColumnOption(),
                new BirthYearColumnOption(),
                new LoginTitleColumnOption(),
                new LoginWebSiteColumnOption(),
                new LoginEmailColumnOption(),
                new LoginUsernameColumnOption(),
                new LoginPasswordColumnOption(),
                new LoginNotesColumnOption(),
                new StreetColumnOption(),
                new CityColumnOption(),
                new StateCodeColumnOption(),
                new ZipCodeColumnOption(),
                new CountryColumnOption(countryRepository),
                new PhoneNumberColumnOption()
            });
        }

        private IImportColumnOption _selected;
        public IImportColumnOption Selected
        {
            get => _selected;
            set
            {
                if (value == UnselectedColumnOption.Instance)
                {
                    value = null;
                }

                if (_selected == value)
                {
                    return;
                }

                if (_selected != null)
                {
                    _selected.IsUsed = false;
                }

                if (value != null)
                {
                    value.IsUsed = true;
                }

                _selected = value;
            }
        }
    }
}
