using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EdTools;

namespace EdChatExtractor
{
    internal class Program
    {
        private static string? currentCommander = null;
        private static string exportDirectory = $"{DateTime.Now.Year}\\{DateTime.Now.Month}\\{DateTime.Now.Day}\\{DateTime.Now.Hour}\\{DateTime.Now.Minute}\\{DateTime.Now.Second}\\";

        internal static void Main(string[] args)
        {
            JournalScanner journalScanner = new JournalScanner();

            JournalScanner.ReceiveTextHandler += JournalScanner_ReceiveTextHandler;
            JournalScanner.SendTextHandler += JournalScanner_SendTextHandler;
            JournalScanner.LoadGameHandler += JournalScanner_LoadGameHandler;

            if (!Directory.Exists(exportDirectory))
                Directory.CreateDirectory(exportDirectory);

            journalScanner.ScanAll();

            foreach (ChannelWriter cw in channelWriters)
            {
                cw.Dispose();
            }

            Console.WriteLine($"Extracted to \"{Path.GetFullPath(exportDirectory)}\"");
        }

        private static void JournalScanner_SendTextHandler(object? sender, EventArgs e)
        {
            JournalScanner.SendTextEventArgs e1 = (JournalScanner.SendTextEventArgs)e;

            if (e1.Timestamp == null || e1.To == null || e1.Message == null)
                return;

            if (currentCommander == null)
                currentCommander = "---";

            EdMessage m = new((DateTime)e1.Timestamp, currentCommander, EdMessage.Direction.Sending, e1.To, e1.Message);

            ExtractMessage(m);

            Console.WriteLine(m);
        }

        private static void JournalScanner_ReceiveTextHandler(object? sender, EventArgs e)
        {
            JournalScanner.ReceiveTextEventArgs e1 = (JournalScanner.ReceiveTextEventArgs)e;
            
            if (e1.Timestamp == null || e1.From == null || e1.Message == null || e1.Channel == null || e1.Channel.Equals("NPC", StringComparison.OrdinalIgnoreCase))
                return;

            if (currentCommander == null)
                currentCommander = "---";

            string channel = e1.Channel;
            if (e1.Channel.Equals("player"))
                channel = e1.From;

            EdMessage m = new((DateTime)e1.Timestamp, e1.From, EdMessage.Direction.Receiving, channel, e1.Message);
            
            ExtractMessage(m);

            Console.WriteLine(m);
        }

        private static List<ChannelWriter> channelWriters = new();
        private static void ExtractMessage(EdMessage message)
        {
            ChannelWriter? found = channelWriters.Find(x => x.Channel.Equals(message.channel));

            if (found == null)
            {
                ChannelWriter cw = new(message.channel, exportDirectory);
                channelWriters.Add(cw);
                found = cw;
            }

            found.Write($"[{message.timestamp}] {message.cmdr} \t {message.directionString} \t {message.message}");
        }

        private static void JournalScanner_LoadGameHandler(object? sender, EventArgs e)
        {
            JournalScanner.LoadGameEventArgs lge = (JournalScanner.LoadGameEventArgs)e;
            currentCommander = lge.LoadGame.Value<string>("Commander");
        }
    }
}
