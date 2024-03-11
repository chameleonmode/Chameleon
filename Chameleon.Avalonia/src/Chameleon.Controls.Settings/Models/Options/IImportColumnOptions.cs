using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.ImportExport.Models
{
    public interface IImportColumnOptions : ITransientDependency
    {
        IImportColumnOption Selected { get; set; }
    }    
}
