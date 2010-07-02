using System;
using System.Net;
using System.Windows.Forms;

namespace TinyRadiusServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Service Setting;
            this.listentIP.Text = Services.Default.ListentIP;
            this.portTextBox.Text = Services.Default.Port.ToString();

            //Client Settings
            ClientSets.Instance.Init(Client.Default.ClientIPs, Client.Default.ShareKey);
            foreach (var entry in ClientSets.Instance)
            {
                var item = new ListViewItem(new[]
                                                {
                                                 entry.Key.ToString(),
                                                 entry.Value
            });
                this.clientListView.Items.Add(item);
            }
        }


        private void SaveServerSetting_Click(object sender, EventArgs e)
        {
            Services.Default.ListentIP = this.listentIP.Text;
            Services.Default.Port = Convert.ToInt32(this.portTextBox.Text);
        }

        private void Save_ClientItem(object sender, EventArgs e)
        {
            var ip = IPAddress.Parse(textBoxClientIp.Text);
            var sharekey = textBoxShareKey.Text;
            var item = new ListViewItem(new string[] { ip.ToString(), sharekey });
            this.clientListView.Items.Add(item);

            ClientSets.Instance.Add(ip, sharekey);
        }
    }
}
