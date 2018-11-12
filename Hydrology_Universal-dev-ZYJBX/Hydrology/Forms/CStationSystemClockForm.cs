using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using Hydrology.Entity;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

namespace Hydrology.Forms
{
    public partial class CStationSystemClockForm : Form
    {
        #region 静态常量
        private readonly string CS_All_Station = "所有站点";
        #endregion 静态常量
        #region 成员变量
        /// <summary>
        /// 系统对时的表格
        /// </summary>
        private CDataGridViewSystemClock m_dgvClock;

        private CEntityStation m_currentStation;
        #endregion 成员变量

        public CStationSystemClockForm()
        {
            InitializeComponent();
            InitUI();
            InitListView();
            InitDataSource();
            CProtocolEventManager.ErrorForUI += this.ErrorForUI_EventHandler;
            CProtocolEventManager.GPRS_TimeOut4UI += CProtocolEventManager_GPRS_TimeOut4UI;
            CProtocolEventManager.GPRS_OffLine4UI += new EventHandler(CProtocolEventManager_GPRS_OffLine4UI);
        }

        void CProtocolEventManager_GPRS_OffLine4UI(object sender, EventArgs e)
        {
            if (this.IsHandleCreated && null != m_currentStation)
            {
                AddLog(String.Format("站点{0}对时失败！", m_currentStation.StationName));
                m_dgvClock.UpdateStationStatus(m_currentStation.StationID, CDataGridViewSystemClock.EStationClockState.EAdjustFailed);
                m_dgvClock.UpdateDataToUI();
            }
        }
        private void InitListView()
        {
            // 初始化listView
            listView1.Columns.Add("表头", -2, HorizontalAlignment.Left);
            listView1.HeaderStyle = ColumnHeaderStyle.None;

            // 设置ListView的行高
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 12);//设置 ImageList 的宽和高
            listView1.SmallImageList = imgList;
            listView1.View = View.Details;
            listView1.Dock = DockStyle.Fill;
        }
        void CProtocolEventManager_GPRS_TimeOut4UI(object sender, ReceivedTimeOutEventArgs e)
        {
            if (this.IsHandleCreated && null != m_currentStation)
            {
                AddLog(String.Format("站点{0}对时失败！", m_currentStation.StationName));
                m_dgvClock.UpdateStationStatus(m_currentStation.StationID, CDataGridViewSystemClock.EStationClockState.EAdjustFailed);
                m_dgvClock.UpdateDataToUI();
            }
        }
        private void ErrorForUI_EventHandler(object sender, ReceiveErrorEventArgs e)
        {
            string msg = e.Msg;
            if (this.IsHandleCreated && null != this.m_currentStation)
            {
                try
                {
                    AddLog(msg);
                    if (msg.Contains("TRU"))
                    {
                        AddLog(String.Format("站点{0}对时成功！", m_currentStation.StationName));
                        m_dgvClock.UpdateStationStatus(m_currentStation.StationID, CDataGridViewSystemClock.EStationClockState.EAjustSuccess);
                    }
                    m_dgvClock.UpdateDataToUI();
                }
                catch (Exception exp) { Debug.WriteLine(exp.Message); }
            }
        }

        #region 事件响应

        private void EHSubCenterChanged(object sender, EventArgs e)
        {
            int selectindex = cmb_SubCenter.SelectedIndex;
            if (0 == selectindex)
            {
                m_dgvClock.SetSubCenterName(null); //所有分中心
            }
            else
            {
                string subcentername = cmb_SubCenter.Text;
                m_dgvClock.SetSubCenterName(subcentername);
            }
            this.tst_Label.Text = string.Format("共{0}个站点", m_dgvClock.Rows.Count);
        }

        private void EHBtnExit(object sender, EventArgs e)
        {
            this.Close();
        }
        private void EHCheckAllChanged(object sender, EventArgs e)
        {
            if (chk_All.CheckState == CheckState.Checked)
            {
                // 全选
                m_dgvClock.SelectAllOrNot(true);

            }
            else
            {
                // 全不选
                m_dgvClock.SelectAllOrNot(false);
            }
        }

        #endregion 事件响应

        #region 帮助方法
        /// <summary>
        /// 初始化列表
        /// </summary>
        private void InitUI()
        {
            this.SuspendLayout();

            m_dgvClock = new CDataGridViewSystemClock() { BPartionPageEnable = false };
            m_dgvClock.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //m_dgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dgvInfo.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvClock.Dock = DockStyle.Fill;
            m_dgvClock.AutoSize = false;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvClock.AllowUserToResizeRows = false;
            m_dgvClock.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvClock.RowHeadersWidth = 50;
            m_dgvClock.ColumnHeadersHeight = 25;
            m_dgvClock.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            m_dgvClock.AllowUserToAddRows = false;
            panelBottom.Controls.Add(m_dgvClock);

            // 绑定消息
            this.btn_Exit.Click += new EventHandler(EHBtnExit);
            this.cmb_SubCenter.SelectedIndexChanged += new EventHandler(EHSubCenterChanged);
            this.chk_All.CheckedChanged += new EventHandler(EHCheckAllChanged);
            this.FormClosing += new FormClosingEventHandler(EHFormClosing);

            this.ResumeLayout(false);
        }

        private void EHFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!btn_StartAdjust.Enabled)
            {
                if (MessageBox.Show("系统正在对时，是否强行退出?", "关闭", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    // this.Close();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitDataSource()
        {
            List<CEntitySubCenter> listSubCenter = CDBDataMgr.Instance.GetAllSubCenter();
            cmb_SubCenter.Items.Add(CS_All_Station);
            for (int i = 0; i < listSubCenter.Count; ++i)
            {
                cmb_SubCenter.Items.Add(listSubCenter[i].SubCenterName);
            }
        }
        #endregion 帮助方法

        private void btn_StartAdjust_Click(object sender, EventArgs e)
        {
            var listStation = this.m_dgvClock.GetAllSelectedStation();
            if (0 == listStation.Count)
            {
                MessageBox.Show("请选择站点！");
                return;
            }
            this.btn_StartAdjust.Enabled = false;
            int i = 0;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer()
            {
                Interval = 40000
            };
            AddLog("系统正在对时，请耐心等待！");
            timer.Tick += (s1, e1) =>
            {
                if (i >= listStation.Count)
                {
                    timer.Stop();
                    m_currentStation = null;
                    this.btn_StartAdjust.Enabled = true;
                    return;
                }

                m_currentStation = listStation[i];
                Debug.WriteLine(" i = " + i + "      Name = " + m_currentStation.StationName);
                string sendMsg = CPortDataMgr.Instance.SendAdjustClock(m_currentStation);
                AddLog(sendMsg);
                i++;
            };
            timer.Start();
        }

        private void AddLog(string text)
        {
            if (this.IsHandleCreated)
            {
                this.listView1.Invoke((Action)delegate
                    {
                        try
                        {
                            this.listView1.Items.Add(text);
                            int index = this.listView1.Items.Count - 1;
                            if (index >= 0)
                            {
                                this.listView1.Items[index].EnsureVisible();
                            }
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    });
            }
        }

    }
}
