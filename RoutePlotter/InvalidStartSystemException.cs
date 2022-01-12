using System;
using System.Runtime.Serialization;

namespace EDNeutronRouterPlugin
{
    [Serializable]
    internal class InvalidStartSystemException : Exception
    {
        public InvalidStartSystemException()
        {
        }

        public InvalidStartSystemException(string? message) : base(message)
        {
        }

        public InvalidStartSystemException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidStartSystemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}