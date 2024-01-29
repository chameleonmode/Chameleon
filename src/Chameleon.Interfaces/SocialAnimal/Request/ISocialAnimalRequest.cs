using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.SocialAnimal
{
    public interface ISocialAnimalRequest : ISingletonDependency
    {
        IQuery Query { get; set; }
        long From_date { get; set; }
        long To_date { get; set; }
        // TODO:use enum
        string Period { get; set; }
        IFilter Filter { get; set; }
    }
}
