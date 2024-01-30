namespace Chameleon.Interfaces.Auth
{
    public interface IAuthUser
    {
        long UserId { get; set; }
        long? CreatorUserId { get; set; }
        string UserName { get; set; }
        bool TookGuidedTour { get; set; }
    }
}
