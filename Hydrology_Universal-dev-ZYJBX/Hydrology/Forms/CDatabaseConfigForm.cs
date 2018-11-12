using System;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.DBManager;
using System.Collections.Generic;

namespace Hydrology.Forms
{
    public partial class CDatabaseConfigForm : Form
    {
        #region 成员变量
        private bool m_bIsAdvanceMode;

        // 用来保存之前的备份
        private string m_strDataSource;
        private string m_strDatabaseName;
        private string m_strUserName;
        private string m_strPassword;

        #endregion
        public CDatabaseConfigForm()
        {
            InitializeComponent();
            InitUI();
            InAdvanceMode(m_bIsAdvanceMode);
            this.cmb_DatabaseType.SelectedIndex = 0;
            m_bIsAdvanceMode = false;
            FormHelper.InitUserModeEvent(this);
            this.chkChangeDBConfig.Checked = false;
            this.chkChangeDBConfig_CheckedChanged(this.chkChangeDBConfig, null);
        }

        //private void btn_Advance_Click(object sender, EventArgs e)
        //{
        //    int flag = this.cmb_DatabaseType.SelectedIndex;
        //    string type = this.cmb_DatabaseType.Items[flag].ToString();
        //  //  string type = this.cmb_DatabaseType.GetItemText(flag);
        //    string source = this.textBox_DataSource.Text;
        //    string sqlname = this.textBox_DataBaseName.Text;
        //    string userName = this.textBox_UserName.Text;
        //    string pass = this.textBox_Password.Text;
        //    string target = "";
        //    if(type.Equals("SQL SERVER")){
        //        target = "Data Source=" + type + ";Initial Catalog=" + sqlname + ";Persist Security Info=True;User ID=" + userName + ";Password=" +
        //            pass + ";Connect Timeout=30";
        //        this.textBox_ConnectionString.Text = target;
        //    }
        //    if (type.Equals("ORACLE"))
        //    {

        //    }
        //    if (type.Equals("DB2"))
        //    {

        //    }
        //    if (type.Equals("ACCESS"))
        //    {

        //    }
        //    if (type.Equals("POSTGRESQL"))
        //    {

        //    }
        //    if (type.Equals("MySQL"))
        //    {

        //    }
        //    m_bIsAdvanceMode = !m_bIsAdvanceMode;
        //    InAdvanceMode(m_bIsAdvanceMode);
        //}

        private void btn_Advance_Click(object sender, EventArgs e)
        {
            if (m_bIsAdvanceMode)
            {
                //Data Source/=/127.0.0.1/;/Initial Catalog/=/HydrologyPureDB/;Persist Security Info=True;User ID=admin;Password=123456;Connect Timeout=30
                string str = textBox_ConnectionString.Text;
                string[] tmp = str.Split('=');
                List<string> result = new List<string>();

                for (int i = 1; i < tmp.Length; i++)
                {
                    string[] tmp1 = tmp[i].Split(';');
                    result.Add(tmp1[0]);
                }
                string source1 = result[0];
                string sqlName1 = result[1];
                string userName1 = result[3];
                string pass1 = result[4];
                this.textBox_DataSource.Text = source1;
                this.textBox_DataBaseName.Text = sqlName1;
                this.textBox_UserName.Text = userName1;
                this.textBox_Password.Text = pass1;


                //m_bIsAdvanceMode = !m_bIsAdvanceMode;
                //InAdvanceMode(m_bIsAdvanceMode);
            }
            if (!m_bIsAdvanceMode)
            {
                int flag = this.cmb_DatabaseType.SelectedIndex;
                string type = this.cmb_DatabaseType.Items[flag].ToString();
                //  string type = this.cmb_DatabaseType.GetItemText(flag);
                string source = this.textBox_DataSource.Text;
                string sqlname = this.textBox_DataBaseName.Text;
                string userName = this.textBox_UserName.Text;
                string pass = this.textBox_Password.Text;
                string target = "";
                if (type.Equals("SQL SERVER"))
                {
                    target = "Data Source=" + source + ";Initial Catalog=" + sqlname + ";Persist Security Info=True;User ID=" + userName + ";Password=" +
                        pass + ";Connect Timeout=30";
                    this.textBox_ConnectionString.Text = target;
                }
                if (type.Equals("ORACLE"))
                {

                }
                if (type.Equals("DB2"))
                {

                }
                if (type.Equals("ACCESS"))
                {

                }
                if (type.Equals("POSTGRESQL"))
                {

                }
                if (type.Equals("MySQL"))
                {

                }

            }
            m_bIsAdvanceMode = !m_bIsAdvanceMode;
            InAdvanceMode(m_bIsAdvanceMode);

        }


