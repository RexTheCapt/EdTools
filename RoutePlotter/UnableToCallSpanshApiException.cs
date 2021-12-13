using System;
using System.Runtime.Serialization;

namespace EDNeutronRouterPlugin
{
    [Serializable]
    internal class UnableToCallSpanshApiException : Exception
    {
        public UnableToCallSpanshApiException()
        {
        }

        public UnableToCallSpanshApiException(string? message) : base(message)
        {
        }

        public UnableToCallSpanshApiException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToCallSpanshApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}