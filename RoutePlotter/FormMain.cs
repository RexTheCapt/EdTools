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
        private RtcLib.Settings _settings;

        internal JournalScanner JournalScanner { get; private set; }
        internal string _ranUpdateInSystem = "";

        public string CurrentStarSystem
        {
            get => _currentStarSystem;
            private set
            {
                if (value.Equals(textBoxCurrentSystem.Text, StringComparison.OrdinalIgnoreCase))
                    return;

                textBoxCurrentSystem.ForeColor = Color.Black;
                textBoxCurrentSystem.BackColor = Color.FromName("Control");

                _currentStarSystem = value;
                textBoxCurrentSystem.Text = _currentStarSystem;

                if (JournalScanner != null && !JournalScanner.FirstRun)
                {
                    UpdateTravelPath(false);
                }
            }
        }

        public FormMain()
        {
            InitializeComponent();

            _settings = new RtcLib.Settings("RoutePlotter", "Settings", "RexTheCapt", RtcLib.Settings.LocationEnum.Roaming);
            numericUpDownJumpRange.Value = _settings.GetDecimal("range", 10);

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
            JournalScanner.SendTextHandler += JournalScanner_SendTextHandler;
            JournalScanner.LoadoutHandler += JournalScanner_LoadoutHandler;
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

            this.FormClosing += FormMain_FormClosing;

            _currentStarSystem = "[UNKNOWN]";
        }

        private void FormMain_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _settings.SetDecimal("range", numericUpDownJumpRange.Value);
            _settings.Save();
        }

        private void JournalScanner_LoadoutHandler(object? sender, EventArgs e)
        {
            JournalScanner.LoadoutEventArgs eArgs = (JournalScanner.LoadoutEventArgs)e;
        }

        private void JournalScanner_SendTextHandler(object? sender, EventArgs e)
        {
            JournalScanner.SendTextEventArgs eArgs = (JournalScanner.SendTextEventArgs)e;
            if (eArgs.FirstRun)
                return;

            string prefix = "/";
            string text = (eArgs.SendText.Value<string>("Message") ?? "").ToUpper();

            if (text.StartsWith(prefix) && text.Length > prefix.Length)
            {
                string sText = text.Substring(prefix.Length);
                string[] split = sText.Split(' ');

                string arg;
                switch (split[0])
                {
                    case "RANGE":
                        if (split.Length < 2)
                            return;

                        arg = split[1];
                        if (decimal.TryParse(arg, out decimal res))
                        {
                            numericUpDownJumpRange.Value = res;
                        }
                        break;
                    case "DEST":
                    case "DESTINATION":
                    case "TARGETSYSTEM":
                    case "TARGET":
                    case "SYSTEM":
                        if (split.Length < 2)
                            return;

                        arg = split[1];

                        textBoxTargetSystem.Text = sText.Substring(sText.IndexOf(' ')).Trim();
                        UpdateTravelPath(true);
                        break;
                    case "UPDATE":
                        UpdateTravelPath(true);
                        break;
                    case "TOP":
                        checkBox1.Checked = !checkBox1.Checked;
                        break;
                    case "NEXT":
                        SetClipboard(1);
                        break;
                    case "PREVIOUS":
                    case "PREV":
                        SetClipboard(-1);
                        break;
                }
            }
        }

        private void SetClipboard(int v)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                ListViewItem lvi = listView1.Items[i];

                if (lvi.BackColor == Color.Green)
                {
                    lvi.BackColor = Color.FromName("Control");
                    lvi.ForeColor = Color.Black;

                    ListViewItem trg = listView1.Items[i + v];
                    Clipboard.SetText(trg.Text);
                    trg.BackColor = Color.Green;
                    trg.ForeColor = Color.White;
                    return;
                }
            }
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

        /// <summary>
        /// Fetch updated route from spansh
        /// </summary>
        private void UpdateTravelPath(bool force)
        {
            if (!force)
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
                    /*
                     * timestamp
                     * event
                     * WarketId
                     * StationName
                     * StationType
                     * StarSystem
                     */
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
                    /*
                     * {
  "timestamp": "2022-01-14T19:59:02Z",
  "event": "ApproachBody",
  "StarSystem": "Arangorii",
  "SystemAddress": 7230745219802,
  "Body": "Arangorii B 1",
  "BodyID": 50
}
                     */
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
                    textBoxCurrentSystem.BackColor = Color.Green;
                    textBoxCurrentSystem.ForeColor = Color.White;
                    _delayedUpdate.Enabled = true;
                    return;
                }
                else
                    throw;
            }

            listView1.Items.Clear();
            bool skipFirst = true;
            bool nextRouteCopied = false;
            uint totalJumps = 0;
            decimal firstDistanceLeft = 0;
            JArray list = jToken.Value<JArray>("system_jumps") ?? new();
            for (int i = 0; i < list.Count; i++)
            {
                JObject j = (JObject)list[i];
                totalJumps += j.Value<uint>("jumps");

                if (skipFirst)
                {
                    skipFirst = false;
                    firstDistanceLeft = j.Value<decimal>("distance_left");
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

                ListViewItem lvi = new ListViewItem(subItems);
                if ((!nextRouteCopied && i == list.Count - 1) || (!nextRouteCopied && firstDistanceLeft - j.Value<decimal>("distance_left") > numericUpDownJumpRange.Value * 4))
                {
                    Clipboard.SetText(subItems[0]);
                    nextRouteCopied = true;
                    lvi.BackColor = Color.Green;
                    lvi.ForeColor = Color.White;
                }

                //if (copySecond)
                //{
                //    Clipboard.SetText(subItems[0]);
                //    copySecond = false;
                //}

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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkBox1.Checked;
        }
    }
}
