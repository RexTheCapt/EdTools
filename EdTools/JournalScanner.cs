using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdTools
{
    internal class JournalScanner
    {
        internal DateTime LastEventTime { get; private set; }
        internal DateTime LastWriteTime { get; private set; }
        internal bool FirstRun { get => _firstRun; private set { _firstRun = value; } }
        private bool _firstRun = true;

        internal void TimerScan(object? sender, EventArgs e)
        {
            string newest = "";
            DateTime currentWriteTime = DateTime.MinValue;

            foreach (string f in Directory.GetFiles(FormMain.JournalPath, "Journal.*.log"))
            {
                FileInfo fi = new FileInfo(f);

                if (fi.LastWriteTime > currentWriteTime)
                {
                    currentWriteTime = fi.LastWriteTime;
                    newest = f;
                }
                //}

                //if (LastWriteTime != currentWriteTime)
                {
                    var fs = new FileStream(newest, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using (var reader = new StreamReader(fs))
                        while (!reader.EndOfStream)
                        {
                            JObject? @event = (JObject)JsonConvert.DeserializeObject(reader.ReadLine());
                            if (@event != null)
                            {
                                DateTime currentEventDateTime = @event.Value<DateTime>("timestamp");
                                if (currentEventDateTime > LastEventTime)
                                {
                                    string eventType = @event.Value<string>("event");
#if DEBUG
                                    #region Debug stuff
                                    if (@event.Value<string>("StarSystem") != null && eventType != "Scan" && eventType != "StartJump" && eventType != "FSDJump" && eventType != "SupercruiseExit" && eventType != "SupercruiseEntry" && eventType != "ApproachBody" && eventType != "Touchdown" && eventType != "Liftoff" && eventType != "Disembark" && eventType != "Embark" && eventType != "LeaveBody" && eventType != "Location" && eventType != "Docked" && eventType != "StoredShips" && eventType != "Outfitting" && eventType != "Shipyard" && eventType != "ScanBaryCentre" && eventType != "Market" && eventType != "StoredModules" && eventType != "CarrierJump")
                                    {
                                        System.Windows.Forms.Clipboard.SetText(eventType);
                                        Console.WriteLine("HasStarSystem");
                                    }
                                    #endregion
#endif

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
                                        #region unused events
                                        case "UseConsumable":
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
                                        case "RefuelAll":
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
                                        case "SendText":
                                        case "FSDTarget":
                                        case "ModuleInfo":
                                        case "Progress":
                                        case "Commander":
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
                                        case "ReceiveText":
                                        case "Music":
                                        case "DockingRequested":
                                        case "HullDamage":
                                        case "StartJump":
                                            break;
                                        #endregion
                                        default:
                                            Console.WriteLine($"Unknown event: {eventType}");
                                            break;
                                    }
                                    LastEventTime = currentEventDateTime;
                                }
                            }
                        }

                    LastWriteTime = currentWriteTime;
                }

                FirstRun = false;
            }
        }

        #region Events

        #region CarrierJump
        internal static event EventHandler CarrierJumpHandler;

        protected virtual void OnCarrierJump(CarrierJumpEventArgs e)
        {
            EventHandler handler = CarrierJumpHandler;
            handler?.Invoke(this, e);
        }

        internal class CarrierJumpEventArgs : EventArgs
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
        internal static event EventHandler StoredModulesHandler;

        protected virtual void OnStoredModules(StoredModulesEventArgs e)
        {
            EventHandler handler = StoredModulesHandler;
            handler?.Invoke(this, e);
        }

        internal class StoredModulesEventArgs : EventArgs
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
        internal static event EventHandler MarketHandler;

        protected virtual void OnMarket(MarketEventArgs e)
        {
            EventHandler handler = MarketHandler;
            handler?.Invoke(this, e);
        }

        internal class MarketEventArgs : EventArgs
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
        internal static event EventHandler ShipyardHandler;

        protected virtual void OnShipyard(ShipyardEventArgs e)
        {
            EventHandler handler = ShipyardHandler;
            handler?.Invoke(this, e);
        }

        internal class ShipyardEventArgs : EventArgs
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
        internal static event EventHandler OutfittingHandler;

        protected virtual void OnOutfitting(OutfittingEventArgs e)
        {
            EventHandler handler = OutfittingHandler;
            handler?.Invoke(this, e);
        }

        internal class OutfittingEventArgs : EventArgs
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
        internal static event EventHandler StoredShipsHandler;

        protected virtual void OnStoredShips(StoredShipsEventArgs e)
        {
            EventHandler handler = StoredShipsHandler;
            handler?.Invoke(this, e);
        }

        internal class StoredShipsEventArgs : EventArgs
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
        internal static event EventHandler EmbarkHandler;

        protected virtual void OnEmbark(EmbarkEventArgs e)
        {
            EventHandler handler = EmbarkHandler;
            handler?.Invoke(this, e);
        }

        internal class EmbarkEventArgs : EventArgs
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
        internal static event EventHandler DisembarkHandler;

        protected virtual void OnDisembark(DisembarkEventArgs e)
        {
            EventHandler handler = DisembarkHandler;
            handler?.Invoke(this, e);
        }

        internal class DisembarkEventArgs : EventArgs
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
        internal static event EventHandler LiftoffHandler;

        protected virtual void OnLiftoff(LiftoffEventArgs e)
        {
            EventHandler handler = LiftoffHandler;
            handler?.Invoke(this, e);
        }

        internal class LiftoffEventArgs : EventArgs
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
        internal static event EventHandler TouchdownHandler;

        protected virtual void OnTouchdown(TouchdownEventArgs e)
        {
            EventHandler handler = TouchdownHandler;
            handler?.Invoke(this, e);
        }

        internal class TouchdownEventArgs : EventArgs
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

        #region Loadout
        internal static event EventHandler LoadoutHandler;

        protected virtual void OnLoadout(LoadoutEventArgs e)
        {
            EventHandler handler = LoadoutHandler;
            handler?.Invoke(this, e);
        }

        internal class LoadoutEventArgs : EventArgs
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

        #region SupercruiseEntry
        internal static event EventHandler SupercruiseEntryHandler;

        protected virtual void OnSupercruiseEntry(SupercruiseEntryEventArgs e)
        {
            EventHandler handler = SupercruiseEntryHandler;
            handler?.Invoke(this, e);
        }

        internal class SupercruiseEntryEventArgs : EventArgs
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

        #region FSDJump
        internal static event EventHandler FSDJumpHandler;

        protected virtual void OnFSDJump(FSDJumpEventArgs e)
        {
            EventHandler handler = FSDJumpHandler;
            handler?.Invoke(this, e);
        }

        internal class FSDJumpEventArgs : EventArgs
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

        #region SupercruiseExit
        internal static event EventHandler SupercruiseExitHandler;

        protected virtual void OnSupercruiseExit(SupercruiseExitEventArgs e)
        {
            EventHandler handler = SupercruiseExitHandler;
            handler?.Invoke(this, e);
        }

        internal class SupercruiseExitEventArgs : EventArgs
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

        #region Friends
        internal static event EventHandler FriendsHandler;

        protected virtual void OnFriends(FriendsEventArgs e)
        {
            EventHandler handler = FriendsHandler;
            handler?.Invoke(this, e);
        }

        internal class FriendsEventArgs : EventArgs
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

        #region LeaveBody
        internal static event EventHandler LeaveBodyHandler;

        protected virtual void OnLeaveBody(LeaveBodyEventArgs e)
        {
            EventHandler handler = LeaveBodyHandler;
            handler?.Invoke(this, e);
        }

        internal class LeaveBodyEventArgs : EventArgs
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

        #region ApproachBody
        internal static event EventHandler ApproachBodyHandler;

        protected virtual void OnApproachBody(ApproachBodyEventArgs e)
        {
            EventHandler handler = ApproachBodyHandler;
            handler?.Invoke(this, e);
        }

        internal class ApproachBodyEventArgs : EventArgs
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

        #region Docked
        internal static event EventHandler DockedHandler;

        protected virtual void OnDocked(DockedEventArgs e)
        {
            EventHandler handler = DockedHandler;
            handler?.Invoke(this, e);
        }

        internal class DockedEventArgs : EventArgs
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

        #region LoadGame
        internal static event EventHandler LoadGameHandler;

        protected virtual void OnLoadGame(LoadGameEventArgs e)
        {
            EventHandler handler = LoadGameHandler;
            handler?.Invoke(this, e);
        }

        internal class LoadGameEventArgs : EventArgs
        {
            public LoadGameEventArgs(JObject @event, bool firstRun)
            {
                LoadGame = @event;
                this.FirstRun = firstRun;
            }

            public JObject LoadGame { get; private set; }
            public bool FirstRun { get; private set; }
        }
        #endregion

        #region ReservoirReplenished
        internal static event EventHandler ReservoirReplenishedHandler;

        protected virtual void OnReservoirReplenished(ReservoirReplenishedEventArgs e)
        {
            EventHandler handler = ReservoirReplenishedHandler;
            handler?.Invoke(this, e);
        }

        internal class ReservoirReplenishedEventArgs : EventArgs
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

        #region Location
        internal static event EventHandler LocationHandler;

        protected virtual void OnLocation(LocationEventArgs e)
        {
            EventHandler handler = LocationHandler;
            handler?.Invoke(this, e);
        }

        internal class LocationEventArgs : EventArgs
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

        #region Scan
        internal static event EventHandler ScanHandler;

        protected virtual void OnScan(ScanEventArgs e)
        {
            EventHandler handler = ScanHandler;
            handler?.Invoke(this, e);
        }

        internal class ScanEventArgs : EventArgs
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

        #region FuelScoop
        internal static event EventHandler FuelScoopHandler;

        protected virtual void OnFuelScoop(FuelScoopEventArgs e)
        {
            EventHandler handler = FuelScoopHandler;
            handler?.Invoke(this, e);
        }

        internal class FuelScoopEventArgs : EventArgs
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

        #region ShipTargeted
        internal static event EventHandler ShipTargetedHandler;

        protected virtual void OnShipTargeted(ShipTargetedEventArgs e)
        {
            EventHandler handler = ShipTargetedHandler;
            handler?.Invoke(this, e);
        }

        internal class ShipTargetedEventArgs : EventArgs
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

        #region ScanBaryCentre
        internal static event EventHandler ScanBaryCentreHandler;

        protected virtual void OnScanBaryCentre(ScanBaryCentreEventArgs e)
        {
            EventHandler handler = ScanBaryCentreHandler;
            handler?.Invoke(this, e);
        }

        internal class ScanBaryCentreEventArgs : EventArgs
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

        #region JetConeBoost
        internal static event EventHandler JetConeBoostHandler;

        protected virtual void OnJetConeBoost(JetConeBoostEventArgs e)
        {
            EventHandler handler = JetConeBoostHandler;
            handler?.Invoke(this, e);
        }

        internal class JetConeBoostEventArgs : EventArgs
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

        #region WingInvite
        internal static event EventHandler WingInviteHandler;

        protected virtual void OnWingInvite(WingInviteEventArgs e)
        {
            EventHandler handler = WingInviteHandler;
            handler?.Invoke(this, e);
        }

        internal class WingInviteEventArgs : EventArgs
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

        #region WingAdd
        internal static event EventHandler WingAddHandler;

        protected virtual void OnWingAdd(WingAddEventArgs e)
        {
            EventHandler handler = WingAddHandler;
            handler?.Invoke(this, e);
        }

        internal class WingAddEventArgs : EventArgs
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

        #region WingLeave
        internal static event EventHandler WingLeaveHandler;

        protected virtual void OnWingLeave(WingLeaveEventArgs e)
        {
            EventHandler handler = WingLeaveHandler;
            handler?.Invoke(this, e);
        }

        internal class WingLeaveEventArgs : EventArgs
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

        #region WingJoin
        internal static event EventHandler WingJoinHandler;

        protected virtual void OnWingJoin(WingJoinEventArgs e)
        {
            EventHandler handler = WingJoinHandler;
            handler?.Invoke(this, e);
        }

        internal class WingJoinEventArgs : EventArgs
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

        #region RepairDrone
        internal static event EventHandler RepairDroneHandler;

        protected virtual void OnRepairDrone(RepairDroneEventArgs e)
        {
            EventHandler handler = RepairDroneHandler;
            handler?.Invoke(this, e);
        }

        internal class RepairDroneEventArgs : EventArgs
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

        #region SellDrones
        internal static event EventHandler SellDronesHandler;

        protected virtual void OnSellDrones(SellDronesEventArgs e)
        {
            EventHandler handler = SellDronesHandler;
            handler?.Invoke(this, e);
        }

        internal class SellDronesEventArgs : EventArgs
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

        #region BuyDrones
        internal static event EventHandler BuyDronesHandler;

        protected virtual void OnBuyDrones(BuyDronesEventArgs e)
        {
            EventHandler handler = BuyDronesHandler;
            handler?.Invoke(this, e);
        }

        internal class BuyDronesEventArgs : EventArgs
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

        #region CockpitBreached
        internal static event EventHandler CockpitBreachedHandler;

        protected virtual void OnCockpitBreached(CockpitBreachedEventArgs e)
        {
            EventHandler handler = CockpitBreachedHandler;
            handler?.Invoke(this, e);
        }

        internal class CockpitBreachedEventArgs : EventArgs
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
        #endregion
    }
}
