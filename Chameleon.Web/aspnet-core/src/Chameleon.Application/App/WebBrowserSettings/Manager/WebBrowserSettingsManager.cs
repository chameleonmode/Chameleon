using Abp.Domain.Repositories;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    public class WebBrowserSettingsManager
        : IWebBrowserSettingsManager
    {
        private const decimal CanvasIncrement = 0.01M;
        private readonly IRepository<WebBrowserSetting> _repository;
        private readonly IWebBrowserUserAgentAppService _webBrowserUserAgentService;

        public WebBrowserSettingsManager(
            IRepository<WebBrowserSetting> repository,
            IWebBrowserUserAgentAppService webBrowserUserAgentService
            )
        {
            _repository = repository;
            _webBrowserUserAgentService = webBrowserUserAgentService;
        }

        public int Insert(WebBrowserSetting entity)
        {
            if (entity.UserAgentId == 0)
            {
                entity.UserAgentId = _webBrowserUserAgentService
                    .GetDefaultUserAgentId();
            }

            entity.Canvas = GetNextCanvas();
            return _repository.InsertAndGetId(entity);
        }

        private decimal GetNextCanvas()
        {
            return GetCanvas() + CanvasIncrement;
        }

        private decimal GetCanvas()
        {
            return _repository
                .GetAll()
                .Select(setting => setting.Canvas)
                .OrderByDescending(setting => setting)
                .FirstOrDefault();
        }
    }
}
