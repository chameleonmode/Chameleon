using Chameleon.Controls.ImportExport.ViewModels;
using Chameleon.Interfaces.Ioc;
using System.Threading.Tasks;

namespace Chameleon.Controls.ImportExport.Services
{
    public interface IUserProfileViewModelImporter : ISingletonDependency
    {
        Task ImportAsync(ImportColumnViewModels viewModels, int? folderId = null);
    }
}
