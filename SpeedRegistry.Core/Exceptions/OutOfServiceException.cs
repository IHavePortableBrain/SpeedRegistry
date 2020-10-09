using System;

namespace SpeedRegistry.Core.Exceptions
{
    public class OutOfServiceException : Exception
    {
        public OutOfServiceException(string message)
            : base(message)
        {
        }
    }
}
