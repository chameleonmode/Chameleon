namespace Chameleon.Interfaces.Repository
{
    public class UserProfileGetAllRequestDto 
        : GetAllRequestDto
    {
        public int? ProfileId { get; }

        public UserProfileGetAllRequestDto()
        {
        }

        public UserProfileGetAllRequestDto(int? profileId)
        {
            ProfileId = profileId;
        }
    }
}
