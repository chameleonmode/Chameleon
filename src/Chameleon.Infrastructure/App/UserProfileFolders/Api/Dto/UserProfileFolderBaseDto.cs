namespace Chameleon.Infrastructure.UserProfileFolders
{
    public class UserProfileFolderBaseDto
    {
        public string Title { get; set; }
        public string IsFavorite { get; set; }
        public int ProfilesCount { get; set; }
        public long? CreatorUserId { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
