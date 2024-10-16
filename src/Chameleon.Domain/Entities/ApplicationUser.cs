using Chameleon.Interfaces.Auth;
using System.Linq;

namespace Chameleon.Domain.Entities
{
    public class ApplicationUser : IApplicationUser
    {
        private readonly IAuthSession _authSession;

        public ApplicationUser(
            IAuthSession authSession
            )
        {
            _authSession = authSession;
        }

        public bool IsAuthenticated => _authSession.HasAuthToken;
        public string Email => _authSession.UserName;
        public bool IsAssistant => _authSession.CreatorUserId.HasValue;
        public bool TookGuidedTour => _authSession.TookGuidedTour;
		public long id => _authSession.UserId;

		public bool HasPemission(string permissionName)
        {
            return _authSession.Permissions.Contains(permissionName);
        }
    }
}
