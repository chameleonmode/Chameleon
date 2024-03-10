using Chameleon.Interfaces.UserProfiles.Additional;
using Prism.Mvvm;
using System.ComponentModel;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public class UserProfileAddressBindable 
        : BindableBase
        , IUserProfileAddress
    {
        public UserProfileAddressBindable()
        {
            PropertyChanged += UserProfilePersonBindablePropertyChanged;
        }

        public UserProfileAddressBindable(int profileId)
        {
            _profileId = profileId;
            PropertyChanged += UserProfilePersonBindablePropertyChanged;
        }

        private void UserProfilePersonBindablePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IsOpenSearchParameters)) ||
                e.PropertyName.Equals(nameof(IsPropertyChanged)))
            {
                return;
            }

            IsPropertyChanged = true;
        }

        private bool _isPropertyChanged;
        public bool IsPropertyChanged
        {
            get => _isPropertyChanged;
            set => SetProperty(ref _isPropertyChanged, value);
        }

        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _title;
        public string Title 
        { 
            get => _title; 
            set => SetProperty(ref _title,value); 
        }

        private string _addressLine1;
        public string AddressLine1 
        { 
            get => _addressLine1;
            set => SetProperty(ref _addressLine1,value); 
        }

        private string _addressLine2;
        public string AddressLine2 
        { 
            get => _addressLine2;
            set => SetProperty(ref _addressLine2,value); 
        }

        private string _city;
        public string City 
        { 
            get => _city;
            set => SetProperty(ref _city,value); 
        }

        private string _state;
        public string State 
        { 
            get => _state; 
            set => SetProperty(ref _state,value); 
        }

        private string _zip;
        public string Zip 
        { 
            get => _zip; 
            set => SetProperty(ref _zip,value); 
        }

        private int? _countryId;
        public int? CountryId 
        { 
            get => _countryId; 
            set => SetProperty(ref _countryId,value); 
        }        

        private string _notes;
        public string Notes 
        { 
            get => _notes;
            set => SetProperty(ref _notes,value); 
        }

        private int _profileId;
        public int ProfileId
        {
            get => _profileId;
            set => SetProperty(ref _profileId, value);
        }

        public override string ToString()
        {
            return Title;
        }

        private bool _isOpenSearchParameters;
        public bool IsOpenSearchParameters
        {
            get => _isOpenSearchParameters;
            set => SetProperty(ref _isOpenSearchParameters, value);
        }
    }
}
