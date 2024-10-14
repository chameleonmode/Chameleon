//using Chameleon.Interfaces.DialogWindows;
//using Chameleon.Interfaces.OutReach;
//using Chameleon.Interfaces.Rss;
//using Chameleon.Interfaces.SocialNetwork;
//using Chameleon.Interfaces.UserProfiles;
//using Chameleon.Interfaces.WebBrowser;
//using Chameleon.Interfaces.Windows;
//using Chameleon.lib.Common.Interfaces.Sys;

//using System;

//namespace Chameleon.Application.Events {
//	public class OpenBrowserEventHandler
//        : IOpenBrowserEventHandler
//    {
//        //private readonly IMainWindow _mainWindow;
//        //TODO: private readonly IUserProfileView _userProfileView;
//        private readonly IEventAggregator _eventAggregator;
//        //private readonly IOutReachRssView _outReachRssView;
//        //private readonly IDialogWindowsService _dialogWindowsService;
//        //private readonly IRssFeedView _rssFeedView;

//        public OpenBrowserEventHandler(
//            IEventAggregator eventAggregator
//            )
//        {
//            //_userProfileView = userProfileView;
//            _eventAggregator = eventAggregator;
//            _eventAggregator
//                .GetEvent<SocialNetworkShareEvent>()
//                .Subscribe(args => OnOpenUserBrowser(args));

//            _eventAggregator
//                .GetEvent<WebBrowserLoadUriEvent>()
//                .Subscribe(args => OnOpenUserBrowser(args));

//            _eventAggregator
//                .GetEvent<WebBrowserOpenNewTabLoadUriEvent>()
//                .Subscribe(args => OnOpenUserBrowserNewTab(args));

//            _eventAggregator
//                .GetEvent<RssFeedOpenEvent>()
//                .Subscribe(args => OnOpenRssFeed(args.UserProfile, args.TextFeeds));
//        }

//        private void OnOpenRssFeed(IUserProfile userProfile, string textFeeds)
//        {
//            //Action<Interfaces.App.Rss.IRssFeedViewModel> initialize = viewModel =>
//            //{
//            //    viewModel.UserProfile = userProfile;
//            //    viewModel.RSSFeedsText = textFeeds;
//            //};

//            //_dialogWindowsService.ShowDialogWindow(_rssFeedView, "ADD RSS FEED", initialize);
//        }

//        private void OnOpenUserBrowserNewTab(WebBrowserEventArgs args)
//        {
//            //_userProfileView.SetUserProfile(args.UserProfile, UserProfileViewTab.Browser);
//            //_userProfileView.OpenWebBrowserNewTab(args.Url.ToString());
//            //_mainWindow.SetContent(_userProfileView, args.UserProfile.Title);
//        }

//        private void OnOpenUserBrowser(WebBrowserEventArgs args)
//        {
//            OnOpenUserBrowser(args.UserProfile, args.Url);
//        }

//        private void OnOpenUserBrowser(SocialNetworkShareEventArgs args)
//        {
//            OnOpenUserBrowser(args.UserProfile, args.Url);
//        }

//        private void OnOpenUserBrowser(IUserProfile profile, Uri url)
//        {
//            //_userProfileView.SetUserProfile(profile, UserProfileViewTab.Browser);
//            ////_userProfileView.WebBrowser.Load(url.ToString());
//            //_mainWindow.SetContent(_userProfileView, profile.Title);
//        }
//    }
//}
