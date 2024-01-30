using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Prospector
{
    public interface IUserProfileProspectorBlogsOfInterestsService
        : ISingletonDependency
    {
        IUserProfileProspectorBlogsOfInterests GetAll(int profileId);
        IUserProfileProspectorBlogsOfInterest AddProspectorBlogsOfInterest(IUserProfileProspectorBlogsOfInterest blogsOfInterest);
        void SaveProspectorBlogsOfInterest(IUserProfileProspectorBlogsOfInterest blogsOfInterest);
        void DeleteProspectorBlogsOfInterest(IUserProfileProspectorBlogsOfInterest blogsOfInterest);
    }    
}
