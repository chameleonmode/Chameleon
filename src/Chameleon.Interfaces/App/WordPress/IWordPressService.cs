using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.WordPress
{
    public interface IWordPressService : ITransientDependency
    {
        string CreatePost(IPostCreateParameters parameters, IUserProfile userProfile);
    }
}
