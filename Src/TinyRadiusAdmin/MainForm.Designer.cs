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
            this.button1 = new System.Windows.Forms.Button();
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxShareKey = new System.Windows.Forms.TextBox();
            this.textBoxClientIp = new System.Windows.Forms.TextBox();
            this.clientListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxDomain = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.TextBoxLdapPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.enableLDAP = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TextBoxSQL = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.TextBoxConnectionString = new System.Windows.Forms.TextBox();
            this.enableDataBase = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(657, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Tag = "Stoped";
            this.button1.Text = "启动";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Start_Server);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(742, 427);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(734, 401);
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
            this.groupBox2.Location = new System.Drawing.Point(3, 127);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(728, 151);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "计费";
            // 
            // AccountListentIPTextBox
            // 
            this.AccountListentIPTextBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AccountListentIPTextBox.FormattingEnabled = true;
            this.AccountListentIPTextBox.Location = new System.Drawing.Point(87, 29);
            this.AccountListentIPTextBox.Name = "AccountListentIPTextBox";
            this.AccountListentIPTextBox.Size = new System.Drawing.Size(121, 21);
            this.AccountListentIPTextBox.TabIndex = 9;
            // 
            // EnableAccountCheckBox
            // 
            this.EnableAccountCheckBox.AutoSize = true;
            this.EnableAccountCheckBox.Checked = true;
            this.EnableAccountCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EnableAccountCheckBox.Location = new System.Drawing.Point(87, 95);
            this.EnableAccountCheckBox.Name = "EnableAccountCheckBox";
            this.EnableAccountCheckBox.Size = new System.Drawing.Size(50, 17);
            this.EnableAccountCheckBox.TabIndex = 8;
            this.EnableAccountCheckBox.Text = "启动";
            this.EnableAccountCheckBox.UseVisualStyleBackColor = true;
            // 
            // AccountListentPort
            // 
            this.AccountListentPort.Location = new System.Drawing.Point(87, 54);
            this.AccountListentPort.Name = "AccountListentPort";
            this.AccountListentPort.Size = new System.Drawing.Size(121, 20);
            this.AccountListentPort.TabIndex = 7;
            // 
            // AccountPortTextBox
            // 
            this.AccountPortTextBox.AutoSize = true;
            this.AccountPortTextBox.Location = new System.Drawing.Point(13, 59);
            this.AccountPortTextBox.Name = "AccountPortTextBox";
            this.AccountPortTextBox.Size = new System.Drawing.Size(34, 13);
            this.AccountPortTextBox.TabIndex = 1;
            this.AccountPortTextBox.Text = "端口:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
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
            this.groupBox1.Size = new System.Drawing.Size(728, 124);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "用户验证";
            // 
            // AuthListentIPTextBox
            // 
            this.AuthListentIPTextBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AuthListentIPTextBox.FormattingEnabled = true;
            this.AuthListentIPTextBox.Location = new System.Drawing.Point(87, 27);
            this.AuthListentIPTextBox.Name = "AuthListentIPTextBox";
            this.AuthListentIPTextBox.Size = new System.Drawing.Size(121, 21);
            this.AuthListentIPTextBox.TabIndex = 9;
            // 
            // enableAuthenticationCheckBox
            // 
            this.enableAuthenticationCheckBox.AutoSize = true;
            this.enableAuthenticationCheckBox.Checked = true;
            this.enableAuthenticationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableAuthenticationCheckBox.Location = new System.Drawing.Point(87, 88);
            this.enableAuthenticationCheckBox.Name = "enableAuthenticationCheckBox";
            this.enableAuthenticationCheckBox.Size = new System.Drawing.Size(50, 17);
            this.enableAuthenticationCheckBox.TabIndex = 4;
            this.enableAuthenticationCheckBox.Text = "启动";
            this.enableAuthenticationCheckBox.UseVisualStyleBackColor = true;
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(13, 59);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(34, 13);
            this.PortLabel.TabIndex = 8;
            this.PortLabel.Text = "端口:";
            // 
            // AuthPortTextBox
            // 
            this.AuthPortTextBox.Location = new System.Drawing.Point(87, 59);
            this.AuthPortTextBox.Name = "AuthPortTextBox";
            this.AuthPortTextBox.Size = new System.Drawing.Size(121, 20);
            this.AuthPortTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "监听IP:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.clientListView);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(734, 401);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "客户端设定";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textBoxShareKey);
            this.panel2.Controls.Add(this.textBoxClientIp);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 334);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(728, 64);
            this.panel2.TabIndex = 8;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(395, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "删除";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(314, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Save_ClientItem);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(135, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "ShareKey";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "NAS:";
            // 
            // textBoxShareKey
            // 
            this.textBoxShareKey.Location = new System.Drawing.Point(187, 4);
            this.textBoxShareKey.Name = "textBoxShareKey";
            this.textBoxShareKey.Size = new System.Drawing.Size(100, 20);
            this.textBoxShareKey.TabIndex = 4;
            // 
            // textBoxClientIp
            // 
            this.textBoxClientIp.Location = new System.Drawing.Point(38, 4);
            this.textBoxClientIp.Name = "textBoxClientIp";
            this.textBoxClientIp.Size = new System.Drawing.Size(91, 20);
            this.textBoxClientIp.TabIndex = 3;
            // 
            // clientListView
            // 
            this.clientListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.clientListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientListView.Location = new System.Drawing.Point(3, 3);
            this.clientListView.Name = "clientListView";
            this.clientListView.Size = new System.Drawing.Size(728, 395);
            this.clientListView.TabIndex = 0;
            this.clientListView.UseCompatibleStateImageBehavior = false;
            this.clientListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "IP";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ShareKey";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(734, 401);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "验证";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBoxDomain);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.TextBoxLdapPath);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.enableLDAP);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(0, 136);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(734, 147);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "LDAP";
            // 
            // textBoxDomain
            // 
            this.textBoxDomain.Location = new System.Drawing.Point(85, 46);
            this.textBoxDomain.Name = "textBoxDomain";
            this.textBoxDomain.Size = new System.Drawing.Size(100, 20);
            this.textBoxDomain.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "DomainName";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(85, 103);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(374, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "例子:LDAP://yourserver/CN=username,CN=users,DC=yourdnsname,DC=CN ";
            // 
            // TextBoxLdapPath
            // 
            this.TextBoxLdapPath.Location = new System.Drawing.Point(85, 76);
            this.TextBoxLdapPath.Name = "TextBoxLdapPath";
            this.TextBoxLdapPath.Size = new System.Drawing.Size(615, 20);
            this.TextBoxLdapPath.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "LDAP服务器";
            // 
            // enableLDAP
            // 
            this.enableLDAP.AutoSize = true;
            this.enableLDAP.Location = new System.Drawing.Point(8, 19);
            this.enableLDAP.Name = "enableLDAP";
            this.enableLDAP.Size = new System.Drawing.Size(50, 17);
            this.enableLDAP.TabIndex = 0;
            this.enableLDAP.Text = "启动";
            this.enableLDAP.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.TextBoxSQL);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.TextBoxConnectionString);
            this.groupBox3.Controls.Add(this.enableDataBase);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(734, 136);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "数据库验证";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(90, 92);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(634, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "例子：select password from userTable where userTable.UserName=@userName,其中@userName是" +
                "用户名参数，名称不能刚换";
            // 
            // TextBoxSQL
            // 
            this.TextBoxSQL.Location = new System.Drawing.Point(93, 69);
            this.TextBoxSQL.Name = "TextBoxSQL";
            this.TextBoxSQL.Size = new System.Drawing.Size(607, 20);
            this.TextBoxSQL.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "SQL";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "连接字符串：";
            // 
            // TextBoxConnectionString
            // 
            this.TextBoxConnectionString.Location = new System.Drawing.Point(93, 42);
            this.TextBoxConnectionString.Name = "TextBoxConnectionString";
            this.TextBoxConnectionString.Size = new System.Drawing.Size(607, 20);
            this.TextBoxConnectionString.TabIndex = 1;
            // 
            // enableDataBase
            // 
            this.enableDataBase.AutoSize = true;
            this.enableDataBase.Location = new System.Drawing.Point(8, 19);
            this.enableDataBase.Name = "enableDataBase";
            this.enableDataBase.Size = new System.Drawing.Size(50, 17);
            this.enableDataBase.TabIndex = 0;
            this.enableDataBase.Text = "启动";
            this.enableDataBase.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(561, 9);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 25);
            this.button4.TabIndex = 11;
            this.button4.Text = "保存";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.SaveServerSetting_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 386);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(742, 41);
            this.panel1.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 427);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TinyRadius.Net Server";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView clientListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.TextBox AuthPortTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label AccountPortTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox AccountListentPort;
        private System.Windows.Forms.CheckBox enableAuthenticationCheckBox;
        private System.Windows.Forms.CheckBox EnableAccountCheckBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxShareKey;
        private System.Windows.Forms.TextBox textBoxClientIp;
        private System.Windows.Forms.ComboBox AuthListentIPTextBox;
        private System.Windows.Forms.ComboBox AccountListentIPTextBox;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox enableLDAP;
        private System.Windows.Forms.CheckBox enableDataBase;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TextBoxConnectionString;
        private System.Windows.Forms.TextBox TextBoxSQL;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TextBoxLdapPath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxDomain;
    }
}

