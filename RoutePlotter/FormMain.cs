using System;
using System.IO;
using System.Windows.Forms;

using EDNeutronRouterPlugin;

using Newtonsoft.Json.Linq;

using EdTools;
using System.Drawing;

namespace RoutePlotter
{
    public partial class FormMain : Form
    {
        public static readonly string JournalPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Saved Games\\Frontier Developments\\Elite Dangerous";

        private Timer _timerJournalScanner;
        private string _currentStarSystem;
        private readonly Timer _delayedUpdate;

        internal JournalScanner JournalScanner { get; private set; }
        internal string _ranUpdateInSystem = "";

        public string CurrentStarSystem
        {
            get => _currentStarSystem;
            private set
            {
                _currentStarSystem = value;
                textBoxCurrentSystem.Text = _currentStarSystem;

                if (!_currentStarSystem.Equals(value, StringComparison.OrdinalIgnoreCase))
                    textBoxCurrentSystem.BackColor = Color.FromName("Control");

                if (JournalScanner != null && !JournalScanner.FirstRun)
                {
                    UpdateTravelPath();
                }
            }
        }

        public FormMain()
        {
            InitializeComponent();

            #region Setup journal scanner
            JournalScanner = new JournalScanner(JournalPath);

            CurrentStarSystem = CurrentStarSystem = "[UNKNOWN]";
            JournalScanner.DockedHandler += JournalScanner_DockedHandler;
            JournalScanner.ScanHandler += JournalScanner_ScanHandler;
            JournalScanner.FSDJumpHandler += JournalScanner_FSDJumpHandler;
            JournalScanner.SupercruiseExitHandler += JournalScanner_SupercruiseExitHandler;
            JournalScanner.SupercruiseEntryHandler += JournalScanner_SupercruiseEntryHandler;
            JournalScanner.ApproachBodyHandler += JournalScanner_ApproachBodyHandler;
            JournalScanner.TouchdownHandler += JournalScanner_TouchdownHandler;
            JournalScanner.LiftoffHandler += JournalScanner_LiftoffHandler;
            JournalScanner.DisembarkHandler += JournalScanner_DisembarkHandler;
            JournalScanner.EmbarkHandler += JournalScanner_EmbarkHandler;
            JournalScanner.LeaveBodyHandler += JournalScanner_LeaveBodyHandler;
            JournalScanner.LocationHandler += JournalScanner_LocationHandler;
            JournalScanner.StoredShipsHandler += JournalScanner_StoredShipsHandler;
            JournalScanner.OutfittingHandler += JournalScanner_OutfittingHandler;
            JournalScanner.ShipyardHandler += JournalScanner_ShipyardHandler;
            JournalScanner.ScanBaryCentreHandler += JournalScanner_ScanBaryCentreHandler;
            JournalScanner.MarketHandler += JournalScanner_MarketHandler;
            JournalScanner.StoredModulesHandler += JournalScanner_StoredModulesHandler;
            JournalScanner.CarrierJumpHandler += JournalScanner_CarrierJumpHandler;
            #endregion

            _timerJournalScanner = new Timer();
            _timerJournalScanner.Interval = 100;
            _timerJournalScanner.Tick += JournalScanner.TimerScan;
            _timerJournalScanner.Enabled = true;

            _delayedUpdate = new Timer();
            _delayedUpdate.Tick += DelayedUpdateTick;
            _delayedUpdate.Enabled = false;
            _delayedUpdate.Interval = 1000;

            listView1.DoubleClick += ListView1_DoubleClick;
        }

        private void DelayedUpdateTick(object? sender, EventArgs e)
        {
            ButtonStartRoute_Click(sender, e);
        }

        private void ListView1_DoubleClick(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListViewItem listViewItem = listView1.SelectedItems[0];
                JObject tag = (JObject)listViewItem.Tag;
                Clipboard.SetText(tag.Value<string>("system"));
            }
        }

        private void UpdateTravelPath()
        {
            if (string.IsNullOrEmpty(textBoxTargetSystem.Text) || string.IsNullOrWhiteSpace(textBoxTargetSystem.Text) || _ranUpdateInSystem.Equals(CurrentStarSystem))
                return;

            ButtonStartRoute_Click(this, new EventArgs());

            _ranUpdateInSystem = CurrentStarSystem;
        }

