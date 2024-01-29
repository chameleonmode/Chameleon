using System;

namespace Chameleon.Interfaces.YouTube
{
    public interface IVideoUploadStatus
    {
        Guid Id { get; set; }
        bool IsSuccesfulCompleted { get; set; }
        long BytesSent { get; set; }
        long BytesAll { get; set; }
        int ProfileId { get; set; }
    }
}
