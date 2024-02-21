using Abp.AspNetCore.Mvc.Authorization;
using Chameleon.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Chameleon.Web.Host.Controllers
{
    public class LogsController : ChameleonControllerBase
    {
        [AbpMvcAuthorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
