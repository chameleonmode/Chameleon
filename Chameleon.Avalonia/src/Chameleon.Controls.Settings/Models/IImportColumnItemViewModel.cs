using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.ImportExport.Models
{
    public interface IImportColumnItemViewModel : ITransientDependency
    {
        string Value { get; set; }
    }    
}
