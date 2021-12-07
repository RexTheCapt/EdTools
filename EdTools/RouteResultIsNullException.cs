using System;
using System.Runtime.Serialization;

namespace EDNeutronRouterPlugin
{
    [Serializable]
    internal class RouteResultIsNullException : Exception
    {
        public RouteResultIsNullException()
        {
        }

        public RouteResultIsNullException(string? message) : base(message)
        {
        }

        public RouteResultIsNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected RouteResultIsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}