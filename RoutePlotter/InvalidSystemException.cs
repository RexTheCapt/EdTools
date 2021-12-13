using System;
using System.Runtime.Serialization;

namespace EDNeutronRouterPlugin
{
    [Serializable]
    internal class InvalidSystemException : Exception
    {
        public InvalidSystemException()
        {
        }

        public InvalidSystemException(string? message) : base(message)
        {
        }

        public InvalidSystemException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidSystemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}