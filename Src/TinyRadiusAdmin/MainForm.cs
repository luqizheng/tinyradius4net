using System;
using System.Net;
using System.Windows.Forms;
using TinyRadius.Net.Cfg;

//using TinyRadiusServer.Radius;

namespace TinyRadiusServer
{
    public partial class MainForm : Form
    {
        ///private readonly MockRadiusServer _server = new MockRadiusServer();
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
            AccountListentIPTextBox.Text = Config.Instance.AccountListentIp;
            AccountListentPort.Text = Config.Instance.AcctPort.ToString();
            EnableAccountCheckBox.Checked = Config.Instance.EnableAccount;

            AuthPortTextBox.Text = Config.Instance.AuthPort.ToString();
            AuthListentIPTextBox.Text = Config.Instance.AuthListentIp;
            enableAuthenticationCheckBox.Checked = Config.Instance.EnableAuthentication;

            //Client Settings
            foreach (var entry in Config.Instance.NasSettings)
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
            SaveSetting();
        }

        private void SaveSetting()
        {
            //check IP formatter;
            IPAddress ip1;
            if (AuthListentIPTextBox.SelectedIndex != -1)
            {
                Config.Instance.AuthListentIp = AuthListentIPTextBox.Text;
            }
            else
            {
                MessageBox.Show("请选择验证监听IP");
            }
            Config.Instance.AuthPort = Convert.ToInt32(AuthPortTextBox.Text);
            Config.Instance.EnableAuthentication = enableAuthenticationCheckBox.Checked;

            if (AuthListentIPTextBox.SelectedIndex != -1)
            {
                Config.Instance.AccountListentIp = AccountListentIPTextBox.Text;
            }
            else
            {
                MessageBox.Show("请选择计费监听IP");
            }
            Config.Instance.AcctPort = Convert.ToInt32(AccountListentPort.Text);
            Config.Instance.EnableAccount = EnableAccountCheckBox.Checked;

            Config.Instance.Save();
        }

        private void Save_ClientItem(object sender, EventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(textBoxClientIp.Text);

                string sharekey = textBoxShareKey.Text;
                if (ip != null)
                {
                    var item = new ListViewItem(new[] { ip.ToString(), sharekey });
                    clientListView.Items.Add(item);
                }

                Config.Instance.NasSettings.Add(ip.ToString(), sharekey);
            }
            catch (FormatException)
            {
                MessageBox.Show("请输入正确的IP地址格式,如:10.169.1.123");
            }
        }

        private void Start_Server(object sender, EventArgs e)
        {
            const string ServiceName = "TinyRadius.Net Server";
            TinyRadiusService trs = new TinyRadiusService(ServiceName);

            SaveSetting();

            var btn = (Button)sender;
            if (btn.Tag.ToString() == "Stoped")
            {
                trs.Start();
                btn.Tag = "Started";
                btn.Text = "停止";
            }
            else
            {
                trs.Stop();
                btn.Tag = "Started";
                btn.Text = "开始";
            }
        }
    }
}