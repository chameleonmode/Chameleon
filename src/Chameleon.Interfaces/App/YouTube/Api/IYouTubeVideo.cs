using Prism.Events;
using System.Collections.Generic;
using System.Threading;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeVideo
    {
        string Title { get; set; }
        string Description { get; set; }
        IList<string> Tags { get; set; }
        CancellationTokenSource CancellationToken { get; set; }
    }
}