        #region events
        private void JournalScanner_CarrierJumpHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.CarrierJumpEventArgs)
            {
                JObject j = ((JournalScanner.CarrierJumpEventArgs)e).CarrierJump;

                if (j != null)
                {
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_StoredModulesHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.StoredModulesEventArgs)
            {
                JObject j = ((JournalScanner.StoredModulesEventArgs)e).StoredModules;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * MarketID
                     * StationName
                     * StarSystem
                     * Items
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_MarketHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.MarketEventArgs)
            {
                JObject j = ((JournalScanner.MarketEventArgs)e).Market;

                if (j != null)
                {
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_ScanBaryCentreHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.ScanBaryCentreEventArgs)
            {
                JObject j = ((JournalScanner.ScanBaryCentreEventArgs)e).ScanBaryCentre;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * StarSystem
                     * SystemAddress
                     * BodyID
                     * SemiMajorAxis
                     * Eccentricity
                     * OrbitalInclination
                     * Periapsis
                     * OrbitalPeriod
                     * AscendingNode
                     * MeanAnomaly
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_ShipyardHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.ShipyardEventArgs)
            {
                JObject j = ((JournalScanner.ShipyardEventArgs)e).Shipyard;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * MarketID
                     * StationName
                     * StarSystem
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_OutfittingHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.OutfittingEventArgs)
            {
                JObject j = ((JournalScanner.OutfittingEventArgs)e).Outfitting;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * MarketID
                     * StationName
                     * StarSystem
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_StoredShipsHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.StoredShipsEventArgs)
            {
                JObject j = ((JournalScanner.StoredShipsEventArgs)e).StoredShips;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * StationName
                     * MarketID
                     * StarSystem
                     * ShipsHere
                     * ShipsRemote
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_LocationHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.LocationEventArgs)
            {
                JObject j = ((JournalScanner.LocationEventArgs)e).Location;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * Docked
                     * StarSystem
                     * SystemAddress
                     * StarPos
                     * SystemAllegiance
                     * SystemEconomy
                     * SystemEconomy_Localised
                     * SystemSecondEconomy
                     * SystemSecondEconomy_Localised
                     * SystemGovernment
                     * SystemGovernment_Localised
                     * SystemSecurity
                     * SystemSecurity_Localised
                     * Population
                     * Body
                     * BodyID
                     * BodyType
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_LeaveBodyHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.LeaveBodyEventArgs)
            {
                JObject j = ((JournalScanner.LeaveBodyEventArgs)e).LeaveBody;

                if (j != null)
                {
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_EmbarkHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.EmbarkEventArgs)
            {
                JObject j = ((JournalScanner.EmbarkEventArgs)e).Embark;

                if (j != null)
                {
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_DisembarkHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.DisembarkEventArgs)
            {
                JObject j = ((JournalScanner.DisembarkEventArgs)e).Disembark;

                if (j != null)
                {
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_LiftoffHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.LiftoffEventArgs)
            {
                JObject j = ((JournalScanner.LiftoffEventArgs)e).Liftoff;

                if (j != null)
                {
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_TouchdownHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.TouchdownEventArgs)
            {
                JObject j = ((JournalScanner.TouchdownEventArgs)e).Touchdown;

                if (j != null)
                {
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_ApproachBodyHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.ApproachBodyEventArgs)
            {
                JObject j = ((JournalScanner.ApproachBodyEventArgs)e).ApproachBody;

                if (j != null)
                {
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_SupercruiseEntryHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.SupercruiseEntryEventArgs)
            {
                JObject j = ((JournalScanner.SupercruiseEntryEventArgs)e).SupercruiseEntry;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * StarSystem
                     * SystemAddress
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_SupercruiseExitHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.SupercruiseExitEventArgs)
            {
                JObject j = ((JournalScanner.SupercruiseExitEventArgs)e).SupercruiseExit;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * Taxi
                     * Multicrew
                     * StarSystem
                     * SystemAddress
                     * Body
                     * BodyID
                     * BodyType
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_FSDJumpHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.FSDJumpEventArgs)
            {
                JObject j = ((JournalScanner.FSDJumpEventArgs)e).FSDJump;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * Taxi
                     * Multicrew
                     * StarSystem
                     * SystemAddress
                     * StarPos
                     * SystemAllegiance
                     * SystemEconomy
                     * SystemEconomy_Localised
                     * SystemGovernment
                     * SystemGovernment_Localised
                     * SystemSecurity
                     * SystemSecurity_Localised
                     * Population
                     * Body
                     * BodyID
                     * BodyType
                     * JumpDist
                     * FuelUsed
                     * FuelLevel
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_ScanHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.ScanEventArgs)
            {
                JObject j = ((JournalScanner.ScanEventArgs)e).Scan;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * ScanType
                     * BodyName
                     * BodyID
                     * Parents
                     * StarSystem
                     * SystemAddress
                     * DistanceFromArrivalLS
                     * WasDiscovered
                     * WasMapped
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }

        private void JournalScanner_DockedHandler(object? sender, EventArgs e)
        {
            if (e is JournalScanner.DockedEventArgs)
            {
                JObject j = ((JournalScanner.DockedEventArgs)e).Docked;

                if (j != null)
                {
                    /*
                     * timestamp
                     * event
                     * StationName
                     * StationType
                     * Taxi
                     * Multicrew
                     * StarSystem
                     * SystemAddress
                     * MarketID
                     * StationFaction
                     * StationGovernment
                     * StationGovernment_Localised
                     * StationAllegiance
                     * StationServices
                     * StationEconomy
                     * StationEconomy_Localised
                     * StationEconomies
                     * DistFromStarLS
                     * LandingPads
                     */
                    CurrentStarSystem = j.Value<string>("StarSystem") ?? "[UNKNOWN]";
                }
            }
        }
        #endregion

        private void ButtonStartRoute_Click(object sender, EventArgs e)
        {
            _delayedUpdate.Enabled = false;
            JToken jToken;
            try
            {
                jToken = NeutronRouterAPI.GetNewRoute(textBoxCurrentSystem.Text, textBoxTargetSystem.Text, numericUpDownJumpRange.Value, 60);

                if (textBoxCurrentSystem.BackColor == Color.Red)
                    textBoxCurrentSystem.BackColor = Color.Orange;
            }
            catch (Exception ex)
            {
                Type t = ex.GetType();

                if (t == typeof(InvalidEndSystemException))
                {
                    MessageBox.Show("Invalid system", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (t == typeof(InvalidStartSystemException))
                {
                    textBoxCurrentSystem.BackColor = Color.Red;
                    _delayedUpdate.Enabled = true;
                    return;
                }
                else
                    throw;
            }

            listView1.Items.Clear();
            bool skipFirst = true;
            bool copySecond = true;
            uint totalJumps = 0;
            foreach (JObject j in jToken.Value<JArray>("system_jumps"))
            {
                totalJumps += j.Value<uint>("jumps");

                if (skipFirst)
                {
                    skipFirst = false;
                    continue;
                }

                string[] subItems = new string[]
                {
                    j.Value<string>("system"),
                    MakeNumberSmaller(j.Value<decimal>("distance_jumped")),
                    MakeNumberSmaller(j.Value<decimal>("distance_left")),
                    j.Value<string>("jumps"),
                    j.Value<string>("neutron_star")
                };

                if (copySecond)
                {
                    Clipboard.SetText(subItems[0]);
                    copySecond = false;
                }

                ListViewItem lvi = new ListViewItem(subItems);
                ListViewItem listViewItem = listView1.Items.Add(lvi);
                listViewItem.Tag = j;
            }
            this.Text = $"Route Plotter | {totalJumps} jumps left";
        }

        private string MakeNumberSmaller(decimal v)
        {
            string[] sizeSuffix = { "Ly", "kLy", "mLy", "gLy" };

            int places = 0;
            while (v > 1000)
            {
                v = v / 1000;
                places++;
            }

            return $"{v:0.00}{sizeSuffix[places]}";
        }
    }
}
