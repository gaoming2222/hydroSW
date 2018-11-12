using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using System.Diagnostics;

namespace Hydrology.Forms
{
    public partial class CWarningInfoQuery : Form
    {
        private static readonly string CN_DataTime = "日期";
        private static readonly string CN_InfoDetail = "信息详情";
        private static readonly string CS_DateFormat = "yyyy-MM-dd HH:mm:ss";
        private static readonly string CN_InfoID = "id";
        private CExDataGridView m_dgvInfo;
        private IWarningInfoProxy m_proxyWarningInfo;
        public bool Editable
        {
            set { SetEditable(value); }
        }
        public CWarningInfoQuery()
        {
            InitializeComponent();
            InitUI();
            m_proxyWarningInfo = CDBDataMgr.Instance.GetWarningInfoProxy();
            SetEditable(false);
        }
        /// <summary>
        /// 默认不可编辑
        /// </summary>
        /// <param name="bEditable"></param>
        public void SetEditable(bool bEditable = false)
        {
            btnDelete.Visible = bEditable;
            if (bEditable)
            {
                // 如果是管理员的话
                FormHelper.InitUserModeEvent(this);
            }
        }
        #region 帮助方法
        private void InitUI()
        {
            // 不开启分页模式
            panelInfo.SuspendLayout();
            m_dgvInfo = new CExDataGridView() { BPartionPageEnable = false };
            m_dgvInfo.Header = new string[] { CN_DataTime, CN_InfoDetail, CN_InfoID };
            m_dgvInfo.HideColomns = new int[] { 2 }; // 隐藏ID列
            m_dgvInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //m_dgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dgvInfo.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvInfo.Dock = DockStyle.Fill;
            m_dgvInfo.AutoSize = false;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvInfo.AllowUserToResizeRows = false;
            m_dgvInfo.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvInfo.RowHeadersWidth = 50;
            m_dgvInfo.ColumnHeadersHeight = 25;
            m_dgvInfo.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            m_dgvInfo.AllowUserToAddRows = false;
            m_dgvInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //整行选中

            this.m_dgvInfo.SizeChanged += (s, e) =>
            {
                m_dgvInfo.Columns[0].Width = 130;
            };

            panelInfo.Controls.Add(m_dgvInfo);
            panelInfo.ResumeLayout(false);

            btnDelete.Enabled = false; //默认不可用


            // 设置日期
            dtp_StartTime.Format = DateTimePickerFormat.Custom;
            dtp_EndTime.Format = DateTimePickerFormat.Custom;
            dtp_StartTime.CustomFormat = "yyyy-MM-dd HH:mm";
            dtp_EndTime.CustomFormat = "yyyy-MM-dd HH:mm";

            TimeSpan span = new TimeSpan(1, 0, 0, 0);
            dtp_StartTime.Value = DateTime.Now.Subtract(span);
            dtp_EndTime.Value = DateTime.Now;



        }

        private void DoQuery()
        {
            tst_Label.Text = "正在查询...";
            this.Enabled = false;
            try
            {
                List<CEntityWarningInfo> results = m_proxyWarningInfo.QueryWarningInfo(dtp_StartTime.Value, dtp_EndTime.Value);
                if (null == results)
                {
                    // 查询失败
                    MessageBox.Show("数据库忙，请稍后再试");
                    this.Enabled = true;
                    return;
                }
                if (results.Count > 5000)
                {
                    //如果大于2000条，表格不支持，提示
                    MessageBox.Show("查询结果超过最大值,请缩小时间范围");
                    this.Enabled = true;
                    return;
                }
                m_dgvInfo.ClearAllRows();
                foreach (CEntityWarningInfo entity in results)
                {
                    m_dgvInfo.AddRow(new string[] { entity.DataTime.ToString(CS_DateFormat), entity.InfoDetail, entity.WarningInfoID.ToString() }, CExDataGridView.EDataState.ENormal);
                }
                m_dgvInfo.UpdateDataToUI();
                tst_Label.Text = string.Format("查询已成功完成，共{0}条警告", results.Count);
                if (results.Count > 0)
                {
                    btnDelete.Enabled = true; //可以删除
                }
                else
                {
                    btnDelete.Enabled = false; //不删除
                }
                this.Enabled = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("查询失败！" + e);
                MessageBox.Show("查询失败！");
            }
        }
        #endregion ///<帮助方法

        private void btnQuery_Click(object sender, EventArgs e)
        {
            // 判断起始时间是否小于结束时间
            if (dtp_StartTime.Value < dtp_EndTime.Value)
            {
                DoQuery();
            }
            else
            {
                MessageBox.Show("起始时间必须小于结束时间");
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(string.Format("是否删除从\"{0}\"到\"{1}\"时间段内的所有告警信息？",
                    dtp_StartTime.Value.ToString("yyyy-MM-dd HH:mm"),
                    dtp_EndTime.Value.ToString("yyyy-MM-dd HH:mm")), "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (m_proxyWarningInfo.DeleteRange(dtp_StartTime.Value, dtp_EndTime.Value))
                    {
                        MessageBox.Show("删除成功");
                    }
                    else
                    {
                        MessageBox.Show("删除失败");
                    }
                    this.DoQuery(); //重新查询
                }
                else
                {
                    // 不执行任何操作
                }
                //if (m_dgvInfo.CurrentRow != null)
                //{
                //    long id = long.Parse(m_dgvInfo.CurrentRow.Cells[CN_InfoID].Value.ToString());
                //    if (m_proxyWarningInfo.DeleteWarningInfo(id))
                //    {
                //        DoQuery();
                //        MessageBox.Show("删除成功");

                //    }
                //    else
                //    {
                //        MessageBox.Show("删除失败");
                //    }
                //}
                //else
                //{
                //    MessageBox.Show("请先选择一条记录，然后删除");
                //}
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
