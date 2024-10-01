using Chameleon.Interfaces.App.Prospector;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserProfiles.Additional;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Interfaces.WordPress;
using Chameleon.Interfaces.YouTube;
using Chameleon.lib.WebBrowser.Interfaces;

using System;
using System.Collections.Generic;

using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.Domain.Entities {
	public class UserProfile : IUserProfile {
		private int _id;
		public int Id {
			get => _id;
			set {
				if (value <= 0) {
					return;
					//throw new ArgumentException();
				}
				_id = value;
			}
		}

		public string? Title { get; set; }
		public bool IsFavourite { get; set; }

		private int? _folderId;
		public int? FolderId {
			get => _folderId;
			set {
				if (value <= 0) {
					// throw new ArgumentException();
					return;
				}
				_folderId = value;
			}
		}
		private long? _creatorUserId;
		public long? CreatorUserId {
			get => _creatorUserId;
			set {
				if (value <= 0) {
					return;
					//throw new ArgumentException();
				}
				_creatorUserId = value;
			}
		}

		private string _notes = string.Empty;
		public string Notes {
			get => _notes;
			set => _notes = value ?? string.Empty;
		}
		private double? _limitCache;
		public double? LimitCache {
			get => _limitCache ?? 100;
			set {
				if (value <= 0) {
					return;
					//throw new ArgumentException();
				}
				_limitCache = value;
			}
		}

		private IProxySettings _proxy = new ProxySettings();
		public IProxySettings Proxy {
			get => _proxy;
			set => _proxy = value ?? new ProxySettings();
		}

		private IWebBrowserSettings _webBrowser = new WebBrowserSettings();
		public IWebBrowserSettings WebBrowser {
			get => _webBrowser;
			set => _webBrowser = value ?? new WebBrowserSettings();
		}

		private IYouTubeSettings _youtubeSettings = new YoutubeSettings();
		public IYouTubeSettings YoutubeSettings {
			get => _youtubeSettings;
			set => _youtubeSettings = value ?? new YoutubeSettings();
		}

		private IWordPressSettings _wordPressSettings = new WordPressSettings();
		public IWordPressSettings WordPressSettings {
			get => _wordPressSettings;
			set => _wordPressSettings = value ?? new WordPressSettings();
		}

		public IList<IUserProfileBusiness> Businesses { get; }
				= new List<IUserProfileBusiness>();

		public IList<IUserProfileLogin> Logins { get; }
				= new List<IUserProfileLogin>();

		public IList<IUserProfilePerson> Persons { get; }
				= new List<IUserProfilePerson>();

		public IList<IUserProfileAddress> Addresses { get; }
				= new List<IUserProfileAddress>();

		public IList<IUserProfileOutReachRss> OutReachRsses { get; }
				= new List<IUserProfileOutReachRss>();

		public IList<IUserProfileProspectorBlogsOfInterest> ProspectorBlogsOfInterest { get; }
				= new List<IUserProfileProspectorBlogsOfInterest>();

		public string[] PermissionNames { get; set; } = [];

		public bool HasPermission(string permissionName)
		{
			return PermissionNames.Contains(permissionName);
		}

		public bool Navigated { get; set; }
		public string IsChromeRunning { get; set; } = "False";
		public string IsBraveRunning { get; set; } = "False";
		public string IsFFRunning { get; set; } = "False";

		public Dictionary<SystemBrowserType, ISysBrowserInstance?> SBI { get; set; } = new Dictionary<SystemBrowserType, ISysBrowserInstance?>(){
			{ SystemBrowserType.Chrome, null },
			{ SystemBrowserType.Firefox, null },
			{ SystemBrowserType.Brave, null }
		};
	}
}
