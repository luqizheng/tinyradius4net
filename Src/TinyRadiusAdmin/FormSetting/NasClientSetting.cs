using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using TinyRadius.Net.Cfg;

namespace TinyRadiusAdmin.FormSetting
{
    public partial class NasClientSetting : UserControl
    {
        private Dictionary<string, NasSetting> _datasource;
        public NasClientSetting()
        {
            InitializeComponent();
        }
        public Dictionary<string, NasSetting> DataSource
        {
            set
            {
                this.dataGridView1.DataSource = value != null ? new List<NasSetting>(value.Values) : null;
                _datasource = value;

            }
            get
            {
                return _datasource;
            }


        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "IpColumn")
            {
                if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    dataGridView1.Rows[e.RowIndex].ErrorText = "请输入IP地址.";
                    MessageBox.Show(dataGridView1.Rows[e.RowIndex].ErrorText);
                    e.Cancel = true;
                    return;
                }
                IPAddress ip;
                if (!IPAddress.TryParse(e.FormattedValue.ToString(), out ip))
                {
                    dataGridView1.Rows[e.RowIndex].ErrorText = "请输入正确的IP地址，如192.168.1.1";
                    MessageBox.Show(dataGridView1.Rows[e.RowIndex].ErrorText);
                    e.Cancel = true;
                }
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "SecretKeyColumn")
            {
                if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    dataGridView1.Rows[e.RowIndex].ErrorText = "请输入SecretKey.";
                    MessageBox.Show(dataGridView1.Rows[e.RowIndex].ErrorText);
                    e.Cancel = true;
                    return;
                }
            }

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.   
            dataGridView1.Rows[e.RowIndex].ErrorText = String.Empty;
            if (dataGridView1.Columns[e.ColumnIndex].Name == "IpColumn")
            {
                try
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value =
                        IPAddress.Parse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                }
                catch
                {

                }
            }


        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count != 0)
            {
                var row = this.dataGridView1.SelectedRows[0];
                string ip = row.Cells[0].Value.ToString();
                DeleteClient(ip);
            }
        }

        private void DeleteClient(string ip)
        {
            if (MessageBox.Show(String.Format("确认要删除客户端{0}吗?", ip), "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this._datasource.Remove(ip);
                this.DataSource = _datasource;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(ipTextBox.Text, out ip))
            {
                MessageBox.Show("请输入正确的IP地址，如192.168.1.1");
                return;
            }

            if (String.IsNullOrEmpty(this.SecretKeyTextBokx.Text))
            {
                MessageBox.Show("请输入SecretKey");
                return;
            }
            var newData = new NasSetting() { Ip = ip, SecretKey = this.SecretKeyTextBokx.Text };

            this.dataGridView1.DataSource = null;
            this._datasource.Add(newData.Ip.ToString(), newData);
            this.DataSource = this._datasource;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            this.ButtonDelete.Enabled = this.contextMenuStrip1.Enabled = this.dataGridView1.SelectedRows.Count != 0;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ip = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            DeleteClient(ip);
        }
    }
}
