using System;
using System.Net;
using System.Windows.Forms;
using Microsoft.Win32;
using TinyRadius.Net.Cfg;
using TinyRadiusAdmin.Configurations;

//using TinyRadiusServer.Radius;

namespace TinyRadiusAdmin
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
            AccountListentIPTextBox.Text = Cfg.Instance.TinyConfig.AccountListentIp;
            AccountListentPort.Text = Cfg.Instance.TinyConfig.AcctPort.ToString();
            EnableAccountCheckBox.Checked = Cfg.Instance.TinyConfig.EnableAccount;

            AuthPortTextBox.Text = Cfg.Instance.TinyConfig.AuthPort.ToString();
            AuthListentIPTextBox.Text = Cfg.Instance.TinyConfig.AuthListentIp;
            enableAuthenticationCheckBox.Checked = Cfg.Instance.TinyConfig.EnableAuthentication;

            //Client Settings
            foreach (var entry in Cfg.Instance.TinyConfig.NasSettings)
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
                Cfg.Instance.TinyConfig.AuthListentIp = AuthListentIPTextBox.Text;
            }
            else
            {
                MessageBox.Show("请选择验证监听IP");
            }
            Cfg.Instance.TinyConfig.AuthPort = Convert.ToInt32(AuthPortTextBox.Text);
            Cfg.Instance.TinyConfig.EnableAuthentication = enableAuthenticationCheckBox.Checked;

            if (AuthListentIPTextBox.SelectedIndex != -1)
            {
                Cfg.Instance.TinyConfig.AccountListentIp = AccountListentIPTextBox.Text;
            }
            else
            {
                MessageBox.Show("请选择计费监听IP");
            }
            Cfg.Instance.TinyConfig.AcctPort = Convert.ToInt32(AccountListentPort.Text);
            Cfg.Instance.TinyConfig.EnableAccount = EnableAccountCheckBox.Checked;

            Cfg.Instance.TinyConfig.Save();
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

                Cfg.Instance.TinyConfig.NasSettings.Add(ip.ToString(), sharekey);
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
                btn.Tag = "Stoped";
                btn.Text = "开始";
            }
        }
    }
}