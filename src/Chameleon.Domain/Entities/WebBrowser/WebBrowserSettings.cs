using Chameleon.Interfaces.WebBrowser;
using System;

namespace Chameleon.Domain.Entities
{
    public class WebBrowserSettings : IWebBrowserSettings
    {
        public WebBrowserSettings()
        {
            Canvas = 0.1M;
        }

        public bool WebRTC { get; set; }
        public bool WebGL { get; set; }
        public bool Tracking { get; set; }
        public bool Flash { get; set; }

        private decimal _canvas;
        public decimal Canvas
        {
            get => _canvas;
            set
            {
                if (value < 0)
                {
                    //throw new ArgumentOutOfRangeException(nameof(Canvas), "value should be > 0");
                    return;
                }
                _canvas = value;
            }
        }

        private int? _userAgentId;
        public int? UserAgentId 
        {
            get => UserAgent?.Id ?? _userAgentId;
            set
            {
                if (value == null)
                {
                    UserAgent = null;
                    _userAgentId = null;
                    return;
                }

                if (value.Value <= 0)
                {
                    //throw new ArgumentOutOfRangeException(nameof(UserAgentId), "value should be > 0");
                    return;
                }

                if (UserAgent != null && UserAgent.Id != value.Value)
                {
                    UserAgent = null;
                }
                _userAgentId = value;
            }
        }

        public IWebBrowserUserAgent UserAgent { get; set; }
    }
}