        //private void btn_Okay_Click(object sender, EventArgs e)
        //{
        //    CDBManager.Instance.DataSource = textBox_DataSource.Text.Trim();
        //    CDBManager.Instance.DataBaseName = textBox_DataBaseName.Text.Trim();
        //    CDBManager.Instance.UserName = textBox_UserName.Text.Trim();
        //    CDBManager.Instance.Password = textBox_Password.Text.Trim();
        //    base.Close();
        //}

        private void btn_Okay_Click(object sender, EventArgs e)
        {
            if (m_bIsAdvanceMode)
            {
                //Data Source/=/127.0.0.1/;/Initial Catalog/=/HydrologyPureDB/;Persist Security Info=True;User ID=admin;Password=123456;Connect Timeout=30
                string str = textBox_ConnectionString.Text;
                string[] tmp = str.Split('=');
                List<string> result = new List<string>();

                for (int i = 1; i < tmp.Length; i++)
                {
                    string[] tmp1 = tmp[i].Split(';');
                    result.Add(tmp1[0]);
                }
                string source1 = result[0];
                string sqlName1 = result[1];
                string userName1 = result[3];
                string pass1 = result[4];
                this.textBox_DataSource.Text = source1;
                this.textBox_DataBaseName.Text = sqlName1;
                this.textBox_UserName.Text = userName1;
                this.textBox_Password.Text = pass1;


                //m_bIsAdvanceMode = !m_bIsAdvanceMode;
                //InAdvanceMode(m_bIsAdvanceMode);
            }
            CDBManager.Instance.DataSource = textBox_DataSource.Text.Trim();
            CDBManager.Instance.DataBaseName = textBox_DataBaseName.Text.Trim();
            CDBManager.Instance.UserName = textBox_UserName.Text.Trim();
            CDBManager.Instance.Password = textBox_Password.Text.Trim();
            base.Close();
        }


        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            CDBManager.Instance.DataSource = m_strDataSource;
            CDBManager.Instance.DataBaseName = m_strDatabaseName;
            CDBManager.Instance.UserName = m_strUserName;
            CDBManager.Instance.Password = m_strPassword;
            base.Close();
        }

