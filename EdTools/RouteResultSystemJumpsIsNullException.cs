using System;
using System.Runtime.Serialization;

namespace EDNeutronRouterPlugin
{
    [Serializable]
    internal class RouteResultSystemJumpsIsNullException : Exception
    {
        public RouteResultSystemJumpsIsNullException()
        {
        }

        public RouteResultSystemJumpsIsNullException(string? message) : base(message)
        {
        }

        public RouteResultSystemJumpsIsNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected RouteResultSystemJumpsIsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}