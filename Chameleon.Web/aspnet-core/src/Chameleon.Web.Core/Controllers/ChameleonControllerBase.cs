using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Chameleon.Controllers
{
    public abstract class ChameleonControllerBase: AbpController
    {
        protected ChameleonControllerBase()
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
