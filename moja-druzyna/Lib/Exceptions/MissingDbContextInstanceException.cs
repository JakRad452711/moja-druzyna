using System;

namespace moja_druzyna.Lib.Exceptions
{
    public class MissingDbContextInstanceException : Exception
    {
        public MissingDbContextInstanceException()
        {
        }

        public MissingDbContextInstanceException(string message)
        : base(message)
        {
        }
    }
}
