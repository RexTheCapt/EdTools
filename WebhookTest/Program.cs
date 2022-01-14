using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpDiscordWebhook.NET.Discord;

using EdTools;

namespace WebhookTest
{
    internal class Program
    {
        internal static DiscordWebhook hook;
        internal static JournalScanner scanner;
        internal static List<DiscordMessage> messageQueue = new List<DiscordMessage>();
        internal static string? carrierName = null;
        internal static EmbedAuthor embedAuthorCarrier;
        internal static EmbedFooter embedFooterJournal;

        internal static async Task Main()
        {
            #region setup journal
            scanner = new EdTools.JournalScanner(@"C:\Users\user\Saved Games\Frontier Developments\Elite Dangerous");
            JournalScanner.CarrierJumpRequestHandler += JournalScanner_CarrierJumpRequestHandler;
            JournalScanner.CarrierJumpHandler += JournalScanner_CarrierJumpHandler;
            JournalScanner.CarrierTradeOrderHandler += JournalScanner_CarrierTradeOrderHandler;
            JournalScanner.CarrierJumpCancelledHandler += JournalScanner_CarrierJumpCancelledHandler;
            #endregion

            Uri? hookUri = null;

            #region setup vars
            string[,] vars = new string[,] {
                { "hookuri", "Please enter discord hook URI" },
                { "carriername", "Please enter carrier name" }
            };

            for (int i = 0; i < vars.GetLength(0); i++)
            {
                string file = vars[i, 0];
                string text = vars[i, 1];

                if (!File.Exists(file))
                {
                    Console.WriteLine(text);
                    Console.Write(": ");

                    using (var w = new StreamWriter(file))
                        w.Write(Console.ReadLine());
                }
            }

            using (var r = new StreamReader("hookuri"))
            {
                string? v = r.ReadLine();
                hookUri = new Uri(v);
            }

            using (var r = new StreamReader("carriername"))
            {
                string? v = r.ReadLine();
                carrierName = v;
            }
            #endregion
            
            hook = new DiscordWebhook()
            {
                Uri = hookUri
            };

            #region setup embeds
            embedAuthorCarrier = new EmbedAuthor()
            {
                Name = carrierName ?? "Carrier Announcer"
            };

            embedFooterJournal = new EmbedFooter()
            {
                Text = "From journal"
            };
            #endregion

            while (true)
            {
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);

                    if (consoleKeyInfo.Key == ConsoleKey.I)
                    {
                        Console.WriteLine("---INTERUPTED---");
                        Console.Write("Write a message to send or leave blank to cancel\n: ");

                        string? read = Console.ReadLine();

                        if (string.IsNullOrEmpty(read) || string.IsNullOrWhiteSpace(read))
                            continue;

                        DiscordMessage message = new DiscordMessage();
                        DiscordEmbed embed = new DiscordEmbed()
                        {
                            Author = new EmbedAuthor()
                            {
                                IconUrl = @"https://cdn.discordapp.com/avatars/272411566111064074/a4e4fc367c7f5de6cdb0b3ca3d2beaba.webp",
                                Name = "CMDR RexTheCapt",
                                Url = @"https://discord.gg/jUdRUWK"
                            },
                            Color = System.Drawing.Color.Beige,
                            Description = "Message from the CMDR",
                            Fields =
                            {
                                new EmbedField()
                                {
                                    InLine = true,
                                    Name = "Message",
                                    Value = read
                                }
                            },
                            Footer = new EmbedFooter()
                            {
                                Text = "Message from CMDR"
                            },
                            Timestamp = DateTime.Now,
                            Title = "Message from CMDR"
                        };
                        message.Embeds.Add(embed);
                        messageQueue.Add(message);
                    }
                }

                Console.WriteLine($"Scanning {DateTime.Now}");
                scanner.TimerScan(null, new EventArgs());

                if (messageQueue.Count > 0)
                {
                    //DiscordMessage message = new DiscordMessage()
                    //{
                    //    Content = messageQueue[0],
                    //    Username = carrierName ?? "Carrier Announcer"
                    //};

                    if (messageQueue[0].Username == null)
                        messageQueue[0].Username = carrierName ?? "Carrier Announcer";

                    await hook.SendAsync(messageQueue[0]);
                    messageQueue.RemoveAt(0);
                }

                Thread.Sleep(1000);
            }

            //DiscordMessage message = new DiscordMessage()
            //{
            //    Content = "Test",
            //    TTS = false,
            //    Username = "Test Boy"
            //};

