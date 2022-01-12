using System;
using System.Runtime.Serialization;

namespace EDNeutronRouterPlugin
{
    [Serializable]
    internal class InvalidEndSystemException : Exception
    {
        public InvalidEndSystemException()
        {
        }

        public InvalidEndSystemException(string? message) : base(message)
        {
        }

        public InvalidEndSystemException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidEndSystemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}