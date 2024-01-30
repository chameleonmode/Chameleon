using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Interfaces.SocialAnimal
{
    public interface ISocialAnimalService : ISingletonDependency
    {
        List<ISocialAnimalArticle> Search(ISocialAnimalRequest request);
    }
}
