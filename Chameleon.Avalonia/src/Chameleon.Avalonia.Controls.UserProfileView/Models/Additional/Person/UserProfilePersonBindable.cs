using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.UserProfiles.Additional;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public partial class UserProfilePersonBindable 
        : ObservableObject
        , IUserProfilePerson
    {
        public UserProfilePersonBindable()
        {
            PropertyChanged += UserProfilePersonBindablePropertyChanged;
        }        

        public UserProfilePersonBindable(int profileId)
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

        private string _title = string.Empty;
        public string Title 
        { 
            get => _title; 
            set => SetProperty(ref _title,value); 
        }

        private string _firstName = string.Empty;
        public string FirstName 
        { 
            get => _firstName;
            set => SetProperty(ref _firstName, value); 
        }

        private string _lastName = string.Empty;
        public string LastName 
        { 
            get => _lastName;
            set => SetProperty(ref _lastName,value); 
        }

        private string _middleName = string.Empty;
        public string MiddleName 
        { 
            get => _middleName;
            set => SetProperty(ref _middleName,value); 
        }

        private string _jobTitle = string.Empty;
        public string JobTitle 
        { 
            get => _jobTitle; 
            set => SetProperty(ref _jobTitle,value); 
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber 
        { 
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber,value); 
        }

        private string _email = string.Empty;
        public string Email 
        { 
            get => _email; 
            set => SetProperty(ref _email,value); 
        }

        //private DateTime _birthDate = DateTime.Now.AddYears(-20);
        public DateTime BirthDate 
        { 
            get => BirthDateOffset.DateTime;
            set
            {
                BirthDateOffset = new DateTimeOffset(value);
            }
        }


        private string _birthPlace = string.Empty;
        public string BirthPlace
        {
            get => _birthPlace;
            set => SetProperty(ref _birthPlace, value);
        }
        [ObservableProperty]
        private DateTimeOffset _birthDateOffset = new DateTimeOffset(DateTime.Now.AddYears(-20));

        private string _notes = string.Empty;
        public string Notes 
        { 
            get => _notes; 
            set => SetProperty(ref _notes,value); 
        }

        private GenderType _gender;
        public GenderType Gender 
        { 
            get => _gender;
            set => SetProperty(ref _gender,value); 
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
