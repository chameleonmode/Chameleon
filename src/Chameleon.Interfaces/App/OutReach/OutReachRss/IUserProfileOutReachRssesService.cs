using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.OutReach
{
    public interface IUserProfileOutReachRssesService 
        : ISingletonDependency
    {
        IUserProfileOutReachRsses GetAll(int profileId);
        IUserProfileOutReachRss AddOutReachRss(UserProfileOutReachRssBindable outReachRss);
        void SaveOutReachRss(UserProfileOutReachRssBindable outReachRss);
        void DeleteOutReachRss(UserProfileOutReachRssBindable outReachRss);
    }

    public class UserProfileOutReachRssBindable
        : BindableBase
        , IUserProfileOutReachRss
    {
        public string RssName { get; set; }
        public string RssLink { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Notes { get; set; }
        public OutReachRssStatus Status { get; set; }
        public int ProfileId { get; set; }
        public int Id { get; set; }
    }
    //public class UserProfileOutReachRssBindableMap : Profile
    //{
    //    public UserProfileOutReachRssBindableMap()
    //    {
    //        // TODO: Use strong type and mapping
    //        var map = CreateMap<UserProfileOutReachRssBindable, UserProfileOutReachRss>();
    //        map.ReverseMap();
    //    }
    //}
}
