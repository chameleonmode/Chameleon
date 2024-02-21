namespace Chameleon.App
{
    public class BookmarkFileBaseDto
    {
        public string Url { set; get; }
        public string Name { set; get; }

        [Identity]
        public int BookmarkId { set; get; }
    }
}
