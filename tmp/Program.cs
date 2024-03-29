﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpDiscordWebhook.NET.Discord;

using EdTools;

using WebhookTest.Exceptions;

namespace WebhookTest
{
    internal class Program
    {
        internal static DiscordWebhook hook;
        internal static JournalScanner scanner;
        internal static List<DiscordMessage> messageQueue = new List<DiscordMessage>();
        internal static string? carrierName = null;
        internal static string carrierID = "3706547712";
        internal static EmbedFooter embedFooterJournal;
        internal static bool eventMsg = false;

        internal static async Task Main()
        {
            Console.Title = "Carrier Announcer";

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

                        DiscordMessage message = new DiscordMessage()
                        {
                            AvatarUrl = @"https://cdn.discordapp.com/attachments/295942440668495873/931611914042228746/Untitled-1.PNG",
                            //AvatarUrl = @"https://cdn.discordapp.com/attachments/930896737688760330/931589138111680592/Untitled-8.PNG",
                            //Username = "Captain (QBZ-T4J)"
                            Username = "Captain B. McCrea",
                            //Content = read
                        };
                        DiscordEmbed embed = new DiscordEmbed()
                        {
                            Color = System.Drawing.Color.Beige,
                            Fields =
                            {
                                new EmbedField()
                                {
                                    InLine = true,
                                    Name = "Message from the captain",
                                    Value = read
                                }
                            },
                            Footer = new EmbedFooter()
                            {
                                Text = "Direct"
                            },
                            Timestamp = DateTime.Now,
                        };
                        message.Embeds.Add(embed);
                        messageQueue.Add(message);
                        Console.WriteLine();
                    }
                }

                if (eventMsg)
                {
                    Console.WriteLine();
                    eventMsg = false;
                }

                if (Console.CursorTop != 0)
                    Console.CursorTop--;

                Console.Write($"{DateTime.Now}\n");
                scanner.TimerScan(sendEventsOnFirstRun: false);

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
            string? cid = eArgs.CarrierJumpCancelled.Value<string>("CarrierID");
            
            if (cid == null && !cid.Equals(carrierID))
                return;
            
            eventMsg = true;
            Console.WriteLine("Jump cancelled");
            DiscordMessage message = NewMessage("bridge crew");
            DiscordEmbed embed = new DiscordEmbed()
            {
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
            string? cid = eArgs.CarrierTradeOrder.Value<string>("CarrierID");

            if (cid == null && !cid.Equals(carrierID))
                return;

            eventMsg = true;

            Console.WriteLine("Trade updated");
            //Console.WriteLine(eArgs.CarrierTradeOrder.ToString());

            bool blackMarket = eArgs.CarrierTradeOrder.Value<bool>("BlackMarket");
            string? commodity = eArgs.CarrierTradeOrder.Value<string>("Commodity");
            int? purchaseOrder = eArgs.CarrierTradeOrder?.Value<int?>("PurchaseOrder");
            int? saleOrder = eArgs.CarrierTradeOrder?.Value<int?>("SaleOrder");
            int? price = eArgs.CarrierTradeOrder?.Value<int?>("Price");

            DiscordMessage message = NewMessage("commodity trading");
            DiscordEmbed embed = new DiscordEmbed()
            {
                //Author = embedAuthorCommodityTrading,
                Title = $"Trade updated{(blackMarket ? " (Black market)" : "")}",
                Footer = embedFooterJournal,
                Timestamp = eArgs.CarrierTradeOrder.Value<DateTime>("timestamp"),
                Color = System.Drawing.Color.CadetBlue
            };

            #region Order type
            if (saleOrder != null && purchaseOrder == null) // Sell order
            {
                embed.Fields.Add(new EmbedField() { InLine = true, Name = "Type", Value = "Sale" });
                embed.Color = System.Drawing.Color.Khaki;
            }
            if (purchaseOrder != null && saleOrder == null) // Buy order
            {
                embed.Fields.Add(new EmbedField() { InLine = true, Name = "Type", Value = "Buy" });
                embed.Color = System.Drawing.Color.Ivory;
            }
            else if (purchaseOrder == null && saleOrder == null) // Removed order
            {
                embed.Fields.Add(new EmbedField() { InLine = true, Name = "Type", Value = "Removed" });
                embed.Color = System.Drawing.Color.IndianRed;
            }
            #endregion

            embed.Fields.Add(new EmbedField() { InLine = true, Name = "Commodity", Value = commodity });
            
            if (price != null)
                embed.Fields.Add(new EmbedField() { InLine = true, Name = "Price", Value = price.ToString() });

            message.Embeds.Add(embed);
            messageQueue.Add(message);
        }

        private static DiscordMessage NewMessage(string type)
        {
            eventMsg = true;
            DiscordMessage message = new();

            switch (type)
            {
                case "commodity trading":
                    message.AvatarUrl = @"https://cdn.discordapp.com/attachments/930896737688760330/931584039180206170/Untitled-6.PNG";
                    message.Username = "Regan Heath (QBZ-T4J)";
                    return message;
                case "bridge crew":
                    message.AvatarUrl = @"https://cdn.discordapp.com/attachments/930896737688760330/931583036129169428/Untitled-4.PNG";
                    message.Username = "Siya Pittman (QBZ-T4J)";
                    return message;
                default:
                    throw new UnknownMessageTypeException();
            }

        }

        private static void JournalScanner_CarrierJumpHandler(object? sender, EventArgs e)
        {
            JournalScanner.CarrierJumpEventArgs eArgs = (JournalScanner.CarrierJumpEventArgs)e;
            string? cid = eArgs.CarrierJump.Value<string>("CarrierID");

            if (cid == null && !cid.Equals(carrierID))
                return;

            eventMsg = true;

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
            string? cid = eArgs.CarrierJumpRequest.Value<string>("CarrierID");

            if (cid == null && !cid.Equals(carrierID))
                return;

            eventMsg = true;

            Console.WriteLine("Jump request");
            //messageQueue.Add(eArgs.CarrierJumpRequest.ToString());
            //messageQueue.Add("Jump requested");

            string fromSystem = eArgs.CarrierJumpRequest.Value<string>("SystemName");
            string fromBody = eArgs.CarrierJumpRequest.Value<string>("Body").Replace(fromSystem, "").Trim();
            DateTime timestamp = eArgs.CarrierJumpRequest.Value<DateTime>("timestamp");
            DateTime timestampJumpTime = timestamp.AddMinutes(15);

            Int32 unixTimestamp = (Int32)(timestampJumpTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //Console.WriteLine(unixTimestamp);
            DiscordMessage message = NewMessage("bridge crew");
            DiscordEmbed embed = new DiscordEmbed()
            {
                Color = System.Drawing.Color.Green,
                Title = "Carrier jump requested.",
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
                        Name = "Countdown",
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
