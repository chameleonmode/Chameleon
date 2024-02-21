using System.Threading.Tasks;
using Chameleon.Configuration.Dto;

namespace Chameleon.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
