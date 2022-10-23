using System;

namespace moja_druzyna.Lib.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException()
        {
        }

        public RecordNotFoundException(string message)
        : base(message)
        {
        }
    }
}
