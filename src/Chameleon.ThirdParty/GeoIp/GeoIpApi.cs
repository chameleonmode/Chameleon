using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
using NodaTime.TimeZones;
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

    public async Task<string> GetIPApi(string proxyUrl, string proxyUsername = null, string proxyPassword = null)
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
            Timeout = TimeSpan.FromSeconds(5)
        };
        HttpResponseMessage response = await client.GetAsync("http://ip-api.com/json");

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            //return JsonSerializer.Deserialize<Models.Ipapi>(responseBody);
            return responseBody;
        }
        else
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
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
            Timeout = TimeSpan.FromSeconds(5)
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

    public string GetFormatted(string tz)
    {
        // Get the time zone
        DateTimeZone dateTimeZone = DateTimeZoneProviders.Tzdb[tz];

        // Create lists to hold the abbreviations, untils, and offsets
        var abbrs = new List<string>();
        var untils = new List<string>();
        var offsets = new List<string>();

        // Get the current time and iterate over the next 10 years
        Instant now = Instant.FromUtc(2023, 1, 1, 0, 0); //SystemClock.Instance.GetCurrentInstant();
        Instant end = Instant.FromUtc(2026, 1, 1, 0, 0); // now.Plus(Duration.FromDays(365 * 10));

        // Iterate over the intervals in the time zone
        foreach (var interval in dateTimeZone.GetZoneIntervals(now, end))
        {
            abbrs.Add(interval.Name);
            untils.Add(interval.End.ToUnixTimeSeconds().ToString());
            offsets.Add(interval.WallOffset.Seconds.ToString());
        }

        // Create an object to hold the time zone information
        var timeZoneInfo = new
        {
            name = tz,
            abbrs = abbrs,
            untils = untils,
            offsets = offsets
        };

        // Serialize the time zone information to JSON
        string json = JsonSerializer.Serialize(timeZoneInfo, new JsonSerializerOptions { WriteIndented = true });
        return json;

        // Create a LocalDateTime
        LocalDateTime localDateTime = new LocalDateTime(2023, 10, 5, 14, 30);

        // Get the time zone
        DateTimeZone timeZone = DateTimeZoneProviders.Tzdb[tz];

        // Create a ZonedDateTime
        ZonedDateTime zonedDateTime = localDateTime.InZoneStrictly(timeZone);

        // Format the ZonedDateTime
        string formattedDateTime = ZonedDateTimePattern.ExtendedFormatOnlyIso.Format(zonedDateTime);
        return formattedDateTime;
    }
    public IEnumerable<string> GetAbbrs(string timeZoneId)
    {
        //var intervals = 
        //    DateTimeZoneProviders.Tzdb[timeZoneId]
        //    .GetZoneIntervals(Instant.FromUtc(2023, 1, 1, 0, 0), Instant.FromUtc(2026, 1, 1, 0, 0))
        //    .Union(DateTimeZoneProviders.Bcl[timeZoneId]
        //    .GetZoneIntervals(Instant.FromUtc(2023, 1, 1, 0, 0), Instant.FromUtc(2026, 1, 1, 0, 0)));

        //var abbrs = intervals.Select(i => @$"""{i.Name}""").ToList();
        var r =  GetZones(timeZoneId).Select(i => @$"""{i.Name}""");
        return r;
    }

    public IEnumerable<long> GetUntilInstants(string timeZoneId)
    {
        //var tzdb = DateTimeZoneProviders.Tzdb;
        //var timeZone = tzdb[timeZoneId];
        //var intervals = timeZone.GetZoneIntervals(Instant.FromUtc(2023, 1, 1, 0, 0), Instant.FromUtc(2026, 1, 1, 0, 0));
        //var untils = intervals.Select(i => i.End.ToDateTimeUtc().Ticks).ToList();
        //return untils;

        var r = GetZones(timeZoneId).Select(i => i.End.ToUnixTimeSeconds());
        return r;
    }

    public IEnumerable<string> GetOffsets(string timeZoneId)
    {
        //var tzdb = DateTimeZoneProviders.Tzdb;
        //var timeZone = tzdb[timeZoneId];

        //var intervals = timeZone.GetZoneIntervals(Instant.FromUtc(2023, 1, 1, 0, 0), Instant.FromUtc(2026, 1, 1, 0, 0));
        //var offsets = intervals.Select(i => @$"""{i.WallOffset.ToString().PadRight(5, '0')}""").ToList();

        var r = GetZones(timeZoneId).Select(i => @$"""{i.WallOffset.ToString().PadRight(5, '0')}""").ToList();
        return r;
        //foreach (var interval in intervals)
        //{
        //    // Calculate the total offset from UTC in minutes
        //    var totalOffset = interval.StandardOffset + interval.Savings;
        //    offsets.Add((int)totalOffset.ToTimeSpan().TotalMinutes);
        //}

        //return offsets;
    }

    IEnumerable<ZoneInterval> GetZones(string timeZoneId) =>
        DateTimeZoneProviders.Tzdb[timeZoneId]
        .GetZoneIntervals(Instant.FromUtc(2023, 1, 1, 0, 0), Instant.FromUtc(2026, 1, 1, 0, 0))
        .Union(DateTimeZoneProviders.Bcl[timeZoneId]
        .GetZoneIntervals(Instant.FromUtc(2023, 1, 1, 0, 0), Instant.FromUtc(2026, 1, 1, 0, 0)));
    

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
