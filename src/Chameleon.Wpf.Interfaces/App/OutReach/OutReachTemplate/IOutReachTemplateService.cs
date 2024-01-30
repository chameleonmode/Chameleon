using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.OutReach
{
    public interface IOutReachTemplateService
         : ISingletonDependency
    {
        IOutReachTemplate Get(int outReachTemplateId);
        IOutReachTemplates GetAll();
        IOutReachTemplate Create(IOutReachTemplate outReachTemplate);
        void Delete(IOutReachTemplate outReachTemplate);
        void Save(IOutReachTemplate outReachTemplate);
    }
}
