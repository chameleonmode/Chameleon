using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.UserProfiles.Additional;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public class UserProfileBusinessBindable
        : ObservableObject
        , IUserProfileBusiness
    {
        public UserProfileBusinessBindable()
        {
            PropertyChanged += UserProfilePersonBindablePropertyChanged;
        }

        public UserProfileBusinessBindable(int profileId)
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

        private string _companyName = string.Empty;
        public string CompanyName 
        { 
            get => _companyName;
            set => SetProperty(ref _companyName,value); 
        }

        private string _department = string.Empty;
        public string Department 
        { 
            get => _department;
            set => SetProperty(ref _department,value); 
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber 
        { 
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber,value); 
        }

        private string _webSite = string.Empty;
        public string WebSite 
        { 
            get => _webSite; 
            set => SetProperty(ref _webSite,value); 
        }

        private string _notes = string.Empty;
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
