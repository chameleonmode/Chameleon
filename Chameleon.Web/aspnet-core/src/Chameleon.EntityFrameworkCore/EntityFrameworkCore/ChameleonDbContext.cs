using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Chameleon.Authorization.Roles;
using Chameleon.Authorization.Users;
using Chameleon.MultiTenancy;
using Chameleon.App.Entities;
using Chameleon.MultiTenancy.Payments;
using Chameleon.App.Entities.Assistant;
using Chameleon.App.Entities.ShareFolders;
using Chameleon.App.Entities.Permissions;

namespace Chameleon.EntityFrameworkCore
{
    public class ChameleonDbContext : AbpZeroDbContext<Tenant, Role, User, ChameleonDbContext>
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Proxy> Proxies { get; set; }
        public DbSet<RSSFeed> RSSFeeds { get; set; }
        public DbSet<UserDefaultSettings> UserDefaultSettings { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<WebBrowserSetting> WebBrowserSettings { get; set; }
        public DbSet<WebBrowserUserAgent> WebBrowserUserAgents { get; set; }
        public DbSet<ProxyCredit> ProxyCredits { get; set; }
        public DbSet<ProxyCreditOrder> ProxyCreditOrders { get; set; }
        public DbSet<ProxyCreditPlan> ProxyCreditPlans { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<OutReachRss> OutReachRsses { get; set; }
        public DbSet<OutReachLink> OutReachLinks { get; set; }
        public DbSet<OutReachTemplate> OutReachTemplates { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<BookmarkFile> BookmarkFiles { get; set; }
        public DbSet<ProspectorBlogsOfInterest> ProspectorBlogsOfInterests { get; set; }
        public DbSet<AppLogger> AppLoggers { get; set; }
        public DbSet<CookiesExcludedDomain> CookiesExcludedDomains { get; set; }
        public DbSet<AssistantLicense> AssistantLicense { get; set; }
        public DbSet<ProfileAssistant> ProfileAssistant { get; set; }
        public DbSet<ProfileAssistantPermission> ProfileAssistantPermissions { get; set; }
        public DbSet<ProfilePermission> ProfilePermissions { get; set; }
        public DbSet<UserFolder> UsersFolders { get; set; }
        public DbSet<UserFolderPermission> UserFoldersPermissions { get; set; }

        public ChameleonDbContext(DbContextOptions<ChameleonDbContext> options)
            : base(options)
        {
        }
    }
}
