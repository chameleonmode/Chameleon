using Chameleon.Avalonia.Controls.UserProfileView.ViewModels;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.UserProfiles.Additional;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public partial class UserProfilePersonBindable 
        : ObservableObject
        , IUserProfilePerson
    {
        [ObservableProperty]
        private int _id; 
        [ObservableProperty]
        private int _profileId;
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _jobTitle = string.Empty;
        [ObservableProperty]
        private string _firstName = string.Empty;
        [ObservableProperty]
        private string _middleName = string.Empty;
        [ObservableProperty]
        private string _lastName = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private string _phoneNumber = string.Empty;
        [ObservableProperty]
        private string _birthPlace = string.Empty;
        [ObservableProperty]
        private string _notes = string.Empty;

        [ObservableProperty]
        private bool _isOpenSearchParameters;
        [ObservableProperty]
        private bool _isPropertyChanged;

        [ObservableProperty]
        string _gendertext = "Female";
        [ObservableProperty]
        public GenderType[] genderTypeList = [GenderType.Female, GenderType.Male, GenderType.Unknown];
        [ObservableProperty]
        private GenderType _gender;

        [ObservableProperty]
        private DateTimeOffset _birthDateOffset = new DateTimeOffset(DateTime.Now.AddYears(-20));
        public DateTime BirthDate
        {
            get => BirthDateOffset.DateTime;
            set
            {
                BirthDateOffset = new DateTimeOffset(value);
            }
        }

        public UserProfilePersonBindable()
        {
            PropertyChanged += UserProfilePersonBindablePropertyChanged;
            Gender = GenderTypeList[0];
        }        

        public UserProfilePersonBindable(int profileId) : this()
        {
            _profileId = profileId;
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

        partial void OnGenderChanged(GenderType oldValue, GenderType newValue)
        {
            switch (newValue)
            {
                case GenderType.Female:
                    Gendertext = "Female";
                    break;
                case GenderType.Male:
                    Gendertext = "Male";
                    break;
                case GenderType.Unknown:
                    Gendertext = "Unknown";
                    break;
                default:
                    Gendertext = "default";
                    break;
            }
        }


        //[ObservableProperty]
        //bool _genderMale;
        //partial void OnGenderMaleChanged(bool oldValue, bool newValue)
        //{
        //    if(newValue)
        //        Gender = GenderType.Male;
        //    else
        //        Gender = GenderType.Female;
        //}
        // [ObservableProperty]
        //bool _genderFeMale = true;
        //partial void OnGenderFeMaleChanged(bool oldValue, bool newValue)
        //{
        //    if(newValue)
        //        Gender = GenderType.Female;
        //    else
        //        Gender = GenderType.Male;
        //}
        //private GenderType _gender = GenderType.Female;
        //public GenderType Gender 
        //{ 
        //    get => _gender;
        //    set {
        //        if(SetProperty(ref _gender,value))
        //        {
        //            if(Gender == GenderType.Female)
        //           {  
        //             GenderMale = false;
        //             GenderFeMale = true;
        //             Gendertext = "Female";
        //           }else if(Gender == GenderType.Male){
        //               GenderMale = true;
        //             GenderFeMale = false;
        //             Gendertext = "Male";
        //           }
        //        }
        //    }
        //}

        public override string ToString()
        {
            return Title;
        }
    }
}
