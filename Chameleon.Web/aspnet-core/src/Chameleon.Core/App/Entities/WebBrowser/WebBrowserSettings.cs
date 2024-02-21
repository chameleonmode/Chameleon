using Abp.Domain.Entities;
using System;

namespace Chameleon.App.Entities
{
    public class WebBrowserSetting : Entity
    {
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
                    throw new ArgumentOutOfRangeException(
                        nameof(Canvas), 
                        "value should be >= 0");
                }
                _canvas = value;
            }
        }

        public int? UserAgentId { get; set; }
        public virtual WebBrowserUserAgent UserAgent { get; set; }
    }
}
