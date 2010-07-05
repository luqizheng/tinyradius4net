using System;
using System.Net;
using System.Windows.Forms;
using TinyRadiusServer.Radius;

namespace TinyRadiusServer
{
    public partial class Form1 : Form
    {
        private readonly MockRadiusServer _server = new MockRadiusServer();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Service Setting;
            listentIP.Text = Services.Default.ListentIP;
            portTextBox.Text = Services.Default.Port.ToString();

            //Client Settings
            foreach (var entry in ClientSets.Instance)
            {
                var item = new ListViewItem(new[]
                                                {
                                                    entry.Key.ToString(),
                                                    entry.Value
                                                });
                clientListView.Items.Add(item);
            }
        }


        private void SaveServerSetting_Click(object sender, EventArgs e)
        {
            Services.Default.ListentIP = listentIP.Text;
            Services.Default.Port = Convert.ToInt32(portTextBox.Text);
        }

        private void Save_ClientItem(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(textBoxClientIp.Text);
            string sharekey = textBoxShareKey.Text;
            var item = new ListViewItem(new[] { ip.ToString(), sharekey });
            clientListView.Items.Add(item);

            ClientSets.Instance.Add(ip.ToString(), sharekey);
            ClientSets.Instance.Save();
        }

        private void Start_Server(object sender, EventArgs e)
        {
            _server.ListenAddress = IPAddress.Parse(Services.Default.ListentIP);
            _server.Start(true, true);

        }
    }
}