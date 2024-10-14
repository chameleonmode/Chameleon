using Chameleon.Interfaces.App.Prospector;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.Prospector;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Interfaces.Sys;

using System;

namespace Chameleon.Application.Events {
	public class ProspectorEventHandler : IProspectorEventHandler
    {
        //private readonly IDialogWindowsService _dialogWindowsService;
        //private readonly IBlogOfInterestView _blogOfInterestView;
        //private readonly IPopupDialogService _dialogManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserProfileProspectorBlogsOfInterestsService _prospectorBlogsOfInterestsService;

        public ProspectorEventHandler(
            IEventAggregator eventAggregator,
            IUserProfileProspectorBlogsOfInterestsService prospectorBlogsOfInterestsService)
        {
            _eventAggregator = eventAggregator;
            _prospectorBlogsOfInterestsService = prospectorBlogsOfInterestsService;

            _eventAggregator
                .GetEvent<BlogOfInterestOpenEvent>()
                .Subscribe(args => OpenBlogOfInterest(args.UserProfileProspectorBlogsOfInterest, args.UserProfile, args.Title));

            _eventAggregator
               .GetEvent<BlogOfInterestDeleteEvent>()
               .Subscribe(args => DeleteBlogOfInterest(args.UserProfileProspectorBlogsOfInterest));            
        }

        private void DeleteBlogOfInterest(IUserProfileProspectorBlogsOfInterest blogsOfInterest)
        {
            _prospectorBlogsOfInterestsService.DeleteProspectorBlogsOfInterest(blogsOfInterest);
        }
    
        private void OpenBlogOfInterest(IUserProfileProspectorBlogsOfInterest blogsOfInterest
            , IUserProfile userProfile
            , string title)
        {
            //Action<IUserProfileProspectorBlogsOfInterest> initialize = viewModel =>
            //{
            //    viewModel.Name = blogsOfInterest.Name;
            //    viewModel.Id = blogsOfInterest.Id;
            //    viewModel.ProfileId = userProfile.Id;
            //    viewModel.Value = blogsOfInterest.Value;
            //    viewModel.Type = blogsOfInterest.Type;
            //};

            //_dialogWindowsService.ShowDialogWindow(_blogOfInterestView, title, initialize);
        }
    }
}
