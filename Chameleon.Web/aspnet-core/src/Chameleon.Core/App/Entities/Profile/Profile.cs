using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;

namespace Chameleon.App.Entities
{
    public class Profile : FullAuditedEntity
    {
        public string Title { get; set; }
        public string Notes { get; set; }
        public string YoutubeApiKey { get; set; }
        public string YoutubeClientId { get; set; }
        public string YoutubeClientSecret { get; set; }
        
        public WordPressSettings WordPressSettings { get; set; }

        public bool IsFavourite { get; set; }
        private double? _limitCache;
        public double? LimitCache
        {
            get 
            {
                if (_limitCache == null)
                {
                    _limitCache = 100;
                }
                return _limitCache;
            }
            set => _limitCache = value;
        }

        public int? FolderId { get; set; }
        public virtual Folder Folder { get; set; }

        public int ProxyId { get; set; }

        private Proxy _proxy;
        public virtual Proxy Proxy
        {
            get
            {
                if (_proxy == null)
                {
                    _proxy = new Proxy();
                }
                return _proxy;
            }
            set => _proxy = value;
        }

        public int WebBrowserSettingId { get; set; }
        
        private WebBrowserSetting _webBrowserSetting;
        public virtual WebBrowserSetting WebBrowserSetting 
        {
            get
            {
                if (_webBrowserSetting == null)
                {
                    _webBrowserSetting = new WebBrowserSetting();
                }
                return _webBrowserSetting;
            }
            set => _webBrowserSetting = value;
        }

        public virtual ICollection<Address> Addresses { get; protected set; }
        public virtual ICollection<Person> Persons { get; protected set; }
        public virtual ICollection<Credential> Credentials { get; protected set; }
        public virtual ICollection<Business> Businesses { get; protected set; }
        public virtual ICollection<RSSFeed> RSSFeeds { get; protected set; }
        public virtual ICollection<ProfileAssistant> ProfilesAssistants { get; protected set; }
    }
}
