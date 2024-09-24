using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.ImportExport.Models
{
    public interface IImportColumnItemViewModel : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string Value { get; set; }
    }    
}
