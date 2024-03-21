
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Prospector;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.Prospector;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Chameleon.Avalonia.Controls.BlogOfInterestView.ViewModels;

public class BlogOfInterestViewModel
       : DialogBase
       , IBlogOfInterestViewModel
{
    private readonly IUserProfileProspectorBlogsOfInterestsService _prospectorBlogsOfInterestsService;
    private readonly IUserProfileService _userProfileService;
    private readonly IEventAggregator _eventAggregator;

    public BlogOfInterestViewModel(
        IUserProfileProspectorBlogsOfInterestsService prospectorBlogsOfInterestsService,
        IUserProfileService userProfileService,
        IEventAggregator eventAggregator
        )
    {
        _eventAggregator = eventAggregator;
        _userProfileService = userProfileService;
        _prospectorBlogsOfInterestsService = prospectorBlogsOfInterestsService;

        BlogOfInterestSaveCommand = new DelegateCommand(BlogOfInterestSave);
        BlogOfInterestCancelCommand = new DelegateCommand(BlogOfInterestCancel);
    }

    public bool HasId => Id > 0;

    private IUserProfile _userProfile;
    public IUserProfile UserProfile
    {
        get => _userProfile;
        set => SetProperty(ref _userProfile, value);
    }

    public int ProfileId => UserProfile == null ? 0 : UserProfile.Id;

    private void BlogOfInterestCancel()
    {
        Close();
    }

    private void Close()
    {
        _eventAggregator
            .GetEvent<CloseDialogWindowEvent>()
            .Publish((int)ButtonResult.Cancel);
    }

    private void BlogOfInterestSave()
    {
        var blogsOfInterest = new UserProfileProspectorBlogsOfInterest
        {
            Name = Name,
            Value = Value,
            Type = Type,
            ProfileId = _profileId,
            Id = Id
        };

        if (HasId)
        {
            if (UserProfile == null)
            {
                UserProfile = _userProfileService.Get(blogsOfInterest.ProfileId);
            }

            _prospectorBlogsOfInterestsService.SaveProspectorBlogsOfInterest(blogsOfInterest);
            _eventAggregator.GetEvent<BlogOfInterestSaveEvent>()
                .Publish(new BlogOfInterestEventArgs(blogsOfInterest, UserProfile));
        }
        else
        {
            _prospectorBlogsOfInterestsService
            .AddProspectorBlogsOfInterest(blogsOfInterest);

            _eventAggregator
                .GetEvent<BlogOfInterestAddEvent>()
                .Publish();
        }

        Close();
    }
    public DelegateCommand BlogOfInterestSaveCommand { get; }
    public DelegateCommand BlogOfInterestCancelCommand { get; }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _value;
    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    private ProspectorBlogsOfInterestTypes _type;
    public ProspectorBlogsOfInterestTypes Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    private int _profileId;
    int IUserProfileRequiredEntity.ProfileId
    {
        get => _profileId;
        set => _profileId = value;
    }
    public int Id { get; set; }

    public override Task<IContentDialogResult> ShowAsync()
    {
        throw new NotImplementedException();
    }
}