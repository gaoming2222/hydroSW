using System;
using System.Collections.Generic;
using System.Drawing;
using Hydrology.DBManager;
using Hydrology.DBManager.DB.SQLServer;
using Hydrology.DBManager.Interface;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using Hydrology.Entity;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Hydrology.Forms
{
    public partial class DebugInfoSearch : Form
    {

        #region 静态常量
        private List<CEntityStation> m_listStations; //所有水情站点的引用
        private List<CEntitySoilStation> m_listSoilStations;//所有墒情站点的引用
        private readonly string CS_All_Station = "所有站点";
        private static readonly string CS_CMB_AllData = "全部数据";
        private static readonly string CS_CMB_TimeData = "整点数据";
        #endregion ///<静态常量
        public ListBox m_listBoxStation;
        private CDataGridViewTSRain m_dgvTSRain;        //自定义雨量数据表对象
        private CDataGridViewTSWater m_dgvTSWater;      //自定义水量数据表对象
        private CDataGridViewTSVoltage m_dgvTSVoltage;  //自定义电压数据表对象
        private ITSRainProxy m_proxyTSRain;
        private ITSWater m_proxyTSWater;
        private ITSVoltage m_proxyTSVoltage;
        public DebugInfoSearch()
        {
            m_proxyTSRain = new CDBSQLTSRain();
            m_proxyTSWater = new CDBSQLTSWater();
            m_proxyTSVoltage = new CDBSQLTSVoltage();
            InitializeComponent();
            InitUI();
            InitDB();
            ts_query_SelectedIndexChanged(null, null);

        }
        private void InitDB()
        {
            List<CEntitySubCenter> listSubCenter = CDBDataMgr.Instance.GetAllSubCenter();
            m_listStations = CDBDataMgr.Instance.GetAllStation();
            m_listSoilStations = CDBSoilDataMgr.Instance.GetAllSoilStation();
            ts_subcenter.Items.Add(CS_All_Station);
            for (int i = 0; i < listSubCenter.Count; ++i)
            {
                ts_subcenter.Items.Add(listSubCenter[i].SubCenterName);
            }
            this.ts_subcenter.SelectedIndex = 0;
        }
        private void InitUI()
        {
            this.SuspendLayout();
            cmb_TimeSelect.Items.AddRange(new string[] { CS_CMB_AllData, CS_CMB_TimeData });
            // 设置日期
            this.dtpTimeStart.Format = DateTimePickerFormat.Custom;
            this.dptTimeEnd.Format = DateTimePickerFormat.Custom;
            dateTimeStart.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimeEnd.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            TimeSpan span = new TimeSpan(1, 0, 0, 0);
            DateTime now = DateTime.Now;
            dateTimeEnd.Value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            dateTimeStart.Value = dateTimeEnd.Value.Subtract(span);// 减少一天
            this.ts_subcenter.SelectedIndexChanged += new EventHandler(EHSubCenterChanged);


            this.panelLeft.Controls.Remove(this.cmbStation);
            // this.cmbStation = new CStationComboBox();
            this.cmbStation = new CStationComboBox_1();
            this.cmbStation.FormattingEnabled = true;
            this.cmbStation.Location = new System.Drawing.Point(80, 96);
            this.cmbStation.Name = "cmbStation";
            this.cmbStation.Size = new System.Drawing.Size(117, 20);
            this.cmbStation.TabIndex = 1;
            this.panel5.Controls.Add(this.cmbStation);


            cmb_TimeSelect.SelectedIndex = 0;

            m_dgvTSRain = new CDataGridViewTSRain();
            m_dgvTSRain.AllowUserToAddRows = false;
            m_dgvTSRain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //m_dgvRain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvTSRain.Dock = DockStyle.Fill;
            m_dgvTSRain.AutoSize = true;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvTSRain.AllowUserToResizeRows = false;
            m_dgvTSRain.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvTSRain.RowHeadersWidth = 50;
            m_dgvTSRain.ColumnHeadersHeight = 25;
            m_dgvTSRain.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            //m_dgvRain.Dock = DockStyle.Fill;

            // 初始化水量查询数据表
            m_dgvTSWater = new CDataGridViewTSWater();
            m_dgvTSWater.AllowUserToAddRows = false;
            m_dgvTSWater.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //m_dgvRain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvTSWater.Dock = DockStyle.Fill;
            m_dgvTSWater.AutoSize = true;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvTSWater.AllowUserToResizeRows = false;
            m_dgvTSWater.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvTSWater.RowHeadersWidth = 50;
            m_dgvTSWater.ColumnHeadersHeight = 25;
            m_dgvTSWater.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            // 初始化电压查询数据表
            m_dgvTSVoltage = new CDataGridViewTSVoltage();
            m_dgvTSVoltage.AllowUserToAddRows = false;
            m_dgvTSVoltage.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //m_dgvVoltage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dgvVoltage.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvTSVoltage.Dock = DockStyle.Fill;
            m_dgvTSWater.AutoSize = true;
            //m_dgvVoltage.ReadOnly = true; //只读
            m_dgvTSVoltage.AllowUserToResizeRows = false;
            m_dgvTSVoltage.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvTSVoltage.RowHeadersWidth = 50;
            m_dgvTSVoltage.ColumnHeadersHeight = 25;
            m_dgvTSVoltage.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            tLayoutRight.SuspendLayout();
            tLayoutRight.Controls.Add(m_dgvTSRain, 0, 0);
            tLayoutRight.Controls.Add(m_dgvTSWater, 0, 0);
            tLayoutRight.Controls.Add(m_dgvTSVoltage, 0, 0);


            tLayoutRight.ResumeLayout(false);

            this.ts_query.SelectedIndex = 0;
            m_dgvTSRain.Show();
            m_dgvTSWater.Hide();
            m_dgvTSVoltage.Hide();

        }
        private void EHSubCenterChanged(object sender, EventArgs e)
        {
            this.cmbStation.Text = "";
            int selectindex = ts_subcenter.SelectedIndex;
            if (0 == selectindex)
            {
                this.cmbStation.m_listBoxStation.Items.Clear();
                // 根据分中心查找测站
                m_listStations = CDBDataMgr.Instance.GetAllStation();
                m_listSoilStations = CDBSoilDataMgr.Instance.GetAllSoilStation();

                for (int i = 0; i < m_listStations.Count; ++i)
                {
                    this.cmbStation.m_listBoxStation.Items.Add(string.Format("({0,-4}|{1})", m_listStations[i].StationID, m_listStations[i].StationName));
                }
                for (int i = 0; i < m_listSoilStations.Count; ++i)
                {
                    this.cmbStation.m_listBoxStation.Items.Add(string.Format("({0,-4}|{1})", m_listSoilStations[i].StationID, m_listSoilStations[i].StationName));
                }

            }
            else
            {
                string subcentername = ts_subcenter.Text;
                this.cmbStation.m_listBoxStation.Items.Clear();


                m_listStations = CDBDataMgr.Instance.GetAllStation();
                m_listSoilStations = CDBSoilDataMgr.Instance.GetAllSoilStation();
                CEntitySubCenter subcenter = CDBDataMgr.Instance.GetSubCenterByName(subcentername);
                if (null != subcenter)
                {
                    // 如果不为空           
                    for (int i = 0; i < m_listStations.Count; ++i)
                    {
                        if (m_listStations[i].SubCenterID == subcenter.SubCenterID)
                        {

                            this.cmbStation.m_listBoxStation.Items.Add(string.Format("({0,-4}|{1})", m_listStations[i].StationID, m_listStations[i].StationName));
                        }
                    }
                    for (int i = 0; i < m_listSoilStations.Count; ++i)
                    {
                        if (m_listSoilStations[i].SubCenterID == subcenter.SubCenterID)
                        {
                            this.cmbStation.m_listBoxStation.Items.Add(string.Format("({0,-4}|{1})", m_listSoilStations[i].StationID, m_listSoilStations[i].StationName));
                        }
                    }
                    //      this.cmbStation.Text = this.cmbStation.m_listBoxStation.Items[0].ToString();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("分中心 Error");
                }
            }
        }

        private void btn_Query_Click(object sender, EventArgs e)
        {
            // 获取站点ID
            // 获取起始日期
            // 获取结束日期
            // 判断输入是否合法
            string stationId = "";
            Object stationObject = (cmbStation as CStationComboBox_1).GetStation_1();
            CEntityStation station = new CEntityStation();
            CEntitySoilStation soilstation = new CEntitySoilStation();
            if (stationObject is CEntityStation)
            {
                station = (CEntityStation)stationObject;
            }
            else
            {
                soilstation = (CEntitySoilStation)stationObject;
            }
            if (null == station || null == soilstation)
            {
                MessageBox.Show("请选择正确的站点后再进行查询");
                return;
            }
            // 判断开始时间是否小于结束时间
            if (dateTimeStart.Value >= dateTimeEnd.Value)
            {
                MessageBox.Show("开始时间必须小于结束时间");
                return;
            }
            if (null != station.StationID)
            {
                stationId = station.StationID;
            }
            else
            {
                stationId = soilstation.StationID;
            }
            DateTime start = this.dateTimeStart.Value;
            DateTime end = this.dateTimeEnd.Value;
            m_statusLable.Text = "正在查询...";
            this.Enabled = false;
            bool TimeSelect = false;
            try
            {
                if (cmb_TimeSelect.Text.Equals(CS_CMB_TimeData))
                {
                    TimeSelect = true;
                }
                if (ts_query.Text.Equals("雨量"))
                {
                    //List<CEntityTSRain> results = m_proxyTSRain.QueryForAll(stationid, start, end);
                    m_dgvTSRain.SetFilter(stationId, start, end, TimeSelect);
                }
                if (ts_query.Text.Equals("水位"))
                {
                    m_dgvTSWater.SetFilter(stationId, start, end, TimeSelect);
                }
                if (ts_query.Text.Equals("电压"))
                {
                    m_dgvTSVoltage.SetFilter(stationId, start, end, TimeSelect);
                }
            }
            catch (SqlException ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("数据库配置错误！");
            }
            this.Enabled = true;
            m_statusLable.Text = "查询已成功完成";


        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ts_query_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ts_query.Text.Equals("雨量"))
            {
                m_dgvTSRain.Show();
                m_dgvTSWater.Hide();
                m_dgvTSVoltage.Hide();

            }
            if (ts_query.Text.Equals("水位"))
            {
                m_dgvTSRain.Hide();
                m_dgvTSWater.Show();
                m_dgvTSVoltage.Hide();
            }
            if (ts_query.Text.Equals("电压"))
            {
                m_dgvTSRain.Hide();
                m_dgvTSWater.Hide();
                m_dgvTSVoltage.Show();
            }
        }
    }
}
