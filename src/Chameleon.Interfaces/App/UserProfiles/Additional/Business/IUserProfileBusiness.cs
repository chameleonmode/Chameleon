namespace Chameleon.Interfaces.UserProfiles.Additional
{
    public interface IUserProfileBusiness 
        : IUserProfileRequiredEntity
    {
        string Title { get; set; }
        string CompanyName { get; set; }
        string Department { get; set; }
        string PhoneNumber { get; set; }
        string WebSite { get; set; }
        string Notes { get; set; }
    }
}
