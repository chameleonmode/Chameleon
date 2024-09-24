using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.ImportExport.Models
{
    public interface IImportColumnOptions : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        IImportColumnOption Selected { get; set; }
    }    
}
