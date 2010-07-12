using System;
using System.Net;
using System.Windows.Forms;
using TinyRadiusServer.Radius;

namespace TinyRadiusServer
{
    public partial class MainForm : Form
    {
        private readonly MockRadiusServer _server = new MockRadiusServer();

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName());
            AuthListentIPTextBox.Items.AddRange(ipHost.AddressList);
            AccountListentIPTextBox.Items.AddRange(ipHost.AddressList);
            //Service Setting;
            AccountListentIPTextBox.Text = Services.Default.AccountListentIP;
            AccountListentPort.Text = Services.Default.AccountPort.ToString();

            AuthPortTextBox.Text = Services.Default.AuthPort.ToString();
            AuthListentIPTextBox.Text = Services.Default.AuthListentIP;



            IPAddress ipAddr = ipHost.AddressList[0];


            //Client Settings)
            foreach (var entry in ClientSets.Instance)
            {
                var item = new ListViewItem(new[]
                                                {
                                                    entry.Key,
                                                    entry.Value
                                                });
                clientListView.Items.Add(item);
            }
        }


        private void SaveServerSetting_Click(object sender, EventArgs e)
        {
            SaveListentSetting();
        }

        private void SaveListentSetting()
        {
            try
            {
                //check IP formatter;
                IPAddress ip1;
                if (IPAddress.TryParse(AuthListentIPTextBox.Text, out ip1))
                {
                    Services.Default.AuthListentIP = AuthListentIPTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Auth listent IP fomat isn't correct.");
                }
                Services.Default.AuthPort = Convert.ToInt32(AuthPortTextBox.Text);

                if (IPAddress.TryParse(AuthListentIPTextBox.Text, out ip1))
                {
                    Services.Default.AccountListentIP = AccountListentIPTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Account listent IP fomat isn't correct.");
                }
                Services.Default.AccountPort = Convert.ToInt32(AccountListentPort.Text);

                Services.Default.Save();
            }
            catch (InvalidCastException error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Save_ClientItem(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(textBoxClientIp.Text);
            string sharekey = textBoxShareKey.Text;
            if (ip != null)
            {
                var item = new ListViewItem(new[] { ip.ToString(), sharekey });
                clientListView.Items.Add(item);
            }

            ClientSets.Instance.Add(ip.ToString(), sharekey);
            ClientSets.Instance.Save();
        }

        private void Start_Server(object sender, EventArgs e)
        {
            SaveListentSetting();
            var btn = (Button)sender;
            if (btn.Tag.ToString() == "Stoped")
            {
                _server.ListenAddress = IPAddress.Parse(Services.Default.AuthListentIP);
                _server.Start(enableAuthenticationCheckBox.Checked, EnableAccountCheckBox.Checked);
                btn.Tag = "Started";
                btn.Text = "Stop";
            }
            else
            {
                btn.Tag = null;
                btn.Text = "Start";
                _server.Stop();
            }
        }
    }
}