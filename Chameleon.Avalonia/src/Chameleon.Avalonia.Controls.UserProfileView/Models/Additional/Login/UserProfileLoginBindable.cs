using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.UserProfiles.Additional;
using System.ComponentModel;

namespace Chameleon.Controls.UserProfileView.Models.Additional
{
    public class UserProfileLoginBindable 
        : ObservableObjectBase
        , IUserProfileLogin
    {
        public UserProfileLoginBindable()
        {
            PropertyChanged += UserProfilePersonBindablePropertyChanged;
        }

        public UserProfileLoginBindable(int profileId)
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
            set => SetProperty(ref _title, value); 
        }

        private string _webSite;
        public string WebSite 
        { 
            get => _webSite;
            set => SetProperty(ref _webSite, value); 
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _userName;
        public string UserName 
        { 
            get => _userName; 
            set => SetProperty(ref _userName, value); 
        }

        private string _password;
        public string Password 
        {
            get => _password; 
            set => SetProperty(ref _password, value); 
        }

        private string _notes;
        public string Notes 
        { 
            get => _notes;
            set => SetProperty(ref _notes, value); 
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
