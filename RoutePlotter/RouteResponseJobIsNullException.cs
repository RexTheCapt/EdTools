using System;
using System.Runtime.Serialization;

namespace EDNeutronRouterPlugin
{
    [Serializable]
    internal class RouteResponseJobIsNullException : Exception
    {
        public RouteResponseJobIsNullException()
        {
        }

        public RouteResponseJobIsNullException(string? message) : base(message)
        {
        }

        public RouteResponseJobIsNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected RouteResponseJobIsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}