
using Chameleon.Core.Util;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Rss;
using Chameleon.Interfaces.Common;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Chameleon.Avalonia.Controls.RssFeed.ViewModels;

public class RssFeedViewModel
     : DialogBase
    , IRssFeedViewModel
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IUrlConfiguration _configuration;
    public RssFeedViewModel(
       IEventAggregator eventAggregator,
       IUrlConfiguration configuration
       )
    {
        _eventAggregator = eventAggregator;
        _configuration = configuration;

        DiscardCommand = new DelegateCommand(OnCloseDialog);
        SaveChangesCommand = new DelegateCommand(OnSaveFeeds);
        OpenUpgradePlanUrlCommand = new DelegateCommand(OpenUpgradePlanUrl);

        _eventAggregator
            .GetEvent<RestrictContentEvent>()
            .Subscribe(args => MaxRssCount = args.Limits.ContentDiscoveryLimits.MaxRssCount);
    }

    #region Commands
    public DelegateCommand DiscardCommand { get; }
    public DelegateCommand SaveChangesCommand { get; }
    public DelegateCommand OpenUpgradePlanUrlCommand { get; }

    #endregion


    #region Methods
    private void OnCloseDialog()
    {
        _eventAggregator
            .GetEvent<CloseDialogWindowEvent>()
            .Publish((int)ButtonResult.OK);
    }
    private void OnSaveFeeds()
    {
        _eventAggregator
            .GetEvent<SaveRssFeedsEvent>()
            .Publish(new RssFeedsEventArgs(UserProfile, RSSFeedsText));

        _eventAggregator
            .GetEvent<CloseDialogWindowEvent>()
            .Publish((int)ButtonResult.OK);
    }

    private void OpenUpgradePlanUrl()
    {
        ProcessesUtil.GoToUrlDefault(_configuration.PricingUrl);
    }

    #endregion


    #region Properties

    private int _profileId;
    public int ProfileId
    {
        get => _profileId;
        set => _profileId = value;
    }

    private IUserProfile _userProfile;
    public IUserProfile UserProfile
    {
        get => _userProfile;
        set => SetProperty(ref _userProfile, value);
    }
    private string _urls;
    public string Url
    {
        get => _urls;
        set => SetProperty(ref _urls, value);
    }
    public int Id { get; set; }

    private string _rssFeedsText;
    public string RSSFeedsText
    {
        get => _rssFeedsText;
        set
        {
            SetProperty(ref _rssFeedsText, value);
            OnPropertyChanged(nameof(RSSFeedsText));
        }
    }

    private int _maxRssCount;
    public int MaxRssCount
    {
        get => _maxRssCount;
        set
        {
            SetProperty(ref _maxRssCount, value);
            OnPropertyChanged(nameof(IsRssCountUnlimited));
        }
    }
    public bool IsRssCountUnlimited => MaxRssCount == int.MaxValue;

    public override Task<IContentDialogResult> ShowAsync()
    {
        throw new NotImplementedException();
    }

    #endregion
}