        private void btn_ConnTest_Click(object sender, EventArgs e)
        {
            // 去掉空格
            CDBManager.Instance.DataSource = textBox_DataSource.Text.Trim();
            CDBManager.Instance.DataBaseName = textBox_DataBaseName.Text.Trim();
            CDBManager.Instance.UserName = textBox_UserName.Text.Trim();
            CDBManager.Instance.Password = textBox_Password.Text.Trim();
            if (CDBManager.Instance.TryToConnection())
            {
                MessageBox.Show("连接成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("连接失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #region 帮助方法

        private void InitUI()
        {
            cmb_DatabaseType.Items.AddRange(new string[] { "SQL SERVER", "DB2", "Oracle", "Access", "PostgreSQL", "MySQL" });

            //显示当前的配置
            m_strDataSource = CDBManager.Instance.DataSource;
            textBox_DataSource.Text = CDBManager.Instance.DataSource;

            m_strDatabaseName = CDBManager.Instance.DataBaseName;
            textBox_DataBaseName.Text = CDBManager.Instance.DataBaseName;

            m_strUserName = CDBManager.Instance.UserName;
            textBox_UserName.Text = CDBManager.Instance.UserName;

            m_strPassword = CDBManager.Instance.Password;
            textBox_Password.Text = CDBManager.Instance.Password;

            // 不显示记住密码
            chb_RembmerPassword.Visible = false;
            btn_Advance.Enabled = false; //禁用高级按钮选项
        }

        private void InAdvanceMode(bool bIsAdvance)
        {
            if (bIsAdvance)
            {
                // 高级模式
             //   lbl_DatasourceName.Text = "数据库类型：";
                lbl_DBName.Visible = false;
                lbl_DatasourceName.Visible = false;
                lbl_Password.Visible = false;
                lbl_UserName.Visible = false;
                textBox_DataSource.Visible = false;
                textBox_DataBaseName.Visible = false;
                textBox_Password.Visible = false;
                textBox_UserName.Visible = false;
                chb_RembmerPassword.Visible = false;
            //    textBox_ConnectionString.Visible = true;
                textBox_ConnectionString.Visible = true;
                ConnectString.Visible = true;
                cmb_DatabaseType.Visible = true;
                btn_Advance.Text = "高级 <<";
            }
            else
            {

                // 普通SQL SERVER模式
                lbl_DatasourceName.Text = "数据源名：";
                lbl_DBName.Text = "数据库名：";
                ConnectString.Visible = false;
                textBox_ConnectionString.Visible = false;
                lbl_DBName.Visible = true;
                lbl_DatasourceName.Visible = true;
                lbl_Password.Visible = true;
                lbl_UserName.Visible = true;
                textBox_DataSource.Visible = true;
                textBox_DataBaseName.Visible = true;
                textBox_Password.Visible = true;
                textBox_UserName.Visible = true;
                //chb_RembmerPassword.Visible = true;
                cmb_DatabaseType.Visible = true;
                btn_Advance.Text = "高级 >>";
            }
        }

        #endregion

        private void CDatabaseConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 判断当前配置是否已经配置成功
            if (CDBManager.Instance.TryToConnection())
            {
                // 继续关闭窗口
                // 判断数据库是否已经切换，如果是的话，需要重新加载界面和生成数据
                if (m_strDatabaseName != CDBManager.Instance.DataBaseName || m_strDataSource != CDBManager.Instance.DataSource)
                {
                    // 显示提示页面
                    CMessageBox box = new CMessageBox() { MessageInfo = "正在切换数据库，请耐心等待" };
                    box.ShowDialog(this);
                    // 先更新分中心
                    //CDBDataMgr.Instance.UpdateAllSubCenter(); 
                    // 再更新站点信息
                    //CDBDataMgr.Instance.UpdateAllStation();
                    CDBSoilDataMgr.Instance.ReloadDatabase();
                    CDBDataMgr.Instance.ReloadDatabase();
                   
                    //box.Invoke((Action)delegate { box.Close(); });
                    box.CloseDialog();

                    // 写入日志文件
                    CSystemInfoMgr.Instance.AddInfo(string.Format("切换数据库,从 {0}:{1} 切换到 {2}:{3} ",
                        m_strDataSource, m_strDatabaseName, CDBManager.Instance.DataSource, CDBManager.Instance.DataBaseName));
                }

            }
            else
            {
                // 当前配置不正确，是否继续退出
                if (MessageBox.Show("数据库配置失败，是否仍然退出配置？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void chkChangeDBConfig_CheckedChanged(object sender, EventArgs e)
        {
            var cmb = sender as CheckBox;
            if (null != cmb)
            {
                var isChecked = cmb.Checked;
                this.cmb_DatabaseType.SelectedIndex = 0;
                this.cmb_DatabaseType.Enabled = isChecked;
                this.textBox_DataBaseName.Enabled = isChecked;
                this.textBox_DataSource.Enabled = isChecked;
                this.textBox_UserName.Enabled = isChecked;
                this.textBox_Password.Enabled = isChecked;

                this.btn_ConnTest.Enabled = isChecked;
                this.btn_Okay.Enabled = isChecked;
                //this.btn_Cancel.Enabled = isChecked;
                this.btn_Advance.Enabled = isChecked;
            }
        }

     
    }
}
