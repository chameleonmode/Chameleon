using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public interface IImportColumnOption : ITransientDependency
    {
        bool IsUsed { get; set; }
        void Map(IUserProfile profile, string input);
    }    
}
