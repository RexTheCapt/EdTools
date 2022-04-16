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
        internal static void Main()
        {
            Console.WriteLine("Please input the language code you want to translate to:");
            languageCode = Console.ReadLine();
            Console.WriteLine($"Language code is set to {languageCode}, translation has started.");

            JournalScanner js = new JournalScanner();

            JournalScanner.ReceiveTextHandler += JournalScanner_ReceiveTextHandler;

            while (true)
            {
                js.TimerScan();

                DateTime end = DateTime.Now;
                Thread.Sleep(1000);
            }
        }

        private static void JournalScanner_ReceiveTextHandler(object? sender, EventArgs e)
        {
            /*
             * timestamp
             * event
             * From
             * Message
             * Message_Localised
             * Channel
             */

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

            Console.WriteLine($"[{timestamp}] {from}: {translatedMessage}");
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
