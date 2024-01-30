namespace Chameleon.Interfaces.Auth
{
    public static class CurrentApplicationUser
    {
        private static IApplicationUser _applicationUser;
        public static void SetCurrentUser(this object _, IApplicationUser user)
        {
            _applicationUser = user;
        }

        public static IApplicationUser GetCurrentUser(this object _)
        {
            return _applicationUser;
        }
    }
}
