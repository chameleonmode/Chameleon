using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.Profiles
{
    public class UserProfileApi
        : ApiLayer<UserProfileDto, int, CreateUserProfileDto, UserProfileDto>
        , IUserProfileApi
    {
        public UserProfileApi(
            IApiClient apiClient
            ) : base(apiClient, "profile")
        {
        }

        public void Execute(string endpointName, object query = null)
        {
            _apiClient.Post(GetEndpointUrl(endpointName), query);
        }

        public UserProfileDto[] GetAllByUserId(int id)
        {
            return _apiClient.Get<UserProfileDto[]>($"services/app/profile/GetAllByUserId?Id={id}");
        }
    }
}
