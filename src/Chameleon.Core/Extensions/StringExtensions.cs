namespace Chameleon.Core.Extensions;

public static class StringExtensions
{
    public static bool HasAny(this string self)
    {
        return self != string.Empty && !string.IsNullOrEmpty(self) && !string.IsNullOrWhiteSpace(self);
    }
    public static string? Get(this string self)
    {
        return self != string.Empty && !string.IsNullOrEmpty(self) && !string.IsNullOrWhiteSpace(self) ? self : null;
    }
    public static string StripPrefix(this string self, string prefix)
    {
        return self.StartsWith(prefix) ? self.Substring(prefix.Length) : self;
    }

    public static string StripQuotes(this string self)
    {
        if (self.EndsWith("\"") && self.StartsWith("\""))
        {
            return self.Substring(1, self.Length - 2);
        }
        return self;
    }

    public static string EnsureDirectoryExists(this string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        return directoryPath;
    }

    public static bool DeleteDirectory(this string directoryPath, bool recursive = true)
    {
        try
        {
            Directory.Delete(directoryPath, true);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool RecreateDirectory(this string directoryPath)
    {
        if (directoryPath.DeleteDirectory())
        {
            directoryPath.EnsureDirectoryExists();
            return true;
        }
        return false;
    }

    public static string RemoveAfter(this string self, string substr)
    {
        var index = self.LastIndexOf(substr);
        if (index == -1)
        {
            return self;
        }
        return self.Remove(index);
    }

    public static string RemoveBefore(this string self, string substr)
    {
        var index = self.IndexOf(substr);
        if (index == -1)
        {
            return self;
        }
        return self.Remove(0, index + 1);
    }

    public static string KiloFormat(this int num)
    {
        if (num >= 100000000)
            return (num / 1000000).ToString("#,0M");

        if (num >= 10000000)
            return (num / 1000000).ToString("0.#") + "M";

        if (num >= 100000)
            return (num / 1000).ToString("#,0K");

        if (num >= 10000)
            return (num / 1000).ToString("0.#") + "K";

        return num.ToString("#,0");
    }

    public static string ToCommaSeparatedString<T>(this IEnumerable<T> self)
    {
        return string.Join(",", self);
    }

    public static string ToCommaSeparatedString<T>(this List<T> self)
    {
        return string.Join(",", self);
    }

    public static string[] AddQuotesToEachElement(this IList<string> self)
    {
        return self.Select(x => $"\"{x}\"").ToArray();
    }

    public static string CheckFeedForId(this string feedUrl)
    {
        if (!feedUrl.Contains("id="))
        {
            return feedUrl.ToLowerInvariant();
        }

        var startIndex = feedUrl.IndexOf("id=");

        var firstPartFeedUrl = feedUrl.Substring(0, startIndex).ToLowerInvariant();
        var secondPart = feedUrl.Substring(startIndex);

        return firstPartFeedUrl + secondPart;
    }

    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        return source?.IndexOf(toCheck, comp) >= 0;
    }
}
