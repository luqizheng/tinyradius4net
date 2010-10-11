using System;
using System.Data.SqlClient;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using log4net;

using TinyRadiusService.Cfg;

namespace TinyRadiusAdmin
{
    public partial class MainForm : Form
    {
        public TinyRadiusService TinyRadiusService
        { get; set; }


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
            AccountListentIPTextBox.Text = ServiceCfg.Instance.TinyConfig.AccountListentIp;
            AccountListentPort.Text = ServiceCfg.Instance.TinyConfig.AcctPort.ToString();
            EnableAccountCheckBox.Checked = ServiceCfg.Instance.TinyConfig.EnableAccount;

            AuthPortTextBox.Text = ServiceCfg.Instance.TinyConfig.AuthPort.ToString();
            AuthListentIPTextBox.Text = ServiceCfg.Instance.TinyConfig.AuthListentIp;
            enableAuthenticationCheckBox.Checked = ServiceCfg.Instance.TinyConfig.EnableAuthentication;

            //Client Settings
            this.nasClientSetting1.DataSource = ServiceCfg.Instance.TinyConfig.NasSettings;
            labelStatus.Text = TinyRadiusService.Status.ToString();
            SetServiceStatus(TinyRadiusService.Status == ServiceControllerStatus.Running);
            //Validation
            //databaseSetting
            TextBoxSQL.Text = ServiceCfg.Instance.TinyConfig.DatabaseSetting.PasswordSql;
            TextBoxConnectionString.Text = ServiceCfg.Instance.TinyConfig.DatabaseSetting.Connection;
            TextBoxCheckMac.Text = ServiceCfg.Instance.TinyConfig.DatabaseSetting.MacSql;
            enableDataBase.Checked = ServiceCfg.Instance.TinyConfig.ValidateByDatabase;

            //ldap
            TextBoxLdapPath.Text = ServiceCfg.Instance.TinyConfig.LdapSetting.SearchUserPath;
            TextBoxServer.Text = ServiceCfg.Instance.TinyConfig.LdapSetting.Server;
            enableLDAP.Checked = ServiceCfg.Instance.TinyConfig.ValidateByLdap;
            TinyRadiusService.StatusChangingEvent += TinyRadiusServiceStatusChangingEvent;
            TextBoxCredentialUserName.Text = ServiceCfg.Instance.TinyConfig.LdapSetting.CredentialUserName;
            TextBoxCredentialPassword.Text = ServiceCfg.Instance.TinyConfig.LdapSetting.CredentialPassword;
        }

        private void TinyRadiusServiceStatusChangingEvent(object sender, EventArgs e)
        {
            var service = ((TinyRadiusService)sender);
            if (!InvokeRequired)
            {
                labelStatus.Text = service.Status.ToString();
                switch (service.Status)
                {
                    case ServiceControllerStatus.Running:
                        SetServiceStatus(true);
                        button1.Enabled = true;
                        break;
                    case ServiceControllerStatus.Stopped:
                        SetServiceStatus(false);
                        button1.Enabled = true;
                        break;
                    default:
                        button1.Enabled = false;
                        break;
                }
            }
            else
            {
                Invoke(new Action<object, EventArgs>(TinyRadiusServiceStatusChangingEvent), sender, e);
            }
        }


        private void SetServiceStatus(bool isRuning)
        {
            if (!InvokeRequired)
            {
                button1.Text = isRuning ? "停止" : "启动";
                button1.Tag = isRuning;
            }
            else
            {
                Invoke(new Action<bool>(SetServiceStatus), isRuning);
            }
        }


