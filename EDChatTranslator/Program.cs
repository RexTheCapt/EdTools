﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EdTools;

namespace EDChatTranslator
{
    internal class Program
    {
        private static string? languageCode;
        private readonly static string MessageFormat = "[__TIMESTAMP__] __DIRECTION__ __FROM__ : __MESSAGE__";
        private static bool _handlersSet = false;

        internal static void Main()
        {
            Console.Title = "ED Chat Translator";
            Console.WriteLine("Please input the language code you want to translate to:");
            languageCode = Console.ReadLine();
            Console.WriteLine($"Language code is set to {languageCode}, translation has started.");

            JournalScanner js = new JournalScanner();

            if (!_handlersSet)
            {
                JournalScanner.ReceiveTextHandler += JournalScanner_ReceiveTextHandler;
                JournalScanner.SendTextHandler += JournalScanner_SendTextHandler;
                JournalScanner.UnknownEventHandler += JournalScanner_UnknownEventHandler;
                //JournalScanner.OnEventHandler += JournalScanner_OnEventHandler;
                _handlersSet = true;
            }

            while (true)
            {
                js.TimerScan();
                Console.ForegroundColor = ConsoleColor.Gray;

                DateTime end = DateTime.Now;
                Thread.Sleep(1000);

                bool brk = false;
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                    brk = consoleKeyInfo.Key == ConsoleKey.Q;

                    if (brk)
                        Console.WriteLine("Quitting...");

                    if (consoleKeyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) && consoleKeyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) && consoleKeyInfo.Key == ConsoleKey.R)
                    {
                        Main();
                        return;
                    }
                    else if (consoleKeyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) && consoleKeyInfo.Key == ConsoleKey.R)
                    {
                        Console.WriteLine("Re-reading journal...");
                        js.ReRead();
                    }
                }
                
                if (brk) break;
            }
        }

        private static void JournalScanner_OnEventHandler(object? sender, EventArgs e)
        {
            var ea = (JournalScanner.OnEventArgs)e;

            if (ea.FirstRun) return;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{ea.OnEvent.Value<string>("event")}");
        }

        private static void JournalScanner_UnknownEventHandler(object? sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            JournalScanner.UnknownEventArgs ea = (JournalScanner.UnknownEventArgs)e;
            Console.WriteLine($"The event \"{ea.UnknownEvent.Value<string>("event")}\" is unknown");
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

            string translatedMessage = Translate(message);

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(FormatMessage((DateTime)timestamp, false, "me", message));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(FormatMessage((DateTime)timestamp, false, "me", translatedMessage));
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

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(FormatMessage((DateTime)timestamp, true, from, message));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(FormatMessage((DateTime)timestamp, true, from, translatedMessage));
        }

        private static string FormatMessage(DateTime timestamp, bool received, string from, string message)
        {
            string s = MessageFormat.Replace("__TIMESTAMP__", timestamp.ToString("HH:mm:ss")).Replace("__DIRECTION__", (received ? "<<<" : ">>>")).Replace("__FROM__", from).Replace("__MESSAGE__", message);
            return s;
        }

        private static string Translate(string? v)
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
