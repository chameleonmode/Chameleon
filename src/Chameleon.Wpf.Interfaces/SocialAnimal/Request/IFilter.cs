using System.ComponentModel;

namespace Chameleon.Interfaces.SocialAnimal
{
    // todo: add link to doc api
    public interface IFilter
    {
        string Domain { get; set; }
        string Authors { get; set; }

        // TODO:use enum?
        string Article_types { get; set; }

        // 
        string DeepSearch { get; set; }

        void AddArticleType(ArticleTypes type);
    }

    public enum ArticleTypes
    {
        [Description("listicle")]
        Listicle,
        [Description("infographic")]
        Infographic,
        [Description("how_to_guide")]
        HowToGuide,
        [Description("case_study")]
        CaseStudy,
        [Description("guest_post")]
        GuestPost,
        [Description("review")]
        Review,
        [Description("video")]
        Video,
        [Description("podcast")]
        Podcast,
        [Description("webinar")]
        Webinar,
        [Description("interview")]
        Interview,
        [Description("quote")]
        Quote,
        [Description("meme")]
        Meme,
        [Description("giveaway")]
        Giveaway,
        [Description("quiz")]
        Quiz,
    }
}
