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
        private static string cmdr = "UNKNOWN";
        private static StreamWriter wingWriter = new StreamWriter("wing.txt");
        private static StreamWriter localWriter = new StreamWriter("local.txt");
        private static StreamWriter squadronWriter = new StreamWriter("squadron.txt");

        internal static void Main(string[] args)
        {
            JournalScanner journalScanner = new JournalScanner();

            JournalScanner.ReceiveTextHandler += HandleText;
            JournalScanner.SendTextHandler += HandleText;
            JournalScanner.LoadGameHandler += JournalScanner_LoadGameHandler;



            while (true)
            {
                journalScanner.Tick();
                Thread.Sleep(1000);

                bool brk = false;
                while(Console.KeyAvailable)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);

                    if (consoleKeyInfo.Key == ConsoleKey.Q)
                    {
                        wingWriter.Flush();
                        wingWriter.Dispose();

                        localWriter.Flush();
                        localWriter.Dispose();

                        squadronWriter.Flush();
                        squadronWriter.Dispose();

                        brk = true;
                        break;
                    }
                }

                if (brk)
                    break;
            }
        }

        private static void JournalScanner_LoadGameHandler(object? sender, EventArgs e)
        {
            JournalScanner.LoadGameEventArgs loadGameEventArgs = (JournalScanner.LoadGameEventArgs)e;
            Newtonsoft.Json.Linq.JObject loadGame = loadGameEventArgs.LoadGame;
            cmdr = loadGame.Value<string>("Commander");
        }

        private static void HandleText(object? sender, EventArgs e)
        {
            JournalScanner.ReceiveTextEventArgs? rArgs = e as JournalScanner.ReceiveTextEventArgs;
            JournalScanner.SendTextEventArgs? sArgs = e as JournalScanner.SendTextEventArgs;

            if (rArgs != null) Receive(rArgs);
            if (sArgs != null) Send(sArgs);
        }

        private static void Send(JournalScanner.SendTextEventArgs sArgs)
        {
            Newtonsoft.Json.Linq.JObject sendText = sArgs.SendText;
            string? channel = sendText.Value<string>("To");
            string? message = sendText.Value<string>("Message");

            WriteLine(cmdr, channel, message);
        }

        private static void WriteLine(string cmdr, string channel, string message)
        {
            Console.WriteLine($"[{channel.ToUpper()}] {cmdr}: {message}");
            switch (channel)
            {
                case "wing":
                    wingWriter.WriteLine($"{cmdr}: {message}");
                    break;
                case "squadron":
                    squadronWriter.WriteLine($"{cmdr}: {message}");
                    break;
                case "npc":
                    // Do nothing
                    break;
                default:
                    throw new Exception("Unknown channel");
            }
        }

        private static void Receive(JournalScanner.ReceiveTextEventArgs rArgs)
        {
            Newtonsoft.Json.Linq.JObject receiveText = rArgs.ReceiveText;
            string? cmdr = receiveText.Value<string>("From");
            string? message = receiveText.Value<string>("Message");
            string? channel = receiveText.Value<string>("Channel");
            WriteLine(cmdr, channel, message);
        }
    }
}
