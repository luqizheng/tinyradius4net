using System;
using System.Data.SqlClient;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using TinyRadius.Net.Cfg;
using TinyRadiusAdmin.Configurations;

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
            AccountListentIPTextBox.Text = Cfg.Instance.TinyConfig.AccountListentIp;
            AccountListentPort.Text = Cfg.Instance.TinyConfig.AcctPort.ToString();
            EnableAccountCheckBox.Checked = Cfg.Instance.TinyConfig.EnableAccount;

            AuthPortTextBox.Text = Cfg.Instance.TinyConfig.AuthPort.ToString();
            AuthListentIPTextBox.Text = Cfg.Instance.TinyConfig.AuthListentIp;
            enableAuthenticationCheckBox.Checked = Cfg.Instance.TinyConfig.EnableAuthentication;

            //Client Settings
            this.nasClientSetting1.DataSource = Cfg.Instance.TinyConfig.NasSettings;
            labelStatus.Text = TinyRadiusService.Status.ToString();
            SetServiceStatus(TinyRadiusService.Status == ServiceControllerStatus.Running);
            //Validation
            //databaseSetting
            TextBoxSQL.Text = Cfg.Instance.TinyConfig.DatabaseSetting.PasswordSql;
            TextBoxConnectionString.Text = Cfg.Instance.TinyConfig.DatabaseSetting.Connection;
            enableDataBase.Checked = Cfg.Instance.TinyConfig.ValidateByDatabase;
            //ldap
            TextBoxLdapPath.Text = Cfg.Instance.TinyConfig.LdapSetting.Path;
            textBoxDomain.Text = Cfg.Instance.TinyConfig.LdapSetting.DomainName;
            enableLDAP.Checked = Cfg.Instance.TinyConfig.ValidateByLdap;
            TinyRadiusService.StatusChangingEvent += TinyRadiusServiceStatusChangingEvent;
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
            if (SaveSetting())
            {
                DialogResult result = MessageBox.Show("有关键数据更改,只有重新启动服务才会生效，现在重启吗?", "信息", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    TinyRadiusService.Restart();
                }
            }
        }

        private bool SaveSetting()
        {
            try
            {
                bool autoRestart = SaveAuthSetting();

                SaveAccountSetting(ref autoRestart);

                Cfg.Instance.TinyConfig.ValidateByDatabase = enableDataBase.Checked;
                Cfg.Instance.TinyConfig.DatabaseSetting.Connection = TextBoxConnectionString.Text;
                Cfg.Instance.TinyConfig.DatabaseSetting.PasswordSql = TextBoxSQL.Text;

                Cfg.Instance.TinyConfig.ValidateByLdap = enableLDAP.Checked;
                Cfg.Instance.TinyConfig.LdapSetting.Path = TextBoxLdapPath.Text;
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
            bool autoRestart = false;
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




    }
}