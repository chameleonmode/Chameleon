using NodaTime;
using NodaTime.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace Chameleon.ThirdParty.GeoIp;
public class GeoIpApi
{
    //make singleton
    private static GeoIpApi _instance;
    public static GeoIpApi Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GeoIpApi();
            }
            return _instance;
        }
    }

    public async Task<Models.Geoiplookup> GetGeoIp(string proxyUrl, string proxyUsername = null, string proxyPassword = null)
    {
        var handler = new HttpClientHandler
        {
            Proxy = new WebProxy(proxyUrl)
        };

        if (!string.IsNullOrEmpty(proxyUsername) && !string.IsNullOrEmpty(proxyPassword))
        {
            handler.Proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
        }

        using HttpClient client = new(handler)
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        HttpResponseMessage response = await client.GetAsync("https://geoip-lookup.vercel.app/api/geoip");

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            // Assuming you have a method to deserialize the response to Models.Geoiplookup
            return DeserializeGeoiplookup(responseBody);
        }
        else
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }
    }

    private Models.Geoiplookup DeserializeGeoiplookup(string responseBody)
    {
        // Implement the deserialization logic here
        // For example, using System.Text.Json:
        return JsonSerializer.Deserialize<Models.Geoiplookup>(responseBody);
    }
    public List<string> GetAbbrs(string timeZoneId)
    {
        var tzdb = DateTimeZoneProviders.Tzdb;
        var timeZone = tzdb[timeZoneId];

        var intervals = timeZone.GetZoneIntervals(Instant.FromUtc(2023, 1, 1, 0, 0), Instant.FromUtc(2026, 1, 1, 0, 0));
        var abbrs = intervals.Select(i => @$"""{i.Name}""").ToList();
        return abbrs;
    }

    public List<long> GetUntilInstants(string timeZoneId)
    {
        var tzdb = DateTimeZoneProviders.Tzdb;
        var timeZone = tzdb[timeZoneId];
        var intervals = timeZone.GetZoneIntervals(Instant.FromUtc(2023, 1, 1, 0, 0), Instant.FromUtc(2026, 1, 1, 0, 0));
        var untils = intervals.Select(i => i.End.ToDateTimeUtc().Ticks).ToList();
        return untils;
    }

    public List<string> GetOffsets(string timeZoneId)
    {
        var tzdb = DateTimeZoneProviders.Tzdb;
        var timeZone = tzdb[timeZoneId];

        var intervals = timeZone.GetZoneIntervals(Instant.FromUtc(2023, 1, 1, 0, 0), Instant.FromUtc(2026, 1, 1, 0, 0));
        var offsets = intervals.Select(i => @$"""{i.WallOffset.ToString().PadRight(5, '0')}""").ToList();

        return offsets;
        //foreach (var interval in intervals)
        //{
        //    // Calculate the total offset from UTC in minutes
        //    var totalOffset = interval.StandardOffset + interval.Savings;
        //    offsets.Add((int)totalOffset.ToTimeSpan().TotalMinutes);
        //}

        //return offsets;
    }

    //public List<string> GetAbbrs(string timeZoneId)
    //{
    //    var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    //    var abbrs = new List<string>();

    //    foreach (var adjustmentRule in timeZoneInfo.GetAdjustmentRules())
    //    {
    //        abbrs.Add(adjustmentRule.DaylightTransitionStart.IsFixedDateRule ? "\"DST\"" : "\"STD\"");
    //    }

    //    return abbrs;
    //}

    //public List<DateTimeOffset?> GetUntilInstants(string timeZoneId)
    //{
    //    var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    //    var untils = new List<DateTimeOffset?>();

    //    foreach (var adjustmentRule in timeZoneInfo.GetAdjustmentRules())
    //    {
    //        untils.Add(adjustmentRule.DateEnd);
    //    }

    //    return untils;
    //}

    //public List<TimeSpan> GetOffsets(string timeZoneId)
    //{
    //    var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    //    var offsets = new List<TimeSpan>();

    //    foreach (var adjustmentRule in timeZoneInfo.GetAdjustmentRules())
    //    {
    //        offsets.Add(adjustmentRule.DaylightDelta);
    //    }

    //    return offsets;
    //}
}