            //await hook.SendAsync(message);
            //Thread.Sleep(5000);
            //await hook.SendAsync(message);
        }

        private static void JournalScanner_CarrierJumpCancelledHandler(object? sender, EventArgs e)
        {
            JournalScanner.CarrierJumpCancelledEventArgs eArgs = (JournalScanner.CarrierJumpCancelledEventArgs)e;
            Console.WriteLine("Jump cancelled");
            DiscordMessage message = new DiscordMessage();
            DiscordEmbed embed = new DiscordEmbed()
            {
                Author = embedAuthorCarrier,
                Color = System.Drawing.Color.Red,
                Title = "Carrier jump cancelled",
                Footer = embedFooterJournal,
                Timestamp = eArgs.CarrierJumpCancelled.Value<DateTime>("timestamp")
            };
            message.Embeds.Add(embed);
            messageQueue.Add(message);
        }

        private static void JournalScanner_CarrierTradeOrderHandler(object? sender, EventArgs e)
        {
            JournalScanner.CarrierTradeOrderEventArgs eArgs = (JournalScanner.CarrierTradeOrderEventArgs)e;
            Console.WriteLine(eArgs.CarrierTradeOrder.ToString());

            bool blackMarket = eArgs.CarrierTradeOrder.Value<bool>("BlackMarket");
            string? commodity = eArgs.CarrierTradeOrder.Value<string>("Commodity");
            int? purchaseOrder = eArgs.CarrierTradeOrder?.Value<int?>("PurchaseOrder");
            int? saleOrder = eArgs.CarrierTradeOrder?.Value<int?>("SaleOrder");
            int? price = eArgs.CarrierTradeOrder?.Value<int?>("price");

            DiscordMessage message = new DiscordMessage();
            DiscordEmbed embed = new DiscordEmbed()
            {
                Author = new EmbedAuthor()
                {
                    Name = "Commodity Trading (QBZ-T4J)"
                },
                Title = $"Trade updated{(blackMarket ? " (Black market)" : "")}",
                Footer = embedFooterJournal
            };

            #region Order type
            if (saleOrder != null)
                embed.Fields.Add(new EmbedField() { InLine = true, Name = "Type", Value = "Sale" });
            if (purchaseOrder != null)
                embed.Fields.Add(new EmbedField() { InLine = true, Name = "Type", Value = "Buy" });
            #endregion

            embed.Fields.Add(new EmbedField() { InLine = true, Name = "Commodity", Value = commodity });
            embed.Fields.Add(new EmbedField() { InLine = true, Name = "Price", Value = price.ToString() });

            message.Embeds.Add(embed);
            messageQueue.Add(message);
        }

        private static void JournalScanner_CarrierJumpHandler(object? sender, EventArgs e)
        {
            Console.WriteLine("Jumped");

            DiscordMessage message = new DiscordMessage()
            {
                Content = "Jumped, <@272411566111064074> default message."
            };

            messageQueue.Add(message);
        }

        private static void JournalScanner_CarrierJumpRequestHandler(object? sender, EventArgs e)
        {
            JournalScanner.CarrierJumpRequestEventArgs eArgs = (JournalScanner.CarrierJumpRequestEventArgs)e;
            Console.WriteLine("Jump request");
            //messageQueue.Add(eArgs.CarrierJumpRequest.ToString());
            //messageQueue.Add("Jump requested");

            string fromSystem = eArgs.CarrierJumpRequest.Value<string>("SystemName");
            string fromBody = eArgs.CarrierJumpRequest.Value<string>("Body").Replace(fromSystem, "").Trim();
            DateTime timestamp = eArgs.CarrierJumpRequest.Value<DateTime>("timestamp");
            DateTime timestampJumpTime = timestamp.AddMinutes(15);

            Int32 unixTimestamp = (Int32)(timestampJumpTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //Console.WriteLine(unixTimestamp);
            DiscordMessage message = new DiscordMessage();
            DiscordEmbed embed = new DiscordEmbed()
            {
                Author = embedAuthorCarrier,
                Color = System.Drawing.Color.Green,
                Description = "Carrier jump requested.",
                Timestamp = timestamp,
                Fields =
                {
                    new EmbedField()
                    {
                        InLine = true,
                        Name = "Current system",
                        Value = fromSystem
                    },
                    new EmbedField()
                    {
                        InLine = true,
                        Name = "Current body",
                        Value = fromBody
                    },
                    new EmbedField()
                    {
                        InLine = false,
                        Name = "Jumping in",
                        Value = $"<t:{unixTimestamp}:R>"
                    }
                },
                Footer = embedFooterJournal
            };
            message.Embeds.Add(embed);
            messageQueue.Add(message);
            //messageQueue.Add($"Jumping from `{fromSystem}` body `{fromBody}` <t:{unixTimestamp}:R>");
        }
    }
}
