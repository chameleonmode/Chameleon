namespace Chameleon.Interfaces.WordPress
{
    public interface IWordPressSettings
    {
        string BaseUrl { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }
}
