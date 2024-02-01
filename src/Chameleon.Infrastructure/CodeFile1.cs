
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;



namespace Prism
{ }
namespace Prism.Ioc
{

}
namespace Prism.Events
{ }

namespace Unity
{
    public interface IUnityContainer
    {
        void RegisterInstance<T>(T instance);
    }
}

namespace Chameleon.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static void InvokeOnUiThread(this object self, Action callback)
        {
            //Application.Current.Dispatcher.Invoke(callback);
        }

        public static T InvokeOnUiThread<T>(this object self, Func<T> action)
        {
            throw new NotImplementedException();
           // return Application.Current.Dispatcher.Invoke(action);
        }

        public static void InvokeOnUiThread(this object self, EventHandler handler, EventArgs args = null)
        {
            self.InvokeOnUiThread(() =>
            {
                handler?.Invoke(self, args ?? new EventArgs());
            });
        }

        public static Task InvokeOnUiThreadAsync<T>(this object self, Func<T> action, Action<T> handler = null, Action @finally = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!TryExecute(action, out var result))
                    {
                        return;
                    }

                    if (handler != null)
                    {
                        self.InvokeOnUiThread(() =>
                        {
                            handler(result);
                        });
                    }
                }
                finally
                {
                    @finally?.Invoke();
                }
            });
        }

        public static Task InvokeOnUiThreadAsync(this object self, Action action, Action<bool> handler = null, Action @finally = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    var success = TryExecute(action);

                    if (handler != null)
                    {
                        self.InvokeOnUiThread(() =>
                        {
                            handler(success);
                        });
                    }
                }
                finally
                {
                    @finally?.Invoke();
                }
            });
        }

        private static Action Noop = () => { };
        public static Task InvokeOnUiThreadAsync(this object self, Action action)
        {
            return Task.Run(() =>
            {
                var success = TryExecute(action);
                self.InvokeOnUiThread(Noop);
            });
        }

        public static async void InvokeAsync(this object self, Func<Task> action)
        {
            await _InvokeAsync(action);
        }

        private static async Task _InvokeAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                //ExceptionHandler.ShowException(ex);
            }
        }

        private static bool TryExecute<T>(Func<T> action, out T result)
        {
            try
            {
                result = action();
                return true;
            }
            catch (Exception ex)
            {
                //ExceptionHandler.ShowException(ex);
            }
            result = default(T);
            return false;
        }

        private static bool TryExecute(Action action)
        {
            return TryExecute(() =>
            {
                action();
                return true;
            }, out var result);
        }

        public static void TryCatchIgnore(this object self, Action action)
        {
            try
            {
                action();
            }
            catch
            {
                //ignore
            }
        }

        public static T1 CopyFrom<T1, T2>(this T1 destObject, T2 srcObject)
            where T1 : class
            where T2 : class
        {
            PropertyInfo[] srcFields = srcObject
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            PropertyInfo[] destFields = destObject
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            foreach (var property in srcFields)
            {
                var dest = destFields
                    .FirstOrDefault(x => x.Name == property.Name);

                if (dest != null && dest.CanWrite)
                {
                    dest.SetValue(destObject, property.GetValue(srcObject, null), null);
                }
            }

            return destObject;
        }
    }

    public static class IListExtensions
    {
        public static void AddRange<TSource>(this IList<TSource> self, IEnumerable<TSource> source)
        {
            foreach (var item in source)
            {
                self.Add(item);
            }
        }

        public static void RemoveRange<TSource>(this IList<TSource> self, IEnumerable<TSource> source)
        {
            foreach (var item in source)
            {
                self.Remove(item);
            }
        }

        public static void ReAddRange<TSource>(this IList<TSource> self, IEnumerable<TSource> source)
        {
            self.Clear();
            self.AddRange(source);
        }

        public static void Remove<TSource>(this IList<TSource> self, Func<TSource, bool> predicate)
        {
            for (var i = 0; i < self.Count; ++i)
            {
                if (predicate(self[i]))
                {
                    self.RemoveAt(i--);
                }
            }
        }

        public static void AddIfMissing<TSource>(this IList<TSource> self, TSource entity)
        {
            if (self.Contains(entity))
            {
                return;
            }
            self.Add(entity);
        }

        public static void AddNew<T>(this IList<T> self, int count)
            where T : class, new()
        {
            for (var i = 0; i < count; ++i)
            {
                self.Add(new T());
            }
        }

        public static List<T> CreateNew<T>(int count)
            where T : class, new()
        {
            var list = new List<T>(count);
            list.AddNew(count);
            return list;
        }
    }

    public static class StringExtensions
    {
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
}
namespace Chameleon.Common.Exceptions
{
    [Serializable]
    public class UserFriendlyException : Exception
    {
        public string Title { get; }

        public UserFriendlyException() { }

        public UserFriendlyException(string message)
            : base(message) { }

        public UserFriendlyException(string message, Exception inner)
            : base(message, inner) { }

        public UserFriendlyException(string message, string title)
            : this(message)
        {
            Title = title;
        }
    }
}