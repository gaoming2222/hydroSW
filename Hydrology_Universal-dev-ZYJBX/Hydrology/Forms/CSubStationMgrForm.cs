using System;
using System.Windows.Forms;
using Hydrology.CControls.CDataGridView;
using Hydrology.DataMgr;

namespace Hydrology.Forms
{
    public partial class CSubStationMgrForm : Form
    {
        private CDataGridViewSubCenter m_dgvSubCenter;

        public CSubStationMgrForm()
        {
            InitializeComponent();
            Init();
            FormHelper.InitUserModeEvent(this);
        }

        private void Init()
        {
            tableLayoutPanel.SuspendLayout();
            m_dgvSubCenter = new CDataGridViewSubCenter();
            m_dgvSubCenter.AllowUserToAddRows = false;
            m_dgvSubCenter.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //m_dgvRain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvSubCenter.Dock = DockStyle.Fill;
            //m_dgvUser.AutoSize = true;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvSubCenter.AllowUserToResizeRows = false;
            m_dgvSubCenter.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvSubCenter.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            m_dgvSubCenter.RowHeadersWidth = 50;
            m_dgvSubCenter.ColumnHeadersHeight = 25;
            m_dgvSubCenter.SubCenterCountChanged += (s, e) => 
            {
                labelUserCount.Text = String.Format("共 {0} 个分中心", e.Value);
            };

            tableLayoutPanel.Controls.Add(m_dgvSubCenter, 0, 1);
            tableLayoutPanel.ResumeLayout(false);

            m_dgvSubCenter.InitDataSource(CDBDataMgr.GetInstance().GetSubCenterProxy());
            m_dgvSubCenter.SetEditMode(true); //目前只支持编写了编辑模式
            m_dgvSubCenter.LoadData();
        }
        
        private void tsButSave_Click(object sender, EventArgs e)
        {
            m_dgvSubCenter.DoSave();
        }

        private void tsButRevert_Click(object sender, EventArgs e)
        {
            m_dgvSubCenter.Revert();
        }

        private void tsButAddUser_Click(object sender, EventArgs e)
        {
            m_dgvSubCenter.AddNew();
        }

        private void tsButDelete_Click(object sender, EventArgs e)
        {
            m_dgvSubCenter.DoDelete();
        }

        private void tsButExit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void CSubStationMgrForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 窗体关闭事件
            if (!m_dgvSubCenter.Close())
            {
                e.Cancel = true;
            }
        }
    }
}
