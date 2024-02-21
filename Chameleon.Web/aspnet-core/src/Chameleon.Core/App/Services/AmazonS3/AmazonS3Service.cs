using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Amazon.Runtime;
using System.Net.Http;
using Abp.Json;
using Newtonsoft.Json;
using Amazon.S3.Transfer;
using Amazon;
using Microsoft.AspNetCore.Hosting;
using Chameleon.Configuration;
using Microsoft.Extensions.Configuration;

namespace Chameleon.App.Services.AmazonS3
{
    public class AmazonS3Service : IAmazonS3Service
    {
        private string AccessKey => appConfig["Amazon:S3:AccessKey"];
        private string SecretKey => appConfig["Amazon:S3:SecretKey"];

        private readonly IConfigurationRoot appConfig;
        private readonly IAmazonS3 client;

        public AmazonS3Service(IWebHostEnvironment env)
        {
            appConfig = env.GetAppConfiguration();
            client = new AmazonS3Client(
              AccessKey,
              SecretKey,
              RegionEndpoint.EUWest2);
        }

        public async Task<T> GetObjectAsync<T>(string bucketName, string objectKey)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            using var response = await client.GetObjectAsync(request);

            if (response.HttpStatusCode != HttpStatusCode.OK)
                throw new Exception("Http response to get object from Amazon S3 wasn't successful.");

            var reader = new StreamReader(response.ResponseStream);
            string json = await reader.ReadToEndAsync();
            T des = JsonConvert.DeserializeObject<T>(json);

            return des;
        }
        public async Task UploadObjectAsync<T>(string bucketName, string objectKey, T objectValue)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                ContentBody = JsonConvert.SerializeObject(objectValue)
            };

            var response = await client.PutObjectAsync(request);
            if (response.HttpStatusCode != HttpStatusCode.OK)
                throw new Exception("Http response to upload object to Amazon S3 wasn't successful.");
        }
    }
}
