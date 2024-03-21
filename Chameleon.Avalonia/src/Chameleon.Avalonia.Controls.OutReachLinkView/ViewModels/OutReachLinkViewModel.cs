
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.OutReachLinkView.ViewModels;

public partial class OutReachLinkViewModel
    : DialogBase
    , IOutReachLinkViewModel
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IUserProfileService _userProfileService;
    private readonly IProfileOutReachLinkService _profileOutReachLinkService;

    public OutReachLinkViewModel(
        IEventAggregator eventAggregator,
        IUserProfileService userProfileService,
        IProfileOutReachLinkService profileOutReachLinkService)
    {
        _eventAggregator = eventAggregator;
        _userProfileService = userProfileService;
        _profileOutReachLinkService = profileOutReachLinkService;
    }

    [RelayCommand]
    private void OnDiscard()
    {
        Close();
    }

    private void Close()
    {
        //_eventAggregator
        //    .GetEvent<CloseDialogWindowEvent>()
        //    .Publish((int)ButtonResult.Cancel);
    }

    [RelayCommand]
    private void Email()
    {
        var outReachTemplate = new OutReachTemplate
        {
            ContactEmail = ContactEmail,
            ContactName = ContactName
        };

        _eventAggregator
            .GetEvent<OpenOutReachTemplateEvent>()
            .Publish(new OutReachTemplateEventArgs(outReachTemplate));

        Close();
    }

    [RelayCommand]
    private void Save()
    {
        var outReachLink = new ProfileOutReachLink
        {
            ContactEmail = ContactEmail,
            ContactName = ContactName,
            Notes = Notes,
            Url = Url,
            UrlName = UrlName,
            Status = Status,
            UrlType = UrlType,
            ProfileId = _profileId,
            Id = Id,
            Twitter = Twitter,
            Facebook = Facebook,
            Linkedin = Linkedin,
            OtherSocial = OtherSocial,
            ReminderNotes = ReminderNotes,
            ReminderDatetime = ReminderDatetime
        };

        if (HasId)
        {
            if (_userProfile == null)
            {
                _userProfile = _userProfileService.Get(_profileId);
            }

            _profileOutReachLinkService.Update(outReachLink);
            _eventAggregator.GetEvent<SavedOutReachLinkEvent>()
                .Publish(new OutReachEventArgs(UserProfile));
        }
        else
        {
            _profileOutReachLinkService.Save(outReachLink);

            _eventAggregator
                .GetEvent<AddOutReachLinkEvent>()
                .Publish();
        }

        Close();
    }

    private IUserProfile _userProfile;
    public IUserProfile UserProfile
    {
        get => _userProfile;
        set => SetProperty(ref _userProfile, value);
    }

    public bool HasId => Id > 0;

    private IProfileOutReachLink _profileOutReachLink;
    public IProfileOutReachLink ProfileOutReachLink
    {
        get => _profileOutReachLink;
        set => SetProperty(ref _profileOutReachLink, value);
    }

    private string _url;
    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

    private string _urlName;
    public string UrlName
    {
        get => _urlName;
        set => SetProperty(ref _urlName, value);
    }

    private string _contactName;
    public string ContactName
    {
        get => _contactName;
        set => SetProperty(ref _contactName, value);
    }

    private string _contactEmail;
    public string ContactEmail
    {
        get => _contactEmail;
        set => SetProperty(ref _contactEmail, value);
    }

    private string _notes;
    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public OutReachLinkStatus _status;
    public OutReachLinkStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    private OutReachLinkUrlType _urlType;
    public OutReachLinkUrlType UrlType
    {
        get => _urlType;
        set => SetProperty(ref _urlType, value);
    }

    private int _profileId;
    int IUserProfileRequiredEntity.ProfileId
    {
        get => _profileId;
        set => _profileId = value;
    }
    public int Id { get; set; }

    private string _twitter;
    public string Twitter
    {
        get => _twitter;
        set => SetProperty(ref _twitter, value);
    }

    private string _linkedin;
    public string Linkedin
    {
        get => _linkedin;
        set => SetProperty(ref _linkedin, value);
    }

    private string _facebook;
    public string Facebook
    {
        get => _facebook;
        set => SetProperty(ref _facebook, value);
    }

    private string _otherSocial;
    public string OtherSocial
    {
        get => _otherSocial;
        set => SetProperty(ref _otherSocial, value);
    }

    private string _reminderNotes;
    public string ReminderNotes
    {
        get => _reminderNotes;
        set => SetProperty(ref _reminderNotes, value);
    }

    private DateTime? _reminderDatetime;
    public DateTime? ReminderDatetime
    {
        get => _reminderDatetime;
        set => SetProperty(ref _reminderDatetime, value);
    }

    private bool _isEnable;
    public bool IsEnable
    {
        get => _isEnable;
        set => SetProperty(ref _isEnable, value);
    }

    public override Task<IContentDialogResult> ShowAsync()
    {
        throw new NotImplementedException();
    }
}
