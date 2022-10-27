using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdTools
{
    public class JournalScanner
    {
        public DateTime LastEventTime { get; private set; }
        public DateTime LastProcessedEventTime { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        public bool FirstRun { get => _firstRun; private set { _firstRun = value; } }
        private bool _firstRun = true;
        private string _journalPath;
        private static JournalScanner? _instance = null;

        public JournalScanner()
        {
            if (_instance != null) throw new Exception("Can only exist one journal scanner!");

            _journalPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Saved Games\\Frontier Developments\\Elite Dangerous";
        }

        /// <summary>
        /// Run the journalscanner
        /// </summary>
        public void Tick()
        {
            TimerScan();
        }

        public void ScanAll()
        {
            string[] journals = Directory.GetFiles(_journalPath, "Journal.*.log").OrderBy(x=>(new FileInfo(x)).LastWriteTime).ToArray();

            foreach (string s in journals)
                TimerScan(s);
        }

        public JournalScanner (string journalPath)
        {
            _journalPath = journalPath;
        }

        public void TimerScan(object? sender, System.EventArgs e) => TimerScan();
        public void TimerScan() => TimerScan(sendEventsOnFirstRun: true, overrideFile: null);
        public void TimerScan(string overrideFile) => TimerScan(sendEventsOnFirstRun: true, overrideFile: overrideFile);
        public void TimerScan(bool sendEventsOnFirstRun = true, string? overrideFile = null)
        {
            string newest = "";
            DateTime currentWriteTime = DateTime.MinValue;

            if (overrideFile == null)
                foreach (string f in Directory.GetFiles(_journalPath, "Journal.*.log"))
                {
                    FileInfo fi = new FileInfo(f);

                    if (fi.LastWriteTime > currentWriteTime)
                    {
                        currentWriteTime = fi.LastWriteTime;
                        newest = f;
                    }
                }

            if (LastWriteTime != currentWriteTime || overrideFile != null)
            {
                if (overrideFile != null)
                    newest = overrideFile;

                var fs = new FileStream(newest, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var reader = new StreamReader(fs))
                    while (!reader.EndOfStream)
                    {
                        string? read = reader.ReadLine();

                        if (read == null) continue;

                        JObject? @event = (JObject?)JsonConvert.DeserializeObject(read);
                        if (@event != null)
                        {
                            DateTime currentEventDateTime = @event.Value<DateTime>("timestamp");
                            if (currentEventDateTime >= LastEventTime && currentEventDateTime != LastProcessedEventTime || overrideFile != null)
                            {
                                string? eventType = @event.Value<string>("event");

                                if (!sendEventsOnFirstRun && FirstRun && overrideFile == null)
                                    goto SkipEventSend;

                                switch (eventType)
                                {
                                    case "Loadout":
                                        OnLoadout(new LoadoutEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "SupercruiseEntry":
                                        OnSupercruiseEntry(new SupercruiseEntryEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "FSDJump":
                                        OnFSDJump(new FSDJumpEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "SupercruiseExit":
                                        OnSupercruiseExit(new SupercruiseExitEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Friends":
                                        OnFriends(new FriendsEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "LeaveBody":
                                        OnLeaveBody(new LeaveBodyEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "ApproachBody":
                                        OnApproachBody(new ApproachBodyEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Docked":
                                        OnDocked(new DockedEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "ReservoirReplenished":
                                        OnReservoirReplenished(new ReservoirReplenishedEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "LoadGame":
                                        OnLoadGame(new LoadGameEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Location":
                                        OnLocation(new LocationEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Scan":
                                        OnScan(new ScanEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "FuelScoop":
                                        OnFuelScoop(new FuelScoopEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "ShipTargeted":
                                        OnShipTargeted(new ShipTargetedEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "ScanBaryCentre":
                                        OnScanBaryCentre(new ScanBaryCentreEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "JetConeBoost":
                                        OnJetConeBoost(new JetConeBoostEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "WingInvite":
                                        OnWingInvite(new WingInviteEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "WingAdd":
                                        OnWingAdd(new WingAddEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "WingLeave":
                                        OnWingLeave(new WingLeaveEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "WingJoin":
                                        OnWingJoin(new WingJoinEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "RepairDrone":
                                        OnRepairDrone(new RepairDroneEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "SellDrones":
                                        OnSellDrones(new SellDronesEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "BuyDrones":
                                        OnBuyDrones(new BuyDronesEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "CockpitBreached":
                                        OnCockpitBreached(new CockpitBreachedEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Touchdown":
                                        OnTouchdown(new TouchdownEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Liftoff":
                                        OnLiftoff(new LiftoffEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Disembark":
                                        OnDisembark(new DisembarkEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Embark":
                                        OnEmbark(new EmbarkEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "StoredShips":
                                        OnStoredShips(new StoredShipsEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Outfitting":
                                        OnOutfitting(new OutfittingEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Shipyard":
                                        OnShipyard(new ShipyardEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Market":
                                        OnMarket(new MarketEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "StoredModules":
                                        OnStoredModules(new StoredModulesEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "CarrierJump":
                                        OnCarrierJump(new CarrierJumpEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "CarrierDepositFuel":
                                        OnCarrierDepositFuel(new CarrierDepositFuelEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "CarrierStats":
                                        OnCarrierStats(new CarrierStatsEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "CarrierJumpRequest":
                                        OnCarrierJumpRequest(new CarrierJumpRequestEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "CarrierTradeOrder":
                                        OnCarrierTradeOrder(new CarrierTradeOrderEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "CarrierJumpCancelled":
                                        OnCarrierJumpCancelled(new CarrierJumpCancelledEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "Commander":
                                        OnCommander(new CommanderEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "SendText":
                                        OnSendText(new SendTextEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "UseConsumable":
                                        OnUseConsumable(new UseConsumableEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "RefuelAll":
                                        OnRefuelAll(new RefuelAllEventArgs(@event: @event, FirstRun));
                                        break;
                                    case "ReceiveText":
                                        OnReceiveText(new ReceiveTextEventArgs(@event: @event, FirstRun));
                                        break;
                                    #region unused events
                                    //case "UseConsumable":
                                    case "UpgradeWeapon":
                                    case "ShipyardNew":
                                    case "BuyWeapon":
                                    case "SellMicroResources":
                                    case "TradeMicroResources":
                                    case "BuySuit":
                                    case "LeftSquadron":
                                    case "CommunityGoalReward":
                                    case "CrewLaunchFighter":
                                    case "BookTaxi":
                                    case "ShipyardBuy":
                                    case "DeleteSuitLoadout":
                                    case "PayFines":
                                    case "CommunityGoalJoin":
                                    case "AppliedToSquadron":
                                    case "ModuleSell":
                                    case "ModuleSwap":
                                    case "SetUserShipName":
                                    case "SelfDestruct":
                                    case "ShipyardSell":
                                    case "MissionAbandoned":
                                    case "Died":
                                    case "FetchRemoteModule":
                                    case "VehicleSwitch":
                                    case "DataScanned":
                                    case "SquadronStartup":
                                    case "RestockVehicle":
                                    case "LaunchSRV":
                                    case "Interdiction":
                                    case "CrimeVictim":
                                    case "MaterialDiscovered":
                                    case "EscapeInterdiction":
                                    case "SwitchSuitLoadout":
                                    case "CrewAssign":
                                    case "BuyMicroResources":
                                    case "ProspectedAsteroid":
                                    case "CodexEntry":
                                    case "Interdicted":
                                    case "ShipyardTransfer":
                                    case "BackpackChange":
                                    case "RebootRepair":
                                    case "MultiSellExplorationData":
                                    case "JetConeDamage":
                                    case "USSDrop":
                                    case "CollectCargo":
                                    case "MaterialTrade":
                                    case "CargoTransfer":
                                    case "FactionKillBond":
                                    case "DockFighter":
                                    case "CommitCrime":
                                    case "CollectItems":
                                    case "DockSRV":
                                    case "Resurrect":
                                    case "HeatDamage":
                                    case "ApproachSettlement":
                                    case "Synthesis":
                                    case "FighterDestroyed":
                                    case "SAASignalsFound":
                                    case "MissionCompleted":
                                    case "Backpack":
                                    case "ModuleStore":
                                    case "SAAScanComplete":
                                    case "ModuleSellRemote":
                                    case "ModuleRetrieve":
                                    case "MissionFailed":
                                    case "MarketSell":
                                    case "ModuleBuy":
                                    case "RedeemVoucher":
                                    case "MissionRedirected":
                                    case "Repair":
                                    case "FighterRebuilt":
                                    case "EngineerCraft":
                                    case "ShieldState":
                                    case "EjectCargo":
                                    case "AfmuRepairs":
                                    case "LaunchDrone":
                                    case "Bounty":
                                    case "SuitLoadout":
                                    case "Scanned":
                                    case "BuyAmmo":
                                    case "LaunchFighter":
                                    case "UnderAttack":
                                    case "FSSAllBodiesFound":
                                    case "FSSBodySignals":
                                    case "HeatWarning":
                                    case "NavRoute":
                                    case "MaterialCollected":
                                    case "ShipLocker":
                                    case "FSSDiscoveryScan":
                                    case "FSDTarget":
                                    case "ModuleInfo":
                                    case "Progress":
                                    case "Missions":
                                    case "Materials":
                                    case "MarketBuy":
                                    case "Fileheader":
                                    case "Rank":
                                    case "Shutdown":
                                    case "EngineerProgress":
                                    case "ShipyardSwap":
                                    case "CommunityGoal":
                                    case "DockingDenied":
                                    case "MissionAccepted":
                                    case "Cargo":
                                    case "Reputation":
                                    case "DockingGranted":
                                    case "Statistics":
                                    case "FSSSignalDiscovered":
                                    case "RepairAll":
                                    case "NpcCrewPaidWage":
                                    case "CargoDepot":
                                    case "Undocked":
                                    case "Music":
                                    case "DockingRequested":
                                    case "HullDamage":
                                    case "StartJump":
                                    case "Powerplay":
                                    case "Promotion":
                                    case "SellOrganicData":
                                    case "CarrierBankTransfer":
                                    case "PVPKill":
                                    case "Passengers":
                                        break;
                                    #endregion
                                    default:
                                        //Console.WriteLine($"UNKNOWN_EVENT: {eventType}");
                                        OnUnknown(new UnknownEventArgs(@event: @event, FirstRun));
                                        break;
                                }

                                OnEvent(new OnEventArgs(@event: @event, FirstRun));

                            SkipEventSend:
                                if (overrideFile == null)
                                    LastEventTime = currentEventDateTime;
                            }
                        }
                    }
            }

            FirstRun = false;

            LastProcessedEventTime = LastEventTime;
        }

        public void ReRead()
        {
            LastEventTime = DateTime.MinValue;
            LastWriteTime = DateTime.MinValue;
        }

        #region Events
        #region Event
        public static event EventHandler OnEventHandler;

        protected virtual void OnEvent(OnEventArgs e)
        {
            EventHandler handler = OnEventHandler;
            handler?.Invoke(this, e);
        }

        public class OnEventArgs : System.EventArgs
        {
            public OnEventArgs(JObject @event, bool firstRun)
            {
                OnEvent = @event;
                this.FirstRun = firstRun;
            }

            /// <summary>
            /// <para>timestamp</para>
            /// <para>event</para>
            /// </summary>
            public JObject OnEvent { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region ReceiveText
        public static event EventHandler ReceiveTextHandler;

        protected virtual void OnReceiveText(ReceiveTextEventArgs e)
        {
            EventHandler handler = ReceiveTextHandler;
            handler?.Invoke(this, e);
        }

        public class ReceiveTextEventArgs : System.EventArgs
        {
            public string? Channel => ReceiveText.Value<string>("Channel");

            public string? Message => ReceiveText.Value<string>("Message");

            public string? From => ReceiveText.Value<string>("From");

            public DateTime? Timestamp => ReceiveText.Value<DateTime>("timestamp");

            public ReceiveTextEventArgs(JObject @event, bool firstRun)
            {
                ReceiveText = @event;
                this.FirstRun = firstRun;
            }

            /// <summary>
            /// <para>Tokens:</para>
            /// <para>timestamp</para>
            /// <para>event</para>
            /// <para>From</para>
            /// <para>Message</para>
            /// <para>Message_Localised</para>
            /// <para>Channel</para>
            /// </summary>
            public JObject ReceiveText { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region RefuelAll
        public static event EventHandler RefuelAllHandler;

        protected virtual void OnRefuelAll(RefuelAllEventArgs e)
        {
            EventHandler handler = RefuelAllHandler;
            handler?.Invoke(this, e);
        }

        public class RefuelAllEventArgs : System.EventArgs
        {
            public RefuelAllEventArgs(JObject @event, bool firstRun)
            {
                RefuelAll = @event;
                this.FirstRun = firstRun;
            }

            public JObject RefuelAll { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region UseConsumable
        public static event EventHandler UseConsumableHandler;

        protected virtual void OnUseConsumable(UseConsumableEventArgs e)
        {
            EventHandler handler = UseConsumableHandler;
            handler?.Invoke(this, e);
        }

        public class UseConsumableEventArgs : System.EventArgs
        {
            public UseConsumableEventArgs(JObject @event, bool firstRun)
            {
                UseConsumable = @event;
                this.FirstRun = firstRun;
            }

            public JObject UseConsumable { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region SendText
        public static event EventHandler SendTextHandler;

        protected virtual void OnSendText(SendTextEventArgs e)
        {
            EventHandler handler = SendTextHandler;
            handler?.Invoke(this, e);
        }

        public class SendTextEventArgs : System.EventArgs
        {
            public DateTime? Timestamp => SendText.Value<DateTime>("timestamp");

            /// <summary>
            /// Which channel the message was sent to.
            /// </summary>
            public string? To => SendText.Value<string>("To");
            /// <summary>
            /// Content of the message.
            /// </summary>
            public string? Message => SendText.Value<string>("Message");
            /// <summary>
            /// If message was sent.
            /// </summary>
            public bool? Sent => SendText.Value<bool>("Sent");

            public SendTextEventArgs(JObject @event, bool firstRun)
            {
                SendText = @event;
                this.FirstRun = firstRun;
            }

            /// <summary>
            /// Tokens:
            /// <para>timestamp</para>
            /// <para>event</para>
            /// <para>To (Which channel the message was sent to)</para>
            /// <para>Message (Content of message)</para>
            /// <para>Sent (If message was sent)</para>
            /// </summary>
            public JObject SendText { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Commander
        public static event EventHandler CommanderHandler;

        protected virtual void OnCommander(CommanderEventArgs e)
        {
            EventHandler handler = CommanderHandler;
            handler?.Invoke(this, e);
        }

        public class CommanderEventArgs : System.EventArgs
        {
            public CommanderEventArgs(JObject @event, bool firstRun)
            {
                Commander = @event;
                this.FirstRun = firstRun;
            }

            public JObject Commander { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region CarrierJumpCancelled
        public static event EventHandler CarrierJumpCancelledHandler;

        protected virtual void OnCarrierJumpCancelled(CarrierJumpCancelledEventArgs e)
        {
            EventHandler handler = CarrierJumpCancelledHandler;
            handler?.Invoke(this, e);
        }

        public class CarrierJumpCancelledEventArgs : System.EventArgs
        {
            public CarrierJumpCancelledEventArgs(JObject @event, bool firstRun)
            {
                CarrierJumpCancelled = @event;
                this.FirstRun = firstRun;
            }

            public JObject CarrierJumpCancelled { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region CarrierTradeOrder
        public static event EventHandler CarrierTradeOrderHandler;

        protected virtual void OnCarrierTradeOrder(CarrierTradeOrderEventArgs e)
        {
            EventHandler handler = CarrierTradeOrderHandler;
            handler?.Invoke(this, e);
        }

        public class CarrierTradeOrderEventArgs : System.EventArgs
        {
            public CarrierTradeOrderEventArgs(JObject @event, bool firstRun)
            {
                CarrierTradeOrder = @event;
                this.FirstRun = firstRun;
            }

            public JObject CarrierTradeOrder { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region CarrierJumpRequest
        public static event EventHandler CarrierJumpRequestHandler;

        protected virtual void OnCarrierJumpRequest(CarrierJumpRequestEventArgs e)
        {
            EventHandler handler = CarrierJumpRequestHandler;
            handler?.Invoke(this, e);
        }

        public class CarrierJumpRequestEventArgs : System.EventArgs
        {
            public CarrierJumpRequestEventArgs(JObject @event, bool firstRun)
            {
                CarrierJumpRequest = @event;
                this.FirstRun = firstRun;
            }

            public JObject CarrierJumpRequest { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region CarrierStats
        public static event EventHandler CarrierStatsHandler;

        protected virtual void OnCarrierStats(CarrierStatsEventArgs e)
        {
            EventHandler handler = CarrierStatsHandler;
            handler?.Invoke(this, e);
        }

        public class CarrierStatsEventArgs : System.EventArgs
        {
            public CarrierStatsEventArgs(JObject @event, bool firstRun)
            {
                CarrierStats = @event;
                this.FirstRun = firstRun;
            }

            public JObject CarrierStats { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region CarrierDepositFuel
        public static event EventHandler CarrierDepositFuelHandler;

        protected virtual void OnCarrierDepositFuel(CarrierDepositFuelEventArgs e)
        {
            EventHandler handler = CarrierDepositFuelHandler;
            handler?.Invoke(this, e);
        }

        public class CarrierDepositFuelEventArgs : System.EventArgs
        {
            public CarrierDepositFuelEventArgs(JObject @event, bool firstRun)
            {
                CarrierDepositFuel = @event;
                this.FirstRun = firstRun;
            }

            public JObject CarrierDepositFuel { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region UnknownEvent
        public static event EventHandler UnknownEventHandler;

        protected virtual void OnUnknown(UnknownEventArgs e)
        {
            EventHandler handler = UnknownEventHandler;
            handler?.Invoke(this, e);
        }

        public class UnknownEventArgs : System.EventArgs
        {
            public UnknownEventArgs(JObject @event, bool firstRun)
            {
                UnknownEvent = @event;
                this.FirstRun = firstRun;
            }

            public JObject UnknownEvent { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region CockpitBreached
        public static event EventHandler CockpitBreachedHandler;

        protected virtual void OnCockpitBreached(CockpitBreachedEventArgs e)
        {
            EventHandler handler = CockpitBreachedHandler;
            handler?.Invoke(this, e);
        }

        public class CockpitBreachedEventArgs : System.EventArgs
        {
            public CockpitBreachedEventArgs(JObject @event, bool firstRun)
            {
                CockpitBreached = @event;
                this.FirstRun = firstRun;
            }

            public JObject CockpitBreached { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region BuyDrones
        public static event EventHandler BuyDronesHandler;

        protected virtual void OnBuyDrones(BuyDronesEventArgs e)
        {
            EventHandler handler = BuyDronesHandler;
            handler?.Invoke(this, e);
        }

        public class BuyDronesEventArgs : System.EventArgs
        {
            public BuyDronesEventArgs(JObject @event, bool firstRun)
            {
                BuyDrones = @event;
                this.FirstRun = firstRun;
            }

            public JObject BuyDrones { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region SellDrones
        public static event EventHandler SellDronesHandler;

        protected virtual void OnSellDrones(SellDronesEventArgs e)
        {
            EventHandler handler = SellDronesHandler;
            handler?.Invoke(this, e);
        }

        public class SellDronesEventArgs : System.EventArgs
        {
            public SellDronesEventArgs(JObject @event, bool firstRun)
            {
                SellDrones = @event;
                this.FirstRun = firstRun;
            }

            public JObject SellDrones { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region RepairDrone
        public static event EventHandler RepairDroneHandler;

        protected virtual void OnRepairDrone(RepairDroneEventArgs e)
        {
            EventHandler handler = RepairDroneHandler;
            handler?.Invoke(this, e);
        }

        public class RepairDroneEventArgs : System.EventArgs
        {
            public RepairDroneEventArgs(JObject @event, bool firstRun)
            {
                RepairDrone = @event;
                this.FirstRun = firstRun;
            }

            public JObject RepairDrone { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region WingJoin
        public static event EventHandler WingJoinHandler;

        protected virtual void OnWingJoin(WingJoinEventArgs e)
        {
            EventHandler handler = WingJoinHandler;
            handler?.Invoke(this, e);
        }

        public class WingJoinEventArgs : System.EventArgs
        {
            public WingJoinEventArgs(JObject @event, bool firstRun)
            {
                WingJoin = @event;
                this.FirstRun = firstRun;
            }

            public JObject WingJoin { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region WingLeave
        public static event EventHandler WingLeaveHandler;

        protected virtual void OnWingLeave(WingLeaveEventArgs e)
        {
            EventHandler handler = WingLeaveHandler;
            handler?.Invoke(this, e);
        }

        public class WingLeaveEventArgs : System.EventArgs
        {
            public WingLeaveEventArgs(JObject @event, bool firstRun)
            {
                WingLeave = @event;
                this.FirstRun = firstRun;
            }

            public JObject WingLeave { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region WingAdd
        public static event EventHandler WingAddHandler;

        protected virtual void OnWingAdd(WingAddEventArgs e)
        {
            EventHandler handler = WingAddHandler;
            handler?.Invoke(this, e);
        }

        public class WingAddEventArgs : System.EventArgs
        {
            public WingAddEventArgs(JObject @event, bool firstRun)
            {
                WingAdd = @event;
                this.FirstRun = firstRun;
            }

            public JObject WingAdd { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region WingInvite
        public static event EventHandler WingInviteHandler;

        protected virtual void OnWingInvite(WingInviteEventArgs e)
        {
            EventHandler handler = WingInviteHandler;
            handler?.Invoke(this, e);
        }

        public class WingInviteEventArgs : System.EventArgs
        {
            public WingInviteEventArgs(JObject @event, bool firstRun)
            {
                WingInvite = @event;
                this.FirstRun = firstRun;
            }

            public JObject WingInvite { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region JetConeBoost
        public static event EventHandler JetConeBoostHandler;

        protected virtual void OnJetConeBoost(JetConeBoostEventArgs e)
        {
            EventHandler handler = JetConeBoostHandler;
            handler?.Invoke(this, e);
        }

        public class JetConeBoostEventArgs : System.EventArgs
        {
            public JetConeBoostEventArgs(JObject @event, bool firstRun)
            {
                JetConeBoost = @event;
                this.FirstRun = firstRun;
            }

            public JObject JetConeBoost { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region ScanBaryCentre
        public static event EventHandler ScanBaryCentreHandler;

        protected virtual void OnScanBaryCentre(ScanBaryCentreEventArgs e)
        {
            EventHandler handler = ScanBaryCentreHandler;
            handler?.Invoke(this, e);
        }

        public class ScanBaryCentreEventArgs : System.EventArgs
        {
            public ScanBaryCentreEventArgs(JObject @event, bool firstRun)
            {
                ScanBaryCentre = @event;
                this.FirstRun = firstRun;
            }

            public JObject ScanBaryCentre { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region ShipTargeted
        public static event EventHandler ShipTargetedHandler;

        protected virtual void OnShipTargeted(ShipTargetedEventArgs e)
        {
            EventHandler handler = ShipTargetedHandler;
            handler?.Invoke(this, e);
        }

        public class ShipTargetedEventArgs : System.EventArgs
        {
            public ShipTargetedEventArgs(JObject @event, bool firstRun)
            {
                ShipTargeted = @event;
                this.FirstRun = firstRun;
            }

            public JObject ShipTargeted { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region FuelScoop
        public static event EventHandler FuelScoopHandler;

        protected virtual void OnFuelScoop(FuelScoopEventArgs e)
        {
            EventHandler handler = FuelScoopHandler;
            handler?.Invoke(this, e);
        }

        public class FuelScoopEventArgs : System.EventArgs
        {
            public FuelScoopEventArgs(JObject @event, bool firstRun)
            {
                FuelScoop = @event;
                this.FirstRun = firstRun;
            }

            public JObject FuelScoop { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Scan
        public static event EventHandler ScanHandler;

        protected virtual void OnScan(ScanEventArgs e)
        {
            EventHandler handler = ScanHandler;
            handler?.Invoke(this, e);
        }

        public class ScanEventArgs : System.EventArgs
        {
            public ScanEventArgs(JObject @event, bool firstRun)
            {
                Scan = @event;
                this.FirstRun = firstRun;
            }

            public JObject Scan { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Location
        public static event EventHandler LocationHandler;

        protected virtual void OnLocation(LocationEventArgs e)
        {
            EventHandler handler = LocationHandler;
            handler?.Invoke(this, e);
        }

        public class LocationEventArgs : System.EventArgs
        {
            public LocationEventArgs(JObject @event, bool firstRun)
            {
                Location = @event;
                this.FirstRun = firstRun;
            }

            public JObject Location { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region LoadGame
        /// <summary>
        /// Tokens:
        /// <para>timestamp</para>
        /// <para>event</para>
        /// <para>FID</para>
        /// <para>Commander</para>
        /// <para>Horizons</para>
        /// <para>Odyssey</para>
        /// <para>Ship</para>
        /// <para>ShipID</para>
        /// <para>ShipName</para>
        /// <para>ShipIdent</para>
        /// <para>FuelLevel</para>
        /// <para>FuelCapacity</para>
        /// <para>GameMode</para>
        /// <para>Credits</para>
        /// <para>Loan</para>
        /// <para>language</para>
        /// <para>gameversion</para>
        /// <para>build</para>
        /// </summary>
        public static event EventHandler LoadGameHandler;

        protected virtual void OnLoadGame(LoadGameEventArgs e)
        {
            EventHandler handler = LoadGameHandler;
            handler?.Invoke(this, e);
        }

        public class LoadGameEventArgs : System.EventArgs
        {
            public LoadGameEventArgs(JObject @event, bool firstRun)
            {
                LoadGame = @event;
                this.FirstRun = firstRun;
            }

            /// <summary>
            /// Tokens:
            /// <para>timestamp</para>
            /// <para>event</para>
            /// <para>FID</para>
            /// <para>Commander</para>
            /// <para>Horizons</para>
            /// <para>Odyssey</para>
            /// <para>Ship</para>
            /// <para>ShipID</para>
            /// <para>ShipName</para>
            /// <para>ShipIdent</para>
            /// <para>FuelLevel</para>
            /// <para>FuelCapacity</para>
            /// <para>GameMode</para>
            /// <para>Credits</para>
            /// <para>Loan</para>
            /// <para>language</para>
            /// <para>gameversion</para>
            /// <para>build</para>
            /// </summary>
            public JObject LoadGame { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region ReservoirReplenished
        public static event EventHandler ReservoirReplenishedHandler;

        protected virtual void OnReservoirReplenished(ReservoirReplenishedEventArgs e)
        {
            EventHandler handler = ReservoirReplenishedHandler;
            handler?.Invoke(this, e);
        }

        public class ReservoirReplenishedEventArgs : System.EventArgs
        {
            public ReservoirReplenishedEventArgs(JObject @event, bool firstRun)
            {
                ReservoirReplenished = @event;
                this.FirstRun = firstRun;
            }

            public JObject ReservoirReplenished { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Docked
        public static event EventHandler DockedHandler;

        protected virtual void OnDocked(DockedEventArgs e)
        {
            EventHandler handler = DockedHandler;
            handler?.Invoke(this, e);
        }

        public class DockedEventArgs : System.EventArgs
        {
            public DockedEventArgs(JObject @event, bool firstRun)
            {
                Docked = @event;
                this.FirstRun = firstRun;
            }

            public JObject Docked { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region ApproachBody
        public static event EventHandler ApproachBodyHandler;

        protected virtual void OnApproachBody(ApproachBodyEventArgs e)
        {
            EventHandler handler = ApproachBodyHandler;
            handler?.Invoke(this, e);
        }

        public class ApproachBodyEventArgs : System.EventArgs
        {
            public ApproachBodyEventArgs(JObject @event, bool firstRun)
            {
                ApproachBody = @event;
                this.FirstRun = firstRun;
            }

            public JObject ApproachBody { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region LeaveBody
        public static event EventHandler LeaveBodyHandler;

        protected virtual void OnLeaveBody(LeaveBodyEventArgs e)
        {
            EventHandler handler = LeaveBodyHandler;
            handler?.Invoke(this, e);
        }

        public class LeaveBodyEventArgs : System.EventArgs
        {
            public LeaveBodyEventArgs(JObject @event, bool firstRun)
            {
                LeaveBody = @event;
                this.FirstRun = firstRun;
            }

            public JObject LeaveBody { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Friends
        public static event EventHandler FriendsHandler;

        protected virtual void OnFriends(FriendsEventArgs e)
        {
            EventHandler handler = FriendsHandler;
            handler?.Invoke(this, e);
        }

        public class FriendsEventArgs : System.EventArgs
        {
            public FriendsEventArgs(JObject @event, bool firstRun)
            {
                Friends = @event;
                this.FirstRun = firstRun;
            }

            public JObject Friends { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region SupercruiseExit
        public static event EventHandler SupercruiseExitHandler;

        protected virtual void OnSupercruiseExit(SupercruiseExitEventArgs e)
        {
            EventHandler handler = SupercruiseExitHandler;
            handler?.Invoke(this, e);
        }

        public class SupercruiseExitEventArgs : System.EventArgs
        {
            public SupercruiseExitEventArgs(JObject @event, bool firstRun)
            {
                SupercruiseExit = @event;
                this.FirstRun = firstRun;
            }

            public JObject SupercruiseExit { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region FSDJump
        public static event EventHandler FSDJumpHandler;

        protected virtual void OnFSDJump(FSDJumpEventArgs e)
        {
            EventHandler handler = FSDJumpHandler;
            handler?.Invoke(this, e);
        }

        public class FSDJumpEventArgs : System.EventArgs
        {
            public FSDJumpEventArgs(JObject @event, bool firstRun)
            {
                FSDJump = @event;
                this.FirstRun = firstRun;
            }

            public JObject FSDJump { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region SupercruiseEntry
        public static event EventHandler SupercruiseEntryHandler;

        protected virtual void OnSupercruiseEntry(SupercruiseEntryEventArgs e)
        {
            EventHandler handler = SupercruiseEntryHandler;
            handler?.Invoke(this, e);
        }

        public class SupercruiseEntryEventArgs : System.EventArgs
        {
            public SupercruiseEntryEventArgs(JObject @event, bool firstRun)
            {
                SupercruiseEntry = @event;
                this.FirstRun = firstRun;
            }

            public JObject SupercruiseEntry { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Loadout
        public static event EventHandler LoadoutHandler;

        protected virtual void OnLoadout(LoadoutEventArgs e)
        {
            EventHandler handler = LoadoutHandler;
            handler?.Invoke(this, e);
        }

        public class LoadoutEventArgs : System.EventArgs
        {
            public LoadoutEventArgs(JObject @event, bool firstRun)
            {
                Loadout = @event;
                this.FirstRun = firstRun;
            }

            public JObject Loadout { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region CarrierJump
        public static event EventHandler CarrierJumpHandler;

        protected virtual void OnCarrierJump(CarrierJumpEventArgs e)
        {
            EventHandler handler = CarrierJumpHandler;
            handler?.Invoke(this, e);
        }

        public class CarrierJumpEventArgs : System.EventArgs
        {
            public CarrierJumpEventArgs(JObject @event, bool firstRun)
            {
                CarrierJump = @event;
                this.FirstRun = firstRun;
            }

            public JObject CarrierJump { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region StoredModules
        public static event EventHandler StoredModulesHandler;

        protected virtual void OnStoredModules(StoredModulesEventArgs e)
        {
            EventHandler handler = StoredModulesHandler;
            handler?.Invoke(this, e);
        }

        public class StoredModulesEventArgs : System.EventArgs
        {
            public StoredModulesEventArgs(JObject @event, bool firstRun)
            {
                StoredModules = @event;
                this.FirstRun = firstRun;
            }

            public JObject StoredModules { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Market
        public static event EventHandler MarketHandler;

        protected virtual void OnMarket(MarketEventArgs e)
        {
            EventHandler handler = MarketHandler;
            handler?.Invoke(this, e);
        }

        public class MarketEventArgs : System.EventArgs
        {
            public MarketEventArgs(JObject @event, bool firstRun)
            {
                Market = @event;
                this.FirstRun = firstRun;
            }

            public JObject Market { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Shipyard
        public static event EventHandler ShipyardHandler;

        protected virtual void OnShipyard(ShipyardEventArgs e)
        {
            EventHandler handler = ShipyardHandler;
            handler?.Invoke(this, e);
        }

        public class ShipyardEventArgs : System.EventArgs
        {
            public ShipyardEventArgs(JObject @event, bool firstRun)
            {
                Shipyard = @event;
                this.FirstRun = firstRun;
            }

            public JObject Shipyard { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Outfitting
        public static event EventHandler OutfittingHandler;

        protected virtual void OnOutfitting(OutfittingEventArgs e)
        {
            EventHandler handler = OutfittingHandler;
            handler?.Invoke(this, e);
        }

        public class OutfittingEventArgs : System.EventArgs
        {
            public OutfittingEventArgs(JObject @event, bool firstRun)
            {
                Outfitting = @event;
                this.FirstRun = firstRun;
            }

            public JObject Outfitting { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region StoredShips
        public static event EventHandler StoredShipsHandler;

        protected virtual void OnStoredShips(StoredShipsEventArgs e)
        {
            EventHandler handler = StoredShipsHandler;
            handler?.Invoke(this, e);
        }

        public class StoredShipsEventArgs : System.EventArgs
        {
            public StoredShipsEventArgs(JObject @event, bool firstRun)
            {
                StoredShips = @event;
                this.FirstRun = firstRun;
            }

            public JObject StoredShips { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Embark
        public static event EventHandler EmbarkHandler;

        protected virtual void OnEmbark(EmbarkEventArgs e)
        {
            EventHandler handler = EmbarkHandler;
            handler?.Invoke(this, e);
        }

        public class EmbarkEventArgs : System.EventArgs
        {
            public EmbarkEventArgs(JObject @event, bool firstRun)
            {
                Embark = @event;
                this.FirstRun = firstRun;
            }

            public JObject Embark { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Disembark
        public static event EventHandler DisembarkHandler;

        protected virtual void OnDisembark(DisembarkEventArgs e)
        {
            EventHandler handler = DisembarkHandler;
            handler?.Invoke(this, e);
        }

        public class DisembarkEventArgs : System.EventArgs
        {
            public DisembarkEventArgs(JObject @event, bool firstRun)
            {
                Disembark = @event;
                this.FirstRun = firstRun;
            }

            public JObject Disembark { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Liftoff
        public static event EventHandler LiftoffHandler;

        protected virtual void OnLiftoff(LiftoffEventArgs e)
        {
            EventHandler handler = LiftoffHandler;
            handler?.Invoke(this, e);
        }

        public class LiftoffEventArgs : System.EventArgs
        {
            public LiftoffEventArgs(JObject @event, bool firstRun)
            {
                Liftoff = @event;
                this.FirstRun = firstRun;
            }

            public JObject Liftoff { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #region Touchdown
        public static event EventHandler TouchdownHandler;

        protected virtual void OnTouchdown(TouchdownEventArgs e)
        {
            EventHandler handler = TouchdownHandler;
            handler?.Invoke(this, e);
        }

        public class TouchdownEventArgs : System.EventArgs
        {
            public TouchdownEventArgs(JObject @event, bool firstRun)
            {
                Touchdown = @event;
                this.FirstRun = firstRun;
            }

            public JObject Touchdown { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion
        #endregion
    }
}
