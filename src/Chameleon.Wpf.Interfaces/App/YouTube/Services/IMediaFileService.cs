using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Interfaces.YouTube
{
    public interface IMediaFileService : ISingletonDependency
    {
        string VideoFormats { get; }
        string ImageFormats { get; }
        long MaxSizeThumbnail { get; }
        long MaxSizeVideo { get; }
        string GetMimeTypeImage(string path);
        long GetFileSize(string path);
        IEnumerable<string> GetFiles(string path, string extensions);
    }
}
