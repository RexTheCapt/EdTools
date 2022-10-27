using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EdTools;

namespace EDChatTranslator
{
    internal class Program
    {
        private static string languageCode;
        private readonly static string MessageFormat = "[__TIMESTAMP__] __DIRECTION__ __FROM__ : __MESSAGE__";
        internal static void Main()
        {
            Console.Title = "ED Chat Translator";
            Console.WriteLine("Please input the language code you want to translate to:");
            languageCode = Console.ReadLine();
            Console.WriteLine($"Language code is set to {languageCode}, translation has started.");

            JournalScanner js = new JournalScanner();

            JournalScanner.ReceiveTextHandler += JournalScanner_ReceiveTextHandler;
            JournalScanner.SendTextHandler += JournalScanner_SendTextHandler;

            while (true)
            {
                js.TimerScan();

                DateTime end = DateTime.Now;
                Thread.Sleep(1000);

                bool brk = false;
                while (Console.KeyAvailable)
                    brk = Console.ReadKey(true).Key == ConsoleKey.Q;
                
                if (brk) break;
            }
        }

        private static void JournalScanner_SendTextHandler(object? sender, EventArgs e)
        {
            JournalScanner.SendTextEventArgs eArgs = (JournalScanner.SendTextEventArgs)e;
            if (eArgs.FirstRun) return;

            Newtonsoft.Json.Linq.JObject receiveText = eArgs.SendText;
            string? to = receiveText.Value<string>("To");
            string? message = receiveText.Value<string>("Message");
            bool? sent = receiveText.Value<bool>("Sent");
            DateTime? timestamp = receiveText.Value<DateTime>("timestamp");

            if (sent == null || !(bool)sent) return;
            if (to == null || to.Equals("npc")) return;
            if (message == null) return;
            if (sent == null) return;

            Console.WriteLine(FormatMessage((DateTime)timestamp, false, "me", message));

        }

        private static void JournalScanner_ReceiveTextHandler(object? sender, EventArgs e)
        {
            JournalScanner.ReceiveTextEventArgs eArgs = (JournalScanner.ReceiveTextEventArgs)e;
            if (eArgs.FirstRun) return;

            Newtonsoft.Json.Linq.JObject receiveText = eArgs.ReceiveText;
            string? channel = receiveText.Value<string>("Channel");
            string? message = receiveText.Value<string>("Message");
            string? from = receiveText.Value<string>("From");
            DateTime? timestamp = receiveText.Value<DateTime>("timestamp");

            if (channel == null || channel.Equals("npc")) return;
            if (message == null) return;
            if (from == null) return;

            string translatedMessage = Translate(message);

            Console.WriteLine(FormatMessage((DateTime)timestamp, true, from, translatedMessage));
        }

        private static string FormatMessage(DateTime timestamp, bool received, string from, string message)
        {
            string s = MessageFormat.Replace("__TIMESTAMP__", timestamp.ToString("HH:mm:ss")).Replace("__DIRECTION__", (received ? "<<<" : ">>>")).Replace("__FROM__", from).Replace("__MESSAGE__", message);
            return s;
        }

        private static string Translate(string v)
        {
            if (v == null) return null;

            var toLanguage = languageCode;//English
            var fromLanguage = "auto";//Deutsch
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={System.Web.HttpUtility.UrlEncode(v)}";
            using (var httpClient = new HttpClient())
            {
                var result = httpClient.GetStringAsync(url).Result;
                try
                {
                    string fix = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                    return fix;
                }
                catch
                {
                    return "Error";
                }
            }
        }
    }
}
