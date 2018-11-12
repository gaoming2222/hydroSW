using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.Entity;
using Hydrology.DataMgr;

namespace Hydrology.Forms
{
    public partial class FlowRateReport : Form
    {
        #region 静态常量
        private static readonly string CS_ReportType_Day = "日报表";
        private static readonly string CS_ReportType_Month = "月报表";
        private static readonly string CS_Subcenter_All = "所有分中心";
        #endregion 静态常量

        #region 数据成员
        /// <summary>
        /// 日畅通率表格控件
        /// </summary>
        private CDataGridViewDayReport m_dgvDayReport;

        /// <summary>
        /// 月畅通率表格控件
        /// </summary>
        private CDataGridViewMonthReport m_dgvMonthReport;
        #endregion 数据成员
        public FlowRateReport()
        {
            InitializeComponent();
            InitUI();
        }
        private void InitUI()
        {
            this.SuspendLayout();

            m_dgvDayReport = new CDataGridViewDayReport();
            m_dgvDayReport.AllowUserToAddRows = false;

            m_dgvDayReport.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            //m_dgvRain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvDayReport.Dock = DockStyle.Fill;
            //m_dgvDayReport.AutoSize = false;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvDayReport.AllowUserToResizeRows = false;
            m_dgvDayReport.AllowUserToResizeColumns = true;
            m_dgvDayReport.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvDayReport.RowHeadersWidth = 50;
            m_dgvDayReport.ColumnHeadersHeight = 60;
            m_dgvDayReport.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            m_dgvMonthReport = new CDataGridViewMonthReport();
            m_dgvMonthReport.AllowUserToAddRows = false;
            m_dgvMonthReport.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //m_dgvRain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvMonthReport.Dock = DockStyle.Fill;
            //m_dgvDayReport.AutoSize = false;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvMonthReport.AllowUserToResizeRows = false;
            m_dgvMonthReport.AllowUserToResizeColumns = true;
            m_dgvMonthReport.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvMonthReport.RowHeadersWidth = 50;
            m_dgvMonthReport.ColumnHeadersHeight = 60;
            m_dgvMonthReport.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            this.panel2.Controls.Add(m_dgvDayReport);
            this.panel2.Controls.Add(m_dgvMonthReport);
            m_dgvMonthReport.Visible = false;

            // 初始化分中心
            List<CEntitySubCenter> subcenter = CDBDataMgr.Instance.GetAllSubCenter();
            comboBox1.Items.Add(CS_Subcenter_All);
            for (int i = 0; i < subcenter.Count; ++i)
            {
                comboBox1.Items.Add(subcenter[i].SubCenterName);
            }
            this.comboBox1.SelectedIndex = 0;
            // 初始化报表类型
            //cmbReportType.Items.Add(CS_ReportType_Day);
            //cmbReportType.Items.Add(CS_ReportType_Month);

            // 状态栏字隐藏
            //m_statusLable.Visible = false;

            // 初始化日期
            DateTimer.Value = DateTime.Now;
            this.ResumeLayout(false);
        }
        private void btnQuery_Click(object sender, EventArgs e)
        {
            // 判断分中心是否为空
            if (comboBox1.Text.Equals(""))
            {
                MessageBox.Show("请选择分中心");
                return;
            }
            string subcenterName = comboBox1.Text;
            if (comboBox1.Text == CS_Subcenter_All)
            {
                subcenterName = null;
            }
            // 判断报表类型是否为空
            if (TabelType.Text.Equals(""))
            {
                MessageBox.Show("请选择报表类型");
                return;
            }
            CMessageBox box = new CMessageBox();
            box.MessageInfo = "正在查询数据库";
            box.ShowDialog(this);
            this.Enabled = false;
            if (TabelType.Text.Equals(CS_ReportType_Day))
            {
                // 日报表
                m_dgvDayReport.SetFilter(subcenterName, DateTimer.Value);
                //lblToolTip.Text = string.Format("畅通率统计  低于95%: {0}个，满足95%：{1}个，共{2}个",
                //    m_dgvDayReport.TotalCount - m_dgvDayReport.MoreThan95Count,
                //    m_dgvDayReport.MoreThan95Count,
                //    m_dgvDayReport.TotalCount);
            }
            else
            {
                // 月报表
                m_dgvMonthReport.SetFilter(subcenterName, DateTimer.Value);
                //lblToolTip.Text = string.Format("畅通率统计  低于95%: {0}个，满足95%：{1}个，共{2}个",
                //    m_dgvMonthReport.TotalCount - m_dgvMonthReport.MoreThan95Count,
                //    m_dgvMonthReport.MoreThan95Count,
                //    m_dgvMonthReport.TotalCount);
            }
            this.Enabled = true;
            box.CloseDialog();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            // 导出到Excel文件
            DateTime dt = new DateTime();
            dt = DateTimer.Value;
            if (m_dgvDayReport.Visible)
            {
                //m_dgvDayReport.ExportToExcel(dt);
                m_dgvDayReport.ExportToExcelNew(m_dgvDayReport, dt);

            }
            else if (m_dgvMonthReport.Visible)
            {
                //m_dgvMonthReport.ExportToExcel(dt);
                m_dgvMonthReport.ExportToExcelNew(m_dgvMonthReport, dt);
            }
            this.Focus();
        }


        private void TabelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 选择的报表类型发生了改变，相应的时间显示格式也需要改变
            if (TabelType.Text == CS_ReportType_Day)
            {
                DateTimer.CustomFormat = "yyyy年MM月dd日";
                m_dgvDayReport.Visible = true;
                m_dgvMonthReport.Visible = false;
            }
            else if (TabelType.Text == CS_ReportType_Month)
            {
                DateTimer.CustomFormat = "yyyy年MM月";
                m_dgvDayReport.Visible = false;
                m_dgvMonthReport.Visible = true;
            }

        }

    }
}
