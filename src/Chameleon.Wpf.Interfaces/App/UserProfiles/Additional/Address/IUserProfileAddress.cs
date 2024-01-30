namespace Chameleon.Interfaces.UserProfiles.Additional
{
    public interface IUserProfileAddress 
        : IUserProfileRequiredEntity
    {
        string Title { get; set; }
        string AddressLine1 { get; set; }
        string AddressLine2 { get; set; }
        string City { get; set; }
        string State { get; set; }
        string Zip { get; set; }
        int? CountryId { get; set; }
        string Notes { get; set; }
    }
}
