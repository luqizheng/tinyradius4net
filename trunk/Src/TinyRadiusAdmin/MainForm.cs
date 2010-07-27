using System;
using System.Net;
using System.ServiceProcess;
using System.Windows.Forms;
using Microsoft.Win32;
using TinyRadiusAdmin.Configurations;

//using TinyRadiusServer.Radius;

namespace TinyRadiusAdmin
{
    public partial class MainForm : Form
    {
        TinyRadiusService tinyRadiusService = new TinyRadiusService();
        private const string RegistryPath = @"SYSTEM\CurrentControlSet\services\TinyRadius.Net Server";
        public MainForm()
        {
            InitializeComponent();
        }
        public string GetServicePath()
        {
           
            RegistryKey registry =
                Registry.LocalMachine.OpenSubKey(RegistryPath);
            try
            {
                if (registry == null)
                {
                    throw new ApplicationException("TinyRadius Server没有安装");
                }
                var path = registry.GetValue("ImagePath").ToString();
                
                if (path.StartsWith("\""))
                {
                    path = path.Substring(1);
                }
                if (path.EndsWith("\""))
                {
                    path = path.Substring(0, path.Length - 1);
                }

                int lastBackslash = path.LastIndexOf('\\');
                return path.Substring(0, lastBackslash);
            }
            finally
            {
                if (registry != null)
                    registry.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName());
            AuthListentIPTextBox.Items.AddRange(ipHost.AddressList);
            AccountListentIPTextBox.Items.AddRange(ipHost.AddressList);

            MessageBox.Show(GetServicePath());
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

            SetServiceStatus(tinyRadiusService.Status == ServiceControllerStatus.Running);
            tinyRadiusService.StatusChangingEvent += new EventHandler(tinyRadiusService_StatusChangingEvent);

        }

        void tinyRadiusService_StatusChangingEvent(object sender, EventArgs e)
        {
            if (!InvokeRequired)
            {
                this.labelStatus.Text = ((TinyRadiusService)sender).Status.ToString();
            }
            else
            {
                this.Invoke(new Action<object, EventArgs>(tinyRadiusService_StatusChangingEvent), sender, e);
            }
        }


        private void SetServiceStatus(bool isRuning)
        {
            if (!InvokeRequired)
            {
                this.button1.Text = isRuning ? "停止" : "启动";
                this.button1.Tag = isRuning;
            }
            else
            {
                Invoke(new Action<bool>(SetServiceStatus), isRuning);
            }
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
            try
            {
                var autoRestart = SaveAuthSetting();

                SaveAccountSetting(ref autoRestart);

                Cfg.Instance.TinyConfig.ValidateByDatabase = this.enableDataBase.Checked;
                Cfg.Instance.TinyConfig.DatabaseSetting.Connection = this.TextBoxConnectionString.Text;
                Cfg.Instance.TinyConfig.DatabaseSetting.PasswordSql = this.TextBoxSQL.Text;

                Cfg.Instance.TinyConfig.ValidateByLdap = this.enableLDAP.Checked;
                Cfg.Instance.TinyConfig.LdapSetting.Path = this.TextBoxLdapPath.Text;
                Cfg.Instance.TinyConfig.LdapSetting.DomainName = textBoxDomain.Text;

                Cfg.Instance.TinyConfig.Save();
                return autoRestart;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

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
            if (Convert.ToBoolean(btn.Tag))
            {
                tinyRadiusService.Stop();
                SetServiceStatus(false);
            }
            else
            {
                tinyRadiusService.Start();
                SetServiceStatus(true);
            }

        }

        private void ReStart_Click(object sender, EventArgs e)
        {
            tinyRadiusService.Restart();
        }
    }
}