        private void SaveServerSetting_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveSetting())
                {
                    DialogResult result = MessageBox.Show("有关键数据更改,只有重新启动服务才会生效，现在重启吗?", "信息", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        TinyRadiusService.Restart();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(this.GetType()).Error(ex.Message, ex);
                MessageBox.Show(ex.Message);
            }
        }

        private bool SaveSetting()
        {
            try
            {
                bool autoRestart = SaveAuthSetting();

                SaveAccountSetting(ref autoRestart);

                ServiceCfg.Instance.TinyConfig.ValidateByDatabase = enableDataBase.Checked;
                ServiceCfg.Instance.TinyConfig.DatabaseSetting.Connection = TextBoxConnectionString.Text;
                ServiceCfg.Instance.TinyConfig.DatabaseSetting.PasswordSql = TextBoxSQL.Text;
                ServiceCfg.Instance.TinyConfig.DatabaseSetting.MacSql = TextBoxCheckMac.Text;

                ServiceCfg.Instance.TinyConfig.ValidateByLdap = enableLDAP.Checked;
                ServiceCfg.Instance.TinyConfig.LdapSetting.SearchUserPath = TextBoxLdapPath.Text;
                ServiceCfg.Instance.TinyConfig.LdapSetting.Server = TextBoxServer.Text;

                if (!this.checkBoxAnonymous.Checked)
                {
                    ServiceCfg.Instance.TinyConfig.LdapSetting.CredentialUserName = TextBoxCredentialUserName.Text;
                    ServiceCfg.Instance.TinyConfig.LdapSetting.CredentialPassword = TextBoxCredentialPassword.Text;
                }
                ServiceCfg.Instance.TinyConfig.LdapSetting.IsSsl = this.CheckBoxSSL.Checked;

                ServiceCfg.Instance.TinyConfig.Save();
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
                    autoRestart = ServiceCfg.Instance.TinyConfig.AuthListentIp != AuthListentIPTextBox.Text;
                }
                ServiceCfg.Instance.TinyConfig.AccountListentIp = AccountListentIPTextBox.Text;
            }
            else
            {
                MessageBox.Show("请选择计费监听IP");
                return false;
            }
            if (!autoRestart)
            {
                autoRestart = ServiceCfg.Instance.TinyConfig.AcctPort != Convert.ToInt32(AccountListentPort.Text);
            }
            ServiceCfg.Instance.TinyConfig.AcctPort = Convert.ToInt32(AccountListentPort.Text);
            if (!autoRestart)
            {
                autoRestart = ServiceCfg.Instance.TinyConfig.EnableAccount != EnableAccountCheckBox.Checked;
            }
            ServiceCfg.Instance.TinyConfig.EnableAccount = EnableAccountCheckBox.Checked;
            return autoRestart;
        }

        private bool SaveAuthSetting()
        {
            bool autoRestart = false;
            IPAddress ip1;
            if (AuthListentIPTextBox.SelectedIndex != -1)
            {
                autoRestart = ServiceCfg.Instance.TinyConfig.AuthListentIp != AuthListentIPTextBox.Text;
                ServiceCfg.Instance.TinyConfig.AuthListentIp = AuthListentIPTextBox.Text;
            }
            else
            {
                MessageBox.Show("请选择验证监听IP");
                return false;
            }

            if (!autoRestart)
            {
                autoRestart = ServiceCfg.Instance.TinyConfig.AuthPort != Convert.ToInt32(AuthPortTextBox.Text);
            }
            ServiceCfg.Instance.TinyConfig.AuthPort = Convert.ToInt32(AuthPortTextBox.Text);

            if (!autoRestart)
            {
                autoRestart = ServiceCfg.Instance.TinyConfig.EnableAuthentication != enableAuthenticationCheckBox.Checked;
            }
            ServiceCfg.Instance.TinyConfig.EnableAuthentication = enableAuthenticationCheckBox.Checked;
            return autoRestart;
        }

        private void Start_Server(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (Convert.ToBoolean(btn.Tag))
            {
                TinyRadiusService.Stop();
                SetServiceStatus(false);
            }
            else
            {
                TinyRadiusService.Start();
                SetServiceStatus(true);
            }
        }

        private void ReStart_Click(object sender, EventArgs e)
        {
            TinyRadiusService.Restart();
        }

        private void TestConnection(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;
            ThreadPool.QueueUserWorkItem(delegate(object state)
                                             {
                                                 SqlConnection conn = null;
                                                 try
                                                 {
                                                     conn = new SqlConnection(TextBoxConnectionString.Text);
                                                     conn.Open();
                                                     if (conn.State == System.Data.ConnectionState.Open)
                                                     {
                                                         MessageBox.Show("连接成功");
                                                     }
                                                     else
                                                     {
                                                         MessageBox.Show("连接失败,状态为" + conn.State);
                                                     }
                                                 }
                                                 catch (Exception ex)
                                                 {
                                                     MessageBox.Show("连接失败,信息如下:" + ex.Message);
                                                 }
                                                 finally
                                                 {
                                                     if (conn != null)
                                                         conn.Dispose();
                                                 }
                                                 this.Invoke(
                                                     new Action<Button>(delegate(Button btn1) { btn1.Enabled = true; }), (Button)state);
                                             }, sender);
        }

        private void checkBoxAnonymous_CheckedChanged(object sender, EventArgs e)
        {
            GroupBoxCredential.Enabled = !this.checkBoxAnonymous.Checked;
        }




    }
}