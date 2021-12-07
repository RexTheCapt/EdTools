using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

using RestSharp;

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
            JToken jToken = EDNeutronRouterPlugin.NeutronRouterAPI.GetNewRoute("sol", textBox1.Text, 70, 60);

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
