using Abp.Authorization;
using Amazon.S3;
using Chameleon.App.Services.AmazonS3;
using Chameleon.App.Services.AmazonS3.Dto;
using Chameleon.Configuration;
using Chameleon.Models.TokenAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Controllers
{
    [AbpAuthorize]
    [Route("api/[controller]/[action]")]
    public class CookiesController : ChameleonControllerBase
    {
        private string BucketName => appConfig["Amazon:S3:Buckets:Cookies"];

        private readonly IWebHostEnvironment env;
        private readonly IConfigurationRoot appConfig;
        private readonly IAmazonS3Service amazonS3Service;

        public CookiesController(IWebHostEnvironment env, IAmazonS3Service amazonS3Service)
        {
            this.env = env;
            appConfig = env.GetAppConfiguration();
            this.amazonS3Service = amazonS3Service;
        }

        [HttpGet]
        public async Task<ActionResult> Import(string profileId)
        {
            CookieDto[] cookies = null;
            try
            {
                cookies = await amazonS3Service.GetObjectAsync<CookieDto[]>(BucketName, GetCookieKey(profileId));
            }
            catch (AmazonS3Exception ex) when (ex.ErrorCode == "NoSuchKey")
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            return new ObjectResult(cookies);
        }
        [HttpPost]
        public async Task<ActionResult> Export(string profileId, [FromBody] CookieDto[] cookies)
        {
            try
            {
                await amazonS3Service.UploadObjectAsync(
                    BucketName,
                    GetCookieKey(profileId),
                    cookies);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        private string GetCookieKey(string profileId) 
        {
            return $"Cookies_{(env.IsDevelopment() ? "Dev" : "Prod")}_{profileId}";
        }
    }
}
