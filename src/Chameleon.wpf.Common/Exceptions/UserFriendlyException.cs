using System;

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
