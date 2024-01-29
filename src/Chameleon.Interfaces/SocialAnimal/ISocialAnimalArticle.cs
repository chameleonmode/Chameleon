namespace Chameleon.Interfaces.SocialAnimal
{
    // TODO: correct types and names
    public interface ISocialAnimalArticle
    {
        string published_date { get; set; }
        string image { get; set; }
        string title { get; set; }
        string url { get; }
        int content_word_count { get; set; }
        string authors { get; set; }
        string domain { get; set; }
        int twitter_share_count { get; set; }
        int total_share_count { get; set; }
        int facebook_share_count { get; set; }
        int pinterest_share_count { get; set; }
        int reddit_stats_score { get; set; }
    }
}
