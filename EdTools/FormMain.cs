using System;
using System.Windows.Forms;

using EDNeutronRouterPlugin;

using Newtonsoft.Json.Linq;

namespace EdTools
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void ButtonStartRoute_Click(object sender, EventArgs e)
        {
            JToken jToken;
            try
            {
                jToken = EDNeutronRouterPlugin.NeutronRouterAPI.GetNewRoute("sol", textBox1.Text, 70, 60);
            }
            catch (Exception ex)
            {
                Type t = ex.GetType();

                if (t == typeof(InvalidSystemException))
                {
                    MessageBox.Show("Invalid system", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                    throw;
            }

            listView1.Items.Clear();
            foreach (JObject j in jToken.Value<JArray>("system_jumps"))
            {
                string[] subItems = new string[]
                {
                    j.Value<string>("system"),
                    MakeNumberSmaller(j.Value<decimal>("distance_jumped")),
                    MakeNumberSmaller(j.Value<decimal>("distance_left")),
                    j.Value<string>("jumps"),
                    j.Value<string>("neutron_star")
                };
                ListViewItem lvi = new ListViewItem(subItems);
                listView1.Items.Add(lvi).Tag = j;
            }
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
