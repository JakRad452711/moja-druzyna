using System;

namespace moja_druzyna.Lib.Exceptions
{
    public class LayoffRoleMismatchException : Exception
    {
        public LayoffRoleMismatchException()
        {
        }

        public LayoffRoleMismatchException(string message)
        : base(message)
        {
        }
    }
}
