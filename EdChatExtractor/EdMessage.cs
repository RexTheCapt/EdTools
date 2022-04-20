using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdChatExtractor
{
    internal class EdMessage
    {
        internal string cmdr;
        internal string channel;
        internal string message;

        internal EdMessage(string cmdr, string channel, string message)
        {
            this.cmdr = cmdr;
            this.channel = channel;
            this.message = message;
        }

        public override string ToString()
        {
            return $"[{channel}] {cmdr}: {message}";
        }
    }
}
