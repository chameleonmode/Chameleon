using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Core.Extensions;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using System;

namespace Chameleon.Application.Events
{
    public class OutReachEventHandler : IOutReachEventHandler
    {
        private readonly IMapper _mapper;
        private readonly IPopupDialogService _dialogManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProfileOutReachLinkService _profileOutReachLinkService;
        private readonly IUserProfileOutReachRssesService _userProfileOutReachRssesService;
        private readonly IDialogWindowsService _dialogWindowsService;
        private readonly IOutReachLinkView _outReachLinkView;

        public OutReachEventHandler(
            IMapper mapper,
            IPopupDialogService dialogManager,
            IEventAggregator eventAggregator,
            IProfileOutReachLinkService profileOutReachLinkService,
            IUserProfileOutReachRssesService userProfileOutReachRssesService,
            IDialogWindowsService dialogWindowsService, 
            IOutReachLinkView outReachLinkView)
        {
            _mapper = mapper;
            _dialogManager = dialogManager;
            _eventAggregator = eventAggregator;
            _profileOutReachLinkService = profileOutReachLinkService;
            _userProfileOutReachRssesService = userProfileOutReachRssesService;
            _dialogWindowsService = dialogWindowsService;
            _outReachLinkView = outReachLinkView;

            _eventAggregator
                .GetEvent<OutReachRssOpenEvent>()
                .Subscribe(args => OpenOutReachRss(args.UserProfileOutReachRss, args.UserProfile));

            _eventAggregator
               .GetEvent<DeleteOutReachRssEvent>()
               .Subscribe(args => DeleteOutReachRss(args.UserProfileOutReachRss));

            _eventAggregator
                .GetEvent<OpenOutReachLinkEvent>()
                .Subscribe(args => OpenOutReachLink(args.ProfileOutReachLink, args.UserProfile, args.Edit, args.IsDisable));

            _eventAggregator
               .GetEvent<DeleteOutReachLinkEvent>()
               .Subscribe(args => DeleteOutReachLink(args.ProfileOutReachLink));            
        }

        private void DeleteOutReachLink(IProfileOutReachLink profileOutReachLink)
        {
            var outReachLink = _mapper.Map<ProfileOutReachLink>(profileOutReachLink);
            _profileOutReachLinkService.Delete(outReachLink);
        }

        private void OpenOutReachLink(IProfileOutReachLink profileOutReachLink, IUserProfile userProfile, bool edit, bool isDisable)
        {
            Action<IProfileOutReachLink> initialize = viewModel =>
            {
                viewModel.ContactEmail = profileOutReachLink.ContactEmail;
                viewModel.ContactName = profileOutReachLink.ContactName;
                viewModel.Url = profileOutReachLink.Url;
                viewModel.UrlName = profileOutReachLink.UrlName;
                viewModel.Notes = profileOutReachLink.Notes;
                viewModel.Status = profileOutReachLink.Status;
                viewModel.UrlType = profileOutReachLink.UrlType;
                viewModel.Id = profileOutReachLink.Id;
                viewModel.ProfileId = userProfile.Id;

                viewModel.Twitter = profileOutReachLink.Twitter;
                viewModel.Facebook = profileOutReachLink.Facebook;
                viewModel.Linkedin = profileOutReachLink.Linkedin;
                viewModel.OtherSocial = profileOutReachLink.OtherSocial;
                viewModel.ReminderNotes = profileOutReachLink.ReminderNotes;
                viewModel.ReminderDatetime = profileOutReachLink.ReminderDatetime;
                viewModel.IsEnable = !isDisable;
            };

            var title = !edit ? "SAVE TO OUTREACH" : "EDIT LINK";

            _dialogWindowsService.ShowDialogWindow(_outReachLinkView, title, initialize);
        }

        private void DeleteOutReachRss(IUserProfileOutReachRss userProfileOutReachRss)
        {
            var outReachRss = _mapper.Map<UserProfileOutReachRssBindable>(userProfileOutReachRss);
            _userProfileOutReachRssesService.DeleteOutReachRss(outReachRss);
        }

        private void OpenOutReachRss(IUserProfileOutReachRss outReachRss, IUserProfile userProfile)
        {
            _dialogManager
                .CreateDialog<IOutReachRssView, IUserProfileOutReachRss>(viewModel =>
                {
                    // TODO: viewModel.OutReachRss = outReachRss;
                    viewModel.RssLink = outReachRss.RssLink;
                    viewModel.RssName = outReachRss.RssName;
                    viewModel.ProfileId = userProfile.Id;
                    viewModel.Notes = outReachRss.Notes;
                    viewModel.Status = outReachRss.Status;
                    viewModel.ContactEmail = outReachRss.ContactEmail;
                    viewModel.ContactName = outReachRss.ContactName;
                    viewModel.Id = outReachRss.Id;
                })
                .Show();
        }
    }
}
