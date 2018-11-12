using System;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using Hydrology.Entity;

namespace Hydrology.Forms
{
    public partial class CUserMgrForm : Form
    {
        #region 成员变量

        private CDataGridViewUser m_dgvUser;

        #endregion ///<DATA_MEMBER
        public CUserMgrForm()
        {
            InitializeComponent();
            InitUI();
            InitDataSource();

            // 绑定消息
            this.FormClosing += new FormClosingEventHandler(this.EH_FormClosing);

            FormHelper.InitUserModeEvent(this);
        }

        private void CUserMgrForm_Load(object sender, EventArgs e)
        {
            // 加载数据
            m_dgvUser.LoadData();
        }

        private void tsButSave_Click(object sender, EventArgs e)
        {
            m_dgvUser.DoSave();
        }

        private void tsButRevert_Click(object sender, EventArgs e)
        {
            m_dgvUser.Revert();
        }

        private void tsButAddUser_Click(object sender, EventArgs e)
        {
            m_dgvUser.AddNewUser();
        }

        private void tsButDelete_Click(object sender, EventArgs e)
        {
            m_dgvUser.DoDelete();
        }

        private void tsButExit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        #region 事件处理

        private void EH_UserCountChanged(object sender, CEventSingleArgs<int> e)
        {
            labelUserCount.Text = String.Format("共{0}个用户", e.Value);
        }

        private void EH_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_dgvUser.Close())
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region 帮助方法
        private void InitUI()
        {
            tableLayoutPanel.SuspendLayout();
            m_dgvUser = new CDataGridViewUser();
            m_dgvUser.AllowUserToAddRows = false;
            m_dgvUser.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //m_dgvRain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvUser.Dock = DockStyle.Fill;
            //m_dgvUser.AutoSize = true;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvUser.AllowUserToResizeRows = false;
            m_dgvUser.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvUser.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            m_dgvUser.RowHeadersWidth = 50;
            m_dgvUser.ColumnHeadersHeight = 25;

            m_dgvUser.SetEditMode(true); //目前只支持编写了编辑模式
            m_dgvUser.UserCountChanged += new EventHandler<CEventSingleArgs<int>>(this.EH_UserCountChanged);

            tableLayoutPanel.Controls.Add(m_dgvUser, 0, 1);
            tableLayoutPanel.ResumeLayout(false);
        }

        // 初始化数据
        public void InitDataSource()
        {
            m_dgvUser.InitDataSource(CDBDataMgr.GetInstance().GetUserProxy());
        }
        #endregion ///<帮助方法


    }
}
