namespace Chameleon.App
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public bool IsValid => !string.IsNullOrWhiteSpace(AccessToken);
    }
}
