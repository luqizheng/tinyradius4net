namespace TinyRadiusAdmin
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.AccountListentIPTextBox = new System.Windows.Forms.ComboBox();
            this.EnableAccountCheckBox = new System.Windows.Forms.CheckBox();
            this.AccountListentPort = new System.Windows.Forms.TextBox();
            this.AccountPortTextBox = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AuthListentIPTextBox = new System.Windows.Forms.ComboBox();
            this.enableAuthenticationCheckBox = new System.Windows.Forms.CheckBox();
            this.PortLabel = new System.Windows.Forms.Label();
            this.AuthPortTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.nasClientSetting1 = new TinyRadiusAdmin.FormSetting.NasClientSetting();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.CheckBoxSSL = new System.Windows.Forms.CheckBox();
            this.GroupBoxCredential = new System.Windows.Forms.GroupBox();
            this.TextBoxCredentialPassword = new System.Windows.Forms.TextBox();
            this.TextBoxCredentialUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxServer = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.TextBoxLdapPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.enableLDAP = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonTestConnection = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.TextBoxSQL = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.TextBoxConnectionString = new System.Windows.Forms.TextBox();
            this.enableDataBase = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBoxAnonymous = new System.Windows.Forms.CheckBox();
            this.TextBoxCheckMac = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.GroupBoxCredential.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(711, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 21);
            this.button1.TabIndex = 0;
            this.button1.Tag = "Stoped";
            this.button1.Text = "启动";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Start_Server);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(533, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "保存";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.SaveServerSetting_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StatusLabel.Location = new System.Drawing.Point(481, 10);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 12);
            this.StatusLabel.TabIndex = 12;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(62, 10);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(29, 12);
            this.labelStatus.TabIndex = 13;
            this.labelStatus.Text = "未知";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(59, 12);
            this.label14.TabIndex = 14;
            this.label14.Text = "服务状态:";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(622, 5);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 21);
            this.button5.TabIndex = 10;
            this.button5.Text = "重启";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.ReStart_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.labelStatus);
            this.panel1.Controls.Add(this.StatusLabel);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 507);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(903, 34);
            this.panel1.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(903, 507);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(853, 441);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "监听设定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.AccountListentIPTextBox);
            this.groupBox2.Controls.Add(this.EnableAccountCheckBox);
            this.groupBox2.Controls.Add(this.AccountListentPort);
            this.groupBox2.Controls.Add(this.AccountPortTextBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 117);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(847, 139);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "计费";
            // 
            // AccountListentIPTextBox
            // 
            this.AccountListentIPTextBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AccountListentIPTextBox.FormattingEnabled = true;
            this.AccountListentIPTextBox.Location = new System.Drawing.Point(87, 27);
            this.AccountListentIPTextBox.Name = "AccountListentIPTextBox";
            this.AccountListentIPTextBox.Size = new System.Drawing.Size(121, 20);
            this.AccountListentIPTextBox.TabIndex = 9;
            // 
            // EnableAccountCheckBox
            // 
            this.EnableAccountCheckBox.AutoSize = true;
            this.EnableAccountCheckBox.Checked = true;
            this.EnableAccountCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EnableAccountCheckBox.Location = new System.Drawing.Point(87, 88);
            this.EnableAccountCheckBox.Name = "EnableAccountCheckBox";
            this.EnableAccountCheckBox.Size = new System.Drawing.Size(48, 16);
            this.EnableAccountCheckBox.TabIndex = 8;
            this.EnableAccountCheckBox.Text = "启动";
            this.EnableAccountCheckBox.UseVisualStyleBackColor = true;
            // 
            // AccountListentPort
            // 
            this.AccountListentPort.Location = new System.Drawing.Point(87, 50);
            this.AccountListentPort.Name = "AccountListentPort";
            this.AccountListentPort.Size = new System.Drawing.Size(121, 21);
            this.AccountListentPort.TabIndex = 7;
            // 
            // AccountPortTextBox
            // 
            this.AccountPortTextBox.AutoSize = true;
            this.AccountPortTextBox.Location = new System.Drawing.Point(13, 54);
            this.AccountPortTextBox.Name = "AccountPortTextBox";
            this.AccountPortTextBox.Size = new System.Drawing.Size(35, 12);
            this.AccountPortTextBox.TabIndex = 1;
            this.AccountPortTextBox.Text = "端口:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "监听IP:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AuthListentIPTextBox);
            this.groupBox1.Controls.Add(this.enableAuthenticationCheckBox);
            this.groupBox1.Controls.Add(this.PortLabel);
            this.groupBox1.Controls.Add(this.AuthPortTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(847, 114);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "用户验证";
            // 
            // AuthListentIPTextBox
            // 
            this.AuthListentIPTextBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AuthListentIPTextBox.FormattingEnabled = true;
            this.AuthListentIPTextBox.Location = new System.Drawing.Point(87, 25);
            this.AuthListentIPTextBox.Name = "AuthListentIPTextBox";
            this.AuthListentIPTextBox.Size = new System.Drawing.Size(121, 20);
            this.AuthListentIPTextBox.TabIndex = 9;
            // 
            // enableAuthenticationCheckBox
            // 
            this.enableAuthenticationCheckBox.AutoSize = true;
            this.enableAuthenticationCheckBox.Checked = true;
            this.enableAuthenticationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableAuthenticationCheckBox.Location = new System.Drawing.Point(87, 81);
            this.enableAuthenticationCheckBox.Name = "enableAuthenticationCheckBox";
            this.enableAuthenticationCheckBox.Size = new System.Drawing.Size(48, 16);
            this.enableAuthenticationCheckBox.TabIndex = 4;
            this.enableAuthenticationCheckBox.Text = "启动";
            this.enableAuthenticationCheckBox.UseVisualStyleBackColor = true;
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(13, 54);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(35, 12);
            this.PortLabel.TabIndex = 8;
            this.PortLabel.Text = "端口:";
            // 
            // AuthPortTextBox
            // 
            this.AuthPortTextBox.Location = new System.Drawing.Point(87, 54);
            this.AuthPortTextBox.Name = "AuthPortTextBox";
            this.AuthPortTextBox.Size = new System.Drawing.Size(121, 21);
            this.AuthPortTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "监听IP:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.nasClientSetting1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(853, 441);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "客户端设定";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // nasClientSetting1
            // 
            this.nasClientSetting1.DataSource = null;
            this.nasClientSetting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nasClientSetting1.Location = new System.Drawing.Point(3, 3);
            this.nasClientSetting1.Name = "nasClientSetting1";
            this.nasClientSetting1.Size = new System.Drawing.Size(847, 435);
            this.nasClientSetting1.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tabPage3.Size = new System.Drawing.Size(895, 481);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "验证";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxAnonymous);
            this.groupBox4.Controls.Add(this.CheckBoxSSL);
            this.groupBox4.Controls.Add(this.GroupBoxCredential);
            this.groupBox4.Controls.Add(this.TextBoxServer);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.TextBoxLdapPath);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.enableLDAP);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(0, 178);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(895, 259);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "LDAP";
            // 
            // CheckBoxSSL
            // 
            this.CheckBoxSSL.AutoSize = true;
            this.CheckBoxSSL.Location = new System.Drawing.Point(7, 114);
            this.CheckBoxSSL.Name = "CheckBoxSSL";
            this.CheckBoxSSL.Size = new System.Drawing.Size(90, 16);
            this.CheckBoxSSL.TabIndex = 11;
            this.CheckBoxSSL.Text = "Support SSL";
            this.CheckBoxSSL.UseVisualStyleBackColor = true;
            // 
            // GroupBoxCredential
            // 
            this.GroupBoxCredential.Controls.Add(this.TextBoxCredentialPassword);
            this.GroupBoxCredential.Controls.Add(this.TextBoxCredentialUserName);
            this.GroupBoxCredential.Controls.Add(this.label3);
            this.GroupBoxCredential.Controls.Add(this.label2);
            this.GroupBoxCredential.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.GroupBoxCredential.Location = new System.Drawing.Point(3, 169);
            this.GroupBoxCredential.Name = "GroupBoxCredential";
            this.GroupBoxCredential.Size = new System.Drawing.Size(889, 87);
            this.GroupBoxCredential.TabIndex = 10;
            this.GroupBoxCredential.TabStop = false;
            this.GroupBoxCredential.Text = "Credential Network Login";
            // 
            // TextBoxCredentialPassword
            // 
            this.TextBoxCredentialPassword.Location = new System.Drawing.Point(148, 53);
            this.TextBoxCredentialPassword.Name = "TextBoxCredentialPassword";
            this.TextBoxCredentialPassword.Size = new System.Drawing.Size(339, 21);
            this.TextBoxCredentialPassword.TabIndex = 13;
            // 
            // TextBoxCredentialUserName
            // 
            this.TextBoxCredentialUserName.Location = new System.Drawing.Point(148, 24);
            this.TextBoxCredentialUserName.Name = "TextBoxCredentialUserName";
            this.TextBoxCredentialUserName.Size = new System.Drawing.Size(339, 21);
            this.TextBoxCredentialUserName.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "Credential Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Credential UserName";
            // 
            // TextBoxServer
            // 
            this.TextBoxServer.Location = new System.Drawing.Point(85, 42);
            this.TextBoxServer.Name = "TextBoxServer";
            this.TextBoxServer.Size = new System.Drawing.Size(100, 21);
            this.TextBoxServer.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 45);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 4;
            this.label10.Text = "Server";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(85, 95);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(131, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "例子:DC=domain,DC=Net";
            // 
            // TextBoxLdapPath
            // 
            this.TextBoxLdapPath.Location = new System.Drawing.Point(85, 70);
            this.TextBoxLdapPath.Name = "TextBoxLdapPath";
            this.TextBoxLdapPath.Size = new System.Drawing.Size(615, 21);
            this.TextBoxLdapPath.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "LdapPath";
            // 
            // enableLDAP
            // 
            this.enableLDAP.AutoSize = true;
            this.enableLDAP.Checked = true;
            this.enableLDAP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableLDAP.Location = new System.Drawing.Point(8, 18);
            this.enableLDAP.Name = "enableLDAP";
            this.enableLDAP.Size = new System.Drawing.Size(48, 16);
            this.enableLDAP.TabIndex = 0;
            this.enableLDAP.Text = "启动";
            this.enableLDAP.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TextBoxCheckMac);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.buttonTestConnection);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.TextBoxSQL);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.TextBoxConnectionString);
            this.groupBox3.Controls.Add(this.enableDataBase);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(895, 168);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "数据库验证";
            // 
            // buttonTestConnection
            // 
            this.buttonTestConnection.Location = new System.Drawing.Point(709, 37);
            this.buttonTestConnection.Name = "buttonTestConnection";
            this.buttonTestConnection.Size = new System.Drawing.Size(75, 23);
            this.buttonTestConnection.TabIndex = 6;
            this.buttonTestConnection.Text = "Test";
            this.buttonTestConnection.UseVisualStyleBackColor = true;
            this.buttonTestConnection.Click += new System.EventHandler(this.TestConnection);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(91, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(641, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "例子：select password from userTable where userTable.UserName=@userName,其中@userName是" +
                "用户名参数,不能修改";
            // 
            // TextBoxSQL
            // 
            this.TextBoxSQL.Location = new System.Drawing.Point(93, 64);
            this.TextBoxSQL.Name = "TextBoxSQL";
            this.TextBoxSQL.Size = new System.Drawing.Size(607, 21);
            this.TextBoxSQL.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 3;
            this.label6.Text = "获取密码SQL";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "连接字符串：";
            // 
            // TextBoxConnectionString
            // 
            this.TextBoxConnectionString.Location = new System.Drawing.Point(93, 38);
            this.TextBoxConnectionString.Name = "TextBoxConnectionString";
            this.TextBoxConnectionString.Size = new System.Drawing.Size(607, 21);
            this.TextBoxConnectionString.TabIndex = 1;
            // 
            // enableDataBase
            // 
            this.enableDataBase.AutoSize = true;
            this.enableDataBase.Checked = true;
            this.enableDataBase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableDataBase.Location = new System.Drawing.Point(8, 18);
            this.enableDataBase.Name = "enableDataBase";
            this.enableDataBase.Size = new System.Drawing.Size(48, 16);
            this.enableDataBase.TabIndex = 0;
            this.enableDataBase.Text = "启动";
            this.enableDataBase.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label13);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(853, 441);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "帮助";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(20, 30);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(125, 12);
            this.label13.TabIndex = 2;
            this.label13.Text = "珠海科蓝电子有限公司";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 56);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(131, 12);
            this.label12.TabIndex = 1;
            this.label12.Text = "Email:55960367@qq.com";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(20, 82);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "电话:3333601";
            // 
            // checkBoxAnonymous
            // 
            this.checkBoxAnonymous.AutoSize = true;
            this.checkBoxAnonymous.Location = new System.Drawing.Point(7, 136);
            this.checkBoxAnonymous.Name = "checkBoxAnonymous";
            this.checkBoxAnonymous.Size = new System.Drawing.Size(48, 16);
            this.checkBoxAnonymous.TabIndex = 12;
            this.checkBoxAnonymous.Text = "匿名";
            this.checkBoxAnonymous.UseVisualStyleBackColor = true;
            this.checkBoxAnonymous.CheckedChanged += new System.EventHandler(this.checkBoxAnonymous_CheckedChanged);
            // 
            // TextBoxCheckMac
            // 
            this.TextBoxCheckMac.Location = new System.Drawing.Point(93, 123);
            this.TextBoxCheckMac.Name = "TextBoxCheckMac";
            this.TextBoxCheckMac.Size = new System.Drawing.Size(607, 21);
            this.TextBoxCheckMac.TabIndex = 10;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 123);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 9;
            this.label15.Text = "检查MacSql";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 541);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "科蓝移动Radius控制台";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.GroupBoxCredential.ResumeLayout(false);
            this.GroupBoxCredential.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox AccountListentIPTextBox;
        private System.Windows.Forms.CheckBox EnableAccountCheckBox;
        private System.Windows.Forms.TextBox AccountListentPort;
        private System.Windows.Forms.Label AccountPortTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox AuthListentIPTextBox;
        private System.Windows.Forms.CheckBox enableAuthenticationCheckBox;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.TextBox AuthPortTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private TinyRadiusAdmin.FormSetting.NasClientSetting nasClientSetting1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox TextBoxServer;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TextBoxLdapPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox enableLDAP;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonTestConnection;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TextBoxSQL;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TextBoxConnectionString;
        private System.Windows.Forms.CheckBox enableDataBase;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox CheckBoxSSL;
        private System.Windows.Forms.GroupBox GroupBoxCredential;
        private System.Windows.Forms.TextBox TextBoxCredentialPassword;
        private System.Windows.Forms.TextBox TextBoxCredentialUserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxAnonymous;
        private System.Windows.Forms.TextBox TextBoxCheckMac;
        private System.Windows.Forms.Label label15;

    }
}

