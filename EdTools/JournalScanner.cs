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
    internal static class JournalScanner
    {
        public static DateTime LastEventTime { get; private set; }
        public static DateTime LastWriteTime { get; private set; }

        internal static void TimerScan(object? sender, EventArgs e)
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
            }

            if (LastWriteTime != currentWriteTime)
            {
                using (var reader = new StreamReader(newest))
                    while (!reader.EndOfStream)
                    {
                        JObject? j = (JObject)JsonConvert.DeserializeObject(reader.ReadLine());
                        if (j != null)
                        {
                            if (j.Value<DateTime>("timestamp") > LastEventTime)
                            {
                                string eventType = j.Value<string>("event");
                                switch (eventType)
                                {
                                    case "Loadout":
                                    case "SupercruiseEntry":
                                    case "FSDJump":
                                    case "SupercruiseExit":
                                    case "Friends":
                                    case "LeaveBody":
                                    case "ApproachBody":
                                    case "Docked":
                                    case "ReservoirReplenished":
                                    case "LoadGame":
                                    case "Location":
                                    case "Scan":
                                    case "FuelScoop":
                                    case "ShipTargeted":
                                    case "ScanBaryCentre":
                                    case "JetConeBoost":
                                    case "WingInvite":
                                    case "WingAdd":
                                    case "WingLeave":
                                    case "WingJoin":
                                    case "RepairDrone":
                                    case "SellDrones":
                                    case "BuyDrones":
                                    case "CockpitBreached":
                                        break;
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
                                    case "Embark":
                                    case "CrimeVictim":
                                    case "MaterialDiscovered":
                                    case "EscapeInterdiction":
                                    case "SwitchSuitLoadout":
                                    case "CrewAssign":
                                    case "Disembark":
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
                                    case "CarrierJump":
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
                                    case "Touchdown":
                                    case "ModuleStore":
                                    case "SAAScanComplete":
                                    case "ModuleSellRemote":
                                    case "ModuleRetrieve":
                                    case "MissionFailed":
                                    case "MarketSell":
                                    case "Liftoff":
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
                                    case "Shipyard":
                                    case "Materials":
                                    case "MarketBuy":
                                    case "Fileheader":
                                    case "Rank":
                                    case "Shutdown":
                                    case "EngineerProgress":
                                    case "ShipyardSwap":
                                    case "CommunityGoal":
                                    case "DockingDenied":
                                    case "StoredModules":
                                    case "MissionAccepted":
                                    case "Outfitting":
                                    case "StoredShips":
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
                                    case "Market":
                                        break;
                                    default:
                                        Console.WriteLine($"Unknown event: {eventType}");
                                        break;
                                }
                            }
                        }
                    }

                LastWriteTime = currentWriteTime;
            }
        }
    }
}
