using Chameleon.Interfaces.Ioc;
using System.Threading.Tasks;

namespace Chameleon.Controls.ImportExport.Services
{
    public interface IUserProfileFileSystemImporter : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        Task ImportAsync();
    }
}
