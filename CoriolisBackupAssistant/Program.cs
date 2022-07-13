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
        private static bool useNotePad = false;
        private static BackupType backupType;

        internal static void Main(string[] args)
        {
#if DEBUG
            //args = new string[] { "--save", "--notepad" };
#endif
            #region arg handler
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    string s = args[i].Substring(2);

                    switch (s.ToLower())
                    {
                        case "load":
                            backupType |= BackupType.Load;
                            break;
                        case "save":
                        case "backup":
                            backupType |= BackupType.Save;
                            break;
                        case "notepad":
                            useNotePad = true;
                            break;
                        case "help":
                        default:
                            Console.WriteLine("Arguments:\n" +
                                              "--load       | Load saved backup.\n" +
                                              "--save       | Write new backup.\n" +
                                              "--backup     | Write new backup.\n" +
                                              "--notepad    | Use notepad to write/load.\n" +
                                              "--help       | --help\n");
                            return;
                    }
                }
                else
                    throw new Exception("Arguments are required to start with --");
            }
            #endregion

            settings = new Settings("CoriolisBackupAssistant", "backup", "RexTheCapt", Settings.LocationEnum.Roaming);

            if (backupType == BackupType.None)
            {
                Console.WriteLine("Do you want to (1) save backup or (2) load backup?");

                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                Console.WriteLine();


                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.D1:
                        Save();
                        break;
                    case ConsoleKey.D2:
                        Load();
                        break;
                    default:
                        Console.WriteLine("Invalid option, choose either 1 or 2.");
                        break;
                }
            }
            else if (((int)backupType) > 2)
                throw new Exception("Cant do both load and save, choose one.");
            else if (backupType == BackupType.Load)
                Load();
            else if (backupType == BackupType.Save)
                Save();
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
            if (useNotePad)
            {
                using (var sw = new StreamWriter("tmp.json"))
                    sw.WriteLine(toPrint);

                Process.Start("notepad", "tmp.json");
            }
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

        private static void Save()
        {
            StringBuilder sb;
            if (!useNotePad)
            {
                Console.WriteLine("How to use:\n" +
                    "1: Open up https://coriolis.io/\n" +
                    "2: Click on the button \"SETTINGS\" in top right of the page.\n" +
                    "3: Click on the text \"Backup\" below \"Builds & Comparisons\"\n" +
                    "4: Make sure all the text is marked in that textbox that popped up\n" +
                    "5: Copy the text with CTRL+C or right clicking it and choosing copy\n" +
                    "6: Paste it all in here with CTRL+V or right clicking this window\n" +
                    "7: Press enter once you are done.");
                sb = new();
                int depth = 0;
                while (true)
                {
                    string? v = Console.ReadLine()?.Trim();
                    if (v == null || string.IsNullOrEmpty(v) || string.IsNullOrWhiteSpace(v)) break;

                    depth += GetDepth(v);
                    sb.Append(v);
                    if (depth <= 0) break;
                }
            }
            else
            {
                if (File.Exists("tmp.json"))
                    File.Delete("tmp.json");

                using (var stream = File.Create("tmp.json"))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("How to use:\n" +
                                     "1: Open up https://coriolis.io/\n" +
                                     "2: Click on the button \"SETTINGS\" in top right of the page.\n" +
                                     "3: Click on the text \"Backup\" below \"Builds & Comparisons\".\n" +
                                     "4: Make sure all the text is marked in that textbox that popped up.\n" +
                                     "5: Copy the text with CTRL+C or right clicking it and choosing copy.\n" +
                                     "6: Delete all text in here by pressing CTRL+A and backspace or delete.\n" +
                                     "7: Paste all copied text in here with CTRL+V or right clicking this window.\n" +
                                     "8: Save by pressing CTRL+S and close this window.");
                }

                Process.Start("notepad", "tmp.json");
                System.IO.FileInfo fi = new("tmp.json");
                DateTime creationDatetime = fi.LastWriteTime;

                while (creationDatetime == fi.LastWriteTime)
                {
                    fi = new("tmp.json");
                    Thread.Sleep(1000);
                }

                sb = new();

                using (var stream = File.OpenRead("tmp.json"))
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                        sb.Append(reader.ReadLine());
                }
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
                Console.Write("[Y/N] >> ");
                Console.ForegroundColor = ConsoleColor.Gray;
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                if (consoleKeyInfo.Key != ConsoleKey.Y)
                {
                    Console.Clear();
                    Console.WriteLine("Follow the instructions in the popup.");
                    Save();
                    return;
                }
            }

            settings.Save();
            Console.WriteLine("Save complete");
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

        private enum BackupType
        {
            None = 0,
            Load = 1,
            Save = 2,
            Invalid = 3
        }
    }
}
