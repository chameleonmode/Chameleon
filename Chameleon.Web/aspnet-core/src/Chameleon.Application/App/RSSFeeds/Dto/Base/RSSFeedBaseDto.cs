namespace Chameleon.App
{
    public class RSSFeedBaseDto
    {
        public string Url { get; set; }

        [Identity]
        public int ProfileId { get; set; }
    }
}
