using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

using RtcLib;

namespace CoriolisBackupAssistant
{
    internal class Program
    {
        private static Settings settings;
        internal static void Main()
        {
            settings = new Settings("CoriolisBackupAssistant", "backup", "RexTheCapt", Settings.LocationEnum.Roaming);

            Console.WriteLine("Do you want to (1) save backup or (2) load backup?");

            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
            Console.WriteLine();

            switch (consoleKeyInfo.Key)
            {
                case ConsoleKey.D1:
                    Backup();
                    break;
                case ConsoleKey.D2:
                    Load();
                    break;
                default:
                    Console.WriteLine("Invalid option, choose either 1 or 2.");
                    break;
            }
        }

        private static void Load()
        {
            string v = settings.GetString("Backup");
            bool jsonConvertFail = false;
            JObject? j = null;
            try
            {
                j = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(v);
            } catch { jsonConvertFail = true; }

            if (v == null) return;

            object toPrint = (j == null ? v : j);

            Console.Clear();
            using (var sw = new StreamWriter("tmp.json"))
                sw.WriteLine(toPrint);

            Process.Start("notepad", "tmp.json");
            Console.WriteLine(toPrint);

            if (jsonConvertFail)
            {
                Console.Beep(400, 100);
                Console.Beep(400, 200);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("-----WARNING-----WARNING-----WARNING-----");
                Console.WriteLine("Something might be wrong with the json format!");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private static void Backup()
        {
            Console.WriteLine("How to use:\n" +
                "1: Open up https://coriolis.io/\n" +
                "2: Click on the button \"SETTINGS\" in top right of the page.\n" +
                "3: Click on the text \"Backup\" below \"Builds & Comparisons\"\n" +
                "4: Make sure all the text is marked in that textbox that popped up\n" +
                "5: Copy the text with CTRL+C or right clicking it and choosing copy\n" +
                "6: Paste it all in here with CTRL+V or right clicking this window\n" +
                "7: Press enter once you are done.");

            StringBuilder sb = new();
            int depth = 0;
            while (true)
            {
                string? v = Console.ReadLine()?.Trim();
                if (v == null || string.IsNullOrEmpty(v) || string.IsNullOrWhiteSpace(v)) break;

                depth += GetDepth(v);
                sb.Append(v);
                if (depth <= 0) break;
            }

            string v1 = sb.ToString();
            JObject? j = null;
            bool convertFailed = false;

            try { j = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(v1); } catch { convertFailed = true; }

            settings.SetString("Backup", v1);
            
            if (convertFailed)
            {
                Console.Beep(400, 100);
                Console.Beep(400, 200);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("-----WARNING-----WARNING-----WARNING-----");
                Console.WriteLine("Something is wrong with the json format, continue saving?");
                Console.Write("Y > ");
                Console.ForegroundColor = ConsoleColor.Gray;
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                if (consoleKeyInfo.Key != ConsoleKey.Y) goto SkipSave;
            }

            settings.Save();
            Console.WriteLine("Save complete");
        SkipSave:
            return;
        }

        private static int GetDepth(string v)
        {
            int d = 0;
            foreach (char c in v.ToUpper())
                if (c.Equals('{') || c.Equals('['))
                    d++;
                else if (c.Equals('}') || c.Equals(']'))
                    d--;
            
            return d;
        }
    }
}
