using Chameleon.Interfaces.WebBrowser;
using System;

namespace Chameleon.Domain.Entities
{
    public class WebBrowserUserAgent 
        : IWebBrowserUserAgent
    {
        public const string CustomUserAgentName = "Custom";

        public int Id { get; set; }
        
        private string _name = CustomUserAgentName;
        public string Name 
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = CustomUserAgentName;
                }
                _name = value;
            }
        }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException();
                }
                _value = value;
            }
        }
    }
}
