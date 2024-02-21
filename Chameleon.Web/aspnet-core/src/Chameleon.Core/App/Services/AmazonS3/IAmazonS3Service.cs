using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.App.Services.AmazonS3
{
    public interface IAmazonS3Service : ISingletonDependency
    {
        Task<T> GetObjectAsync<T>(string bucketName, string objectKey);
        Task UploadObjectAsync<T>(string bucketName, string objectKey, T objectValue);
    }
}
