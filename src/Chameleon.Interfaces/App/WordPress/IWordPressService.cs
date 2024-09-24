using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.WordPress
{
    public interface IWordPressService : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string CreatePost(IPostCreateParameters parameters, IUserProfile userProfile);
    }
}
