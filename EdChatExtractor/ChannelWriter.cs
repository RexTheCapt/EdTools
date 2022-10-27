using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdChatExtractor
{
    internal class ChannelWriter
    {
        public ChannelWriter(string channel, string exportDirectory)
        {
            Channel = channel;
            _writer = new($"{exportDirectory}{channel}.log");
        }

        public readonly string Channel;
        private readonly StreamWriter _writer;

        internal void Write(string message)
        {
            _writer.WriteLine(message);
        }

        internal void Dispose()
        {
            _writer.Flush();
            _writer.Close();
            _writer.Dispose();
        }
    }
}
