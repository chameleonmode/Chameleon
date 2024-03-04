using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.OutReach;

namespace Chameleon.Interfaces.App.OutReach;

public interface IOutReachTemplateItemViewModel
    : ISingletonDependency
{
    IOutReachTemplate OutReachTemplate { get; set; }
    string ItemContent { get; set; }
    string ItemName { get; set; }
}
