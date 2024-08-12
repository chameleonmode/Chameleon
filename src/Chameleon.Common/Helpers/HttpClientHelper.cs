using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Common.Helpers;
public class HttpClientHelper
{
    public static async Task<string> GetAsync(string url)
    {
        using HttpClient client = new();
        //if (!string.IsNullOrEmpty(bearerToken))
        //{
        //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", bearerToken);
        //}
        using HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> PostAsync(string url, HttpContent content, string bearerToken = null)
    {
        using HttpClient client = new();
        //if (!string.IsNullOrEmpty(bearerToken))
        //{
        //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", bearerToken);
        //}
        using HttpResponseMessage response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<HttpResponseMessage> PostAsync(string url, AuthenticationHeaderValue authorization = null, IEnumerable<KeyValuePair<string, string>> headers = null, MultipartFormDataContent content = null)
    {
        using HttpClient client = new();
        if(authorization != null)
            client.DefaultRequestHeaders.Authorization = authorization;
        //if (!string.IsNullOrEmpty(bearerToken))
        //{
        //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", bearerToken);
        //}
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        if (headers != null)
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);
        return await client.SendAsync(request);
        //response.EnsureSuccessStatusCode();
        //return await response.Content.ReadAsStringAsync();
    }

    public async static Task<HttpResponseMessage> PutAsync(string url, IEnumerable<KeyValuePair<string, string>> headers = null)
    {
        using HttpClient client = new();
        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        if(headers != null)
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);
      

        return await client.SendAsync(request);
    }
}
