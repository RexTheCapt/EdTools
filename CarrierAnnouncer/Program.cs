using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpDiscordWebhook.NET.Discord;

using EdTools;

using CarrierAnnouncer.Exceptions;

namespace CarrierAnnouncer
{
    internal class Program
    {
        internal static List<DiscordWebhook> hooks = new();
        internal static JournalScanner scanner;
        internal static List<DiscordMessage> messageQueue = new List<DiscordMessage>();
        internal static string carrierID = "3706547712";
        internal static EmbedFooter embedFooterJournal;
        internal static bool eventMsgSent = false;
        internal static string? targetSystem = null;
        internal static string? targetBody = null;

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

            List<Uri> hookUris = new();

            #region setup vars
            string[,] vars = new string[,] {
                { "hookuri", "Please enter discord hook URI" },
            };

            for (int i = 0; i < vars.GetLength(0); i++)
            {
                string file = vars[i, 0];
                string text = vars[i, 1];

                if (!File.Exists(file))
                {
                    Console.WriteLine(text);
                    Console.Write(": ");
                    string[] uris = Console.ReadLine().Split(';');

                    using (var w = new StreamWriter(file))
                        foreach (var u in uris)
                            w.WriteLine(u);
                }
            }

            using (var r = new StreamReader("hookuri"))
            {
                while (!r.EndOfStream)
                {
                    string? v = r.ReadLine();
                    hookUris.Add(new Uri(v));
                }
            }
            #endregion

            foreach (Uri uri in hookUris)
            {
                hooks.Add(new DiscordWebhook()
                {
                    Uri = uri
                });
            }

            #region setup embeds
            embedFooterJournal = new EmbedFooter()
            {
                Text = "From journal"
            };
            #endregion

            Console.Clear();

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
                    else if (consoleKeyInfo.Key == ConsoleKey.T)
                    {
                        targetBody = targetSystem = null;
                        Console.WriteLine("---INTERUPTED--");
                        Console.Write("Please input target system or leave blank to cancel\n: ");

                        string? read = Console.ReadLine();

                        if (string.IsNullOrEmpty(read) || string.IsNullOrWhiteSpace(read))
                        {
                            Console.WriteLine("Target system cleared.\n");
                            continue;
                        }
                        else
                        {
                            targetSystem = read.Trim().ToUpper();
                            Console.Write("Please input target body or leave blank to cancel\n: ");

                            read = Console.ReadLine();

                            if (!string.IsNullOrEmpty(read) && !string.IsNullOrWhiteSpace(read))
                                targetBody = read.Trim().ToUpper();
                        }

                        Console.WriteLine($"Target system: {targetSystem} > {targetBody}\n");
                    }
                }

                if (eventMsgSent)
                {
                    Console.WriteLine();
                    eventMsgSent = false;
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
                        messageQueue[0].Username = "Carrier Announcer";

                    foreach (var hook in hooks)
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
            
            eventMsgSent = true;
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

            eventMsgSent = true;

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
            eventMsgSent = true;
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
            string? stationName = eArgs.CarrierJump.Value<string>("StationName");

            if (stationName == null || !stationName.Equals("QBZ-T4J"))
                return;

            eventMsgSent = true;

            string? starSystem = eArgs.CarrierJump.Value<string>("StarSystem");
            string? body = eArgs.CarrierJump.Value<string>("Body")?.Replace(starSystem, "").Trim();

            Console.WriteLine("Jumped");
            
            DiscordMessage message = NewMessage("brudge crew");
            DiscordEmbed embed = new DiscordEmbed()
            {
                Color = System.Drawing.Color.Green,
                Title = "Carrier jump completed",
                Footer = embedFooterJournal
            };

            if (starSystem != null)
            {
                embed.Fields.Add(new EmbedField()
                {
                    InLine = true,
                    Name = "System",
                    Value = starSystem
                });

                if (body != null && !string.IsNullOrEmpty(body))
                    embed.Fields.Add(new EmbedField()
                    {
                        InLine = true,
                        Name = "Body",
                        Value = body
                    });
            }

            message.Embeds.Add(embed);
            messageQueue.Add(message);
        }

        private static void JournalScanner_CarrierJumpRequestHandler(object? sender, EventArgs e)
        {
            JournalScanner.CarrierJumpRequestEventArgs eArgs = (JournalScanner.CarrierJumpRequestEventArgs)e;
            string? cid = eArgs.CarrierJumpRequest.Value<string>("CarrierID");

            if (cid == null && !cid.Equals(carrierID))
                return;

            eventMsgSent = true;

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
                        Value = fromSystem.Trim().ToUpper(),
                    },
                    new EmbedField()
                    {
                        InLine = true,
                        Name = "Current body",
                        Value = fromBody.Trim().ToUpper()
                    }
                },
                Footer = embedFooterJournal
            };

            if (targetSystem != null)
            {
                embed.Fields.Add(new EmbedField()
                {
                    InLine = false,
                    Name = "Target System",
                    Value = targetSystem
                });
                targetSystem = null;
            }

            if (targetBody != null)
            {
                embed.Fields.Add(new EmbedField()
                {
                    InLine = true,
                    Name = "Target Body",
                    Value = targetBody
                });
                targetSystem = null;
            }

            embed.Fields.Add(new EmbedField()
            {
                InLine = false,
                Name = "Countdown",
                Value = $"<t:{unixTimestamp}:R>"
            });

            message.Embeds.Add(embed);
            messageQueue.Add(message);
            //messageQueue.Add($"Jumping from `{fromSystem}` body `{fromBody}` <t:{unixTimestamp}:R>");
        }
    }
}
