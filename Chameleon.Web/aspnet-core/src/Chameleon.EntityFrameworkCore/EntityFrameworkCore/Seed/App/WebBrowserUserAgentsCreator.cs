using System.Linq;
using Chameleon.App.Entities;

namespace Chameleon.EntityFrameworkCore.Seed.Host.App
{
    public class WebBrowserUserAgentsCreator : ApplicationBaseCreator
    {
        // http://www.useragentstring.com/
        private readonly WebBrowserUserAgent[] _userAgents = new []
        {
            new WebBrowserUserAgent
            {
                Name = "Chrome 70.0.3538.77",
                Value = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36"
            },
            new WebBrowserUserAgent
            {
                Name = "Chrome 55.0.2919.83",
                Value = "Mozilla/5.0 (X11; Ubuntu; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2919.83 Safari/537.36"
            },
            new WebBrowserUserAgent
            {
                Name = "Internet Explorer 11.0",
                Value = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; AS; rv:11.0) like Gecko"
            },
            new WebBrowserUserAgent
            {
                Name = "Internet Explorer 6.0",
                Value = "Mozilla/5.0 (Windows; U; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)"
            }
        };

        public WebBrowserUserAgentsCreator(ChameleonDbContext context)
            : base(context)
        {
        }

        public override void Run()
        {
            if (Context.WebBrowserUserAgents.Any())
            {
                return;
            }

            foreach (var item in _userAgents)
            {
                Create(item);
            }
            SaveChanges();
        }

        private void Create(WebBrowserUserAgent userAgent)
        {
            var table = Context.WebBrowserUserAgents;
            if (table.Any(e => e.Name == userAgent.Name))
            {
                return;
            }
            
            table.Add(userAgent);
        }
    }
}
