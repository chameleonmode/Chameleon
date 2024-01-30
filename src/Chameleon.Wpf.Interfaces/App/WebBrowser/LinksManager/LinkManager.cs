using Chameleon.Interfaces.UserProfiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chameleon.Interfaces.WebBrowser
{
    public class LinkManager : ILinkManager
    {
        public string Url { get; set; }
        public string NameUrl { get; set; }
        public IUserProfile UserProfile { get; set; }
    }
}
