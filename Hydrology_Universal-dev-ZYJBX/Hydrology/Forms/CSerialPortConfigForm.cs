using System;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;

namespace Hydrology.Forms
{
    public partial class CSerialPortConfigForm : Form
    {
        private CDataGridViewSerialPort m_dgvPort;
        public CSerialPortConfigForm()
        {
            InitializeComponent();
            Init();
            FormHelper.InitUserModeEvent(this);
        }

        private void Init()
        {
            tableLayoutPanel.SuspendLayout();
            m_dgvPort = new CDataGridViewSerialPort();
            m_dgvPort.AllowUserToAddRows = false;
            m_dgvPort.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            m_dgvPort.Dock = DockStyle.Fill;
            m_dgvPort.AllowUserToResizeRows = false;
            m_dgvPort.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvPort.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            m_dgvPort.RowHeadersWidth = 50;
            m_dgvPort.ColumnHeadersHeight = 25;

            m_dgvPort.SetEditMode(true);
            m_dgvPort.SerialPortCountChanged += (s, e) =>
            {
                labelUserCount.Text = String.Format("共{0}个端口", e.Value);
            };

            tableLayoutPanel.Controls.Add(m_dgvPort, 0, 1);
            tableLayoutPanel.ResumeLayout(false);

            m_dgvPort.InitDataSource(CDBDataMgr.GetInstance().GetSerialPortProxy());

            if (!m_dgvPort.LoadData())
            {
                //加载失败
                //this.Close(); //关闭窗体
                // 抛出异常，结束显示
                throw new Exception("未配置通讯方式和数据协议，配置串口");
            }
        }

        private void tsButSave_Click(object sender, EventArgs e)
        {
            m_dgvPort.DoSave();
            this.Focus();
        }

        private void tsButRevert_Click(object sender, EventArgs e)
        {
            m_dgvPort.Revert();
        }

        private void tsButAddUser_Click(object sender, EventArgs e)
        {
            m_dgvPort.AddNewPort();
        }

        private void tsButDelete_Click(object sender, EventArgs e)
        {
            m_dgvPort.DoDelete();
        }

        private void tsButExit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void CSerialPortConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 窗体关闭事件，检测是否需要保存数据
            if (!m_dgvPort.Close())
            {
                e.Cancel = true;
            }
        }

        private void ClearGSM_Click(object sender, EventArgs e)
        {
            m_dgvPort.DoClearGSM();
            this.Focus();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            m_dgvPort.DoClearGSM();
            MessageBox.Show("已经清空部分短信，如果多次删除，请重启GSM口");
            this.Focus();
        }
    }
}
