using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdChatExtractor
{
    internal class EdMessage
    {
        internal DateTime timestamp;
        internal string cmdr;
        internal Direction direction;
        internal string channel;
        internal string message;
        internal string directionString
        {
            get
            {
                if (direction.Equals(Direction.Sending))
                    return ">>>";
                else if (direction.Equals(Direction.Receiving))
                    return "<<<";
                else if (direction.Equals(Direction.Unknown))
                    return "---";
                else throw new Exception("Unknown direction");
            }
        }

        internal EdMessage(DateTime timestamp, string cmdr, Direction direction, string channel, string message)
        {
            this.timestamp = timestamp;
            this.cmdr = cmdr;
            this.direction = direction;
            this.channel = channel;
            this.message = message;
        }

        public override string ToString()
        {
            string dir = "";

            if (direction.Equals(Direction.Receiving))
                dir = "<<<";
            else if (direction.Equals(Direction.Sending))
                dir = ">>>";
            else
                throw new Exception("Unknown direction");

            return $"[{timestamp} | {channel}] {cmdr} {dir} {message}";
        }

        public enum Direction
        {
            Unknown,
            Sending,
            Receiving
        }
    }
}
