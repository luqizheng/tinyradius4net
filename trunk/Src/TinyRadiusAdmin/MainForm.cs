using System;
using System.Net;
using System.ServiceProcess;
using System.Windows.Forms;
using TinyRadiusAdmin.Configurations;

//using TinyRadiusServer.Radius;

namespace TinyRadiusAdmin
{
    public partial class MainForm : Form
    {
        TinyRadiusService tinyRadiusService = new TinyRadiusService();
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
            //Start button stats;
        }


        private void SaveServerSetting_Click(object sender, EventArgs e)
        {
            if (SaveSetting())
            {
                var result = MessageBox.Show("发现有关键数据更改，必须从其服务，是现在重启服务吗？", "", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    tinyRadiusService.Restart();
                }
            }
        }

        private bool SaveSetting()
        {
            var autoRestart = SaveAuthSetting();

            if (!SaveAccountSetting(ref autoRestart)) return false;

            Cfg.Instance.TinyConfig.ValidateByDatabase = this.enableDataBase.Checked;
            Cfg.Instance.TinyConfig.DatabaseSetting.Connection = this.TextBoxConnectionString.Text;
            Cfg.Instance.TinyConfig.DatabaseSetting.PasswordSql = this.TextBoxSQL.Text;

            Cfg.Instance.TinyConfig.ValidateByLdap = this.enableLDAP.Checked;
            Cfg.Instance.TinyConfig.LdapSetting.Path = this.TextBoxLdapPath.Text;
            Cfg.Instance.TinyConfig.LdapSetting.DomainName = textBoxDomain.Text;

            Cfg.Instance.TinyConfig.Save();
            return autoRestart;
        }

        private bool SaveAccountSetting(ref bool autoRestart)
        {
            if (AuthListentIPTextBox.SelectedIndex != -1)
            {
                if (!autoRestart)
                {
                    autoRestart = Cfg.Instance.TinyConfig.AuthListentIp != AuthListentIPTextBox.Text;
                }
                Cfg.Instance.TinyConfig.AccountListentIp = AccountListentIPTextBox.Text;
            }
            else
            {
                MessageBox.Show("请选择计费监听IP");
                return false;
            }
            if (!autoRestart)
            {
                autoRestart = Cfg.Instance.TinyConfig.AcctPort != Convert.ToInt32(AccountListentPort.Text);
            }
            Cfg.Instance.TinyConfig.AcctPort = Convert.ToInt32(AccountListentPort.Text);
            if (!autoRestart)
            {
                autoRestart = Cfg.Instance.TinyConfig.EnableAccount != EnableAccountCheckBox.Checked;
            }
            Cfg.Instance.TinyConfig.EnableAccount = EnableAccountCheckBox.Checked;
            return autoRestart;
        }

        private bool SaveAuthSetting()
        {
            var autoRestart = false;
            IPAddress ip1;
            if (AuthListentIPTextBox.SelectedIndex != -1)
            {
                autoRestart = Cfg.Instance.TinyConfig.AuthListentIp != AuthListentIPTextBox.Text;
                Cfg.Instance.TinyConfig.AuthListentIp = AuthListentIPTextBox.Text;
            }
            else
            {
                MessageBox.Show("请选择验证监听IP");
                return false;
            }

            if (!autoRestart)
            {
                autoRestart = Cfg.Instance.TinyConfig.AuthPort != Convert.ToInt32(AuthPortTextBox.Text);
            }
            Cfg.Instance.TinyConfig.AuthPort = Convert.ToInt32(AuthPortTextBox.Text);

            if (!autoRestart)
            {
                autoRestart = Cfg.Instance.TinyConfig.EnableAuthentication != enableAuthenticationCheckBox.Checked;
            }
            Cfg.Instance.TinyConfig.EnableAuthentication = enableAuthenticationCheckBox.Checked;
            return autoRestart;
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
            var btn = (Button)sender;
            var status = (ServiceControllerStatus) btn.Tag;
            if (status==ServiceControllerStatus.Running)
            {
                tinyRadiusService.Start();
                btn.Text = "停止";
            }
            else
            {
                tinyRadiusService.Stop();
                btn.Text = "开始";
            }
            btn.Tag = tinyRadiusService.Status;
        }
    }
}