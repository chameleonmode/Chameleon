using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Chameleon.Configuration.Dto;

namespace Chameleon.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : ChameleonAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
