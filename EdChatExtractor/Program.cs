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
        private static List<EdMessage> edMessages = new List<EdMessage>();

        internal static void Main(string[] args)
        {
            JournalScanner journalScanner = new JournalScanner();

            JournalScanner.ReceiveTextHandler += HandleText;
            JournalScanner.SendTextHandler += HandleText;
            JournalScanner.LoadGameHandler += JournalScanner_LoadGameHandler;

            while (journalScanner.FirstRun)
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

            List<PlayerConsented> playerConsented = new();
            while (true)
            {
                foreach (var m in edMessages)
                    if (m.channel.Equals("npc")) continue;
                    else if (string.IsNullOrEmpty(m.cmdr) || string.IsNullOrWhiteSpace(m.cmdr)) continue;
                    else if (playerConsented.Find(x=>x.Cmdr.Equals(m.cmdr)) == null)
                        playerConsented.Add(new(m.cmdr, cmdr.Equals(m.cmdr)));

                Console.WriteLine($"{playerConsented.Count} player names found");
                PrintNames(playerConsented);
            Console.WriteLine();
                Console.WriteLine("Please write they're names if they consent, or \";brk\" to finish or write \";all\" to select everyone.");
                string? cmdrConsented = Console.ReadLine();

                if (cmdrConsented.StartsWith(";"))
                {
                    if (cmdrConsented.Equals(";brk"))
                        break;
                    else if (cmdrConsented.Equals(";all"))
                    {
                        foreach (var v in playerConsented)
                            v.HaveConsented();
                        break;
                    }
                }
                else if (cmdrConsented != null && !string.IsNullOrEmpty(cmdrConsented) && !string.IsNullOrWhiteSpace(cmdrConsented))
                    playerConsented.Find(x => x.Cmdr.Equals(cmdrConsented))?.HaveConsented();
            }

            Console.WriteLine($"Players consented: {GetConsentedNumber(playerConsented)}");
            WriteFiles(playerConsented);
        }

        private static void WriteFiles(List<PlayerConsented> playerConsented)
        {
            foreach (var v in edMessages)
            {
                var c = playerConsented.Find(x => x.Cmdr.Equals(v.cmdr));
                if (c == null) continue;
                if (!c.Consent) continue;
                WriteLine(v.cmdr, v.channel, v.message);
            }
        }

        private static int GetConsentedNumber(List<PlayerConsented> playerConsented)
        {
            int cnt = 0;
            foreach (var v in playerConsented)
                if (v.Consent)
                    cnt++;
            return cnt;
        }

        private static void PrintNames(List<PlayerConsented> playerConsented)
        {
            for (int i = 0; i < playerConsented.Count; i++)
            {
                if (playerConsented[i].Consent)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.Write(playerConsented[i].Cmdr);

                Console.ForegroundColor = ConsoleColor.Gray;
                if (i < playerConsented.Count - 1)
                    Console.Write(", ");
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

            AddEdMessage(cmdr, channel, message);
        }

        private static void AddEdMessage(string cmdr, string channel, string message)
        {
            //Console.WriteLine($"[{channel.ToUpper()}] {cmdr}: {message}");
            edMessages.Add(new(cmdr, channel, message));

            //switch (channel)
            //{
            //    case "wing":
            //        wingWriter.WriteLine($"{cmdr}: {message}");
            //        break;
            //    case "squadron":
            //        squadronWriter.WriteLine($"{cmdr}: {message}");
            //        break;
            //    case "npc":
            //        // Do nothing
            //        break;
            //    default:
            //        throw new Exception("Unknown channel");
            //}
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
            AddEdMessage(cmdr, channel, message);
        }
    }
}
