using System;

namespace Chameleon.Interfaces.WordPress
{
    public interface IPostCreateParameters
    {
        string Title { get; set; }
        string Content { get; set; }
        DateTime Date { get; set; }
        string Excerpt { get; set; }
        string FeaturedMedia { get; set; }
        string Categories { get; set; }
        string Tags { get; set; }
    }
}
