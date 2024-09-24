using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public interface IImportColumnOption : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        bool IsUsed { get; set; }
        void Map(IUserProfile profile, string input);
    }    
}
