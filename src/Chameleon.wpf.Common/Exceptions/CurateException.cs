using System;

namespace Chameleon.Common.Exceptions
{
    public class CurateException : Exception
    {
        public CurateException() { }

        public CurateException(string message) : base(message) { }

        public CurateException(string message, Exception ex) 
            : base(message, ex) 
        { }

        public CurateException(Exception ex) 
            : base(ex.Message)
        { }
    }
}
 