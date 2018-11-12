using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Hydrology.CControls;
using Hydrology.Entity;
using Hydrology.DataMgr;
using System.Windows.Forms;
using Hydrology.DBManager.Interface;



namespace Hydrology.Forms
{
    public partial class Form2 : Form
    {
        #region 静态常量
        private static readonly string CS_Subcenter_All = "所有分中心";
        #endregion 静态常量

        #region 数据成员
        /// <summary>
        /// 月畅通率表格控件
        /// </summary>

        private OneRainMonth m_OneWanMonth;
        private OneWaterMonth m_OneWaterMonth;
        private OneSoilMonth m_OneSoilMonth;

        #endregion 数据成员
        public Form2()
        {
            InitializeComponent();
            InitUI();
            //m_OneWanMonth.ReCalculateSize();
            //FormHelper.InitUserModeEvent(this);

        }
        private void InitUI()
        {
            this.SuspendLayout();

            //单站月rain表
            m_OneWanMonth = new OneRainMonth();
            m_OneWanMonth.AllowUserToAddRows = false;
            m_OneWanMonth.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            m_OneWanMonth.Dock = DockStyle.Fill;
            m_OneWanMonth.AllowUserToResizeRows = false;
            m_OneWanMonth.AllowUserToResizeColumns = true;
            m_OneWanMonth.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_OneWanMonth.RowHeadersWidth = 50;
            m_OneWanMonth.ColumnHeadersHeight = 25;
            m_OneWanMonth.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            //单站月water表
            m_OneWaterMonth = new OneWaterMonth();
            m_OneWaterMonth.AllowUserToAddRows = false;
            m_OneWaterMonth.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            m_OneWaterMonth.Dock = DockStyle.Fill;
            m_OneWaterMonth.AllowUserToResizeRows = false;
            m_OneWaterMonth.AllowUserToResizeColumns = true;
            m_OneWaterMonth.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_OneWaterMonth.RowHeadersWidth = 50;
            m_OneWaterMonth.ColumnHeadersHeight = 25;
            m_OneWaterMonth.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            //单站月soil表
            m_OneSoilMonth = new OneSoilMonth();
            m_OneSoilMonth.AllowUserToAddRows = false;
            m_OneSoilMonth.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            m_OneSoilMonth.Dock = DockStyle.Fill;
            m_OneSoilMonth.AllowUserToResizeRows = false;
            m_OneSoilMonth.AllowUserToResizeColumns = true;
            m_OneSoilMonth.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_OneSoilMonth.RowHeadersWidth = 50;
            m_OneSoilMonth.ColumnHeadersHeight = 25;
            m_OneSoilMonth.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            this.panel3.Controls.Add(m_OneWanMonth);
            this.panel3.Controls.Add(m_OneWaterMonth);
            this.panel3.Controls.Add(m_OneSoilMonth);

            m_OneWanMonth.Visible = true;
            m_OneWaterMonth.Visible = false;
            m_OneSoilMonth.Visible = false;
            InitDate();
            InitSubCenter();

        }
        private void InitSubCenter()
        {
            // 初始化分中心
            List<CEntitySubCenter> subcenter = CDBDataMgr.Instance.GetAllSubCenter();
            center.Items.Add(CS_Subcenter_All);
            for (int i = 0; i < subcenter.Count; ++i)
            {
                center.Items.Add(subcenter[i].SubCenterName);
            }
            this.center.SelectedIndex = 0;
            List<CEntityStation> iniStations = getStations(center.Text);
            List<CEntitySoilStation> iniSoilStations = getSoilStations(center.Text);
            InitStations(iniStations);
            InitSoilStations(iniSoilStations);
        }
        //gm 帮助函数
        private void center_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<CEntityStation> iniStations = getStations(center.Text);
            InitStations(iniStations);
            List<CEntitySoilStation> iniSoilStations = getSoilStations(center.Text);
            InitSoilStations(iniSoilStations);
        }

        private void staitonSelect_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue == CheckState.Checked) return;//取消选中就不用进行以下操作  
            for (int i = 0; i < ((CheckedListBox)sender).Items.Count; i++)
            {
                ((CheckedListBox)sender).SetItemChecked(i, false);//将所有选项设为不选中  
            }
            e.NewValue = CheckState.Checked;//刷新  
        }

        // 初始化日期
        private void InitDate()
        {
            date.CustomFormat = "yyyy年MM月";
            date.Value = DateTime.Now;
        }
        private List<CEntityStation> getStations(string subCenter)
        {
            List<CEntityStation> stations = new List<CEntityStation>();
            if (center.Text == CS_Subcenter_All)
            {
                // 统计所有的站点
                stations = CDBDataMgr.Instance.GetAllStation();
            }
            else
            {
                // 统计某个中心下的所有站点
                List<CEntityStation> stationsAll = CDBDataMgr.Instance.GetAllStation();
                CEntitySubCenter centerName = CDBDataMgr.Instance.GetSubCenterByName(subCenter);
                for (int i = 0; i < stationsAll.Count; ++i)
                {
                    if (stationsAll[i].SubCenterID == centerName.SubCenterID)
                    {
                        // 该测站在分中心内，添加到要查询的列表中
                        stations.Add(stationsAll[i]);
                    }
                }
            }
            return stations;
        }

        //1108 gaoming
        private List<CEntitySoilStation> getSoilStations(string subCenter)
        {
            List<CEntitySoilStation> stations = new List<CEntitySoilStation>();
            if (center.Text == CS_Subcenter_All)
            {
                // 统计所有的站点
                stations = CDBSoilDataMgr.Instance.GetAllSoilStation();
            }
            else
            {
                // 统计某个中心下的所有站点
                List<CEntitySoilStation> stationsAll = CDBSoilDataMgr.Instance.GetAllSoilStation();
                CEntitySubCenter centerName = CDBDataMgr.Instance.GetSubCenterByName(subCenter);
                for (int i = 0; i < stationsAll.Count; ++i)
                {
                    if (stationsAll[i].SubCenterID == centerName.SubCenterID)
                    {
                        // 该测站在分中心内，添加到要查询的列表中
                        stations.Add(stationsAll[i]);
                    }
                }
            }
            return stations;
        }
        private void InitStations(List<CEntityStation> stations)
        {
            if (stations.Count > 0)
            {
                this.stationSelect.Items.Clear();
                for (int i = 0; i < stations.Count; i++)
                {
                    this.stationSelect.Items.Add(stations[i].StationID + "|" + stations[i].StationName);
                }
                if (this.stationSelect.Items.Count > 0)
                {
                    this.stationSelect.SetItemChecked(0, true);
                }

            }
            else
            {
                this.stationSelect.Items.Clear();
            }
        }
        private void InitSoilStations(List<CEntitySoilStation> stations)
        {
            if (stations.Count > 0)
            {
                //this.stationSelect.Items.Clear();
                for (int i = 0; i < stations.Count; i++)
                {
                    this.stationSelect.Items.Add(stations[i].StationID + "|" + stations[i].StationName);
                }
                if (this.stationSelect.Items.Count > 0)
                {
                    this.stationSelect.SetItemChecked(0, true);
                }
            }
        }

        private void search_Click(object sender, EventArgs e)
        {
            //获取 radioButton 的table类型值；
            string tableType = getType();

            // 获取分中心
            string subcenterName = center.Text;
            //获取时间  date.value;

            // 获取站点信息 默认为全部站点
            List<string> stationSelected = new List<string>();
            for (int i = 0; i < stationSelect.Items.Count; i++)
            {
                if (stationSelect.GetItemChecked(i))
                {
                    stationSelected.Add(stationSelect.GetItemText(stationSelect.Items[i]));
                }
            }
            string TargetStation = stationSelected[0];
            string[] targetArray = TargetStation.Split('|');
            string StationID = targetArray[0];
            string StationName = targetArray[1];
            DateTime dt1 = date.Value;
            DateTime dt = dt1.Date;
            int year = dt.Year;
            int Month = dt.Month;
            int days = DateTime.DaysInMonth(dt.Year, dt.Month);
            DateTime Temp = new DateTime(year, Month, 1, 9, 0, 0);
            if (tableType.Equals("雨  量"))
            {
                CMessageBox box = new CMessageBox();
                box.MessageInfo = "正在查询数据库";
                box.ShowDialog(this);
                this.Enabled = false;
                m_OneWanMonth.SetFilter(StationID, Temp);
                m_OneWanMonth.Visible = true;
                this.Enabled = true;
                box.CloseDialog();
            }
            if (tableType.Equals("水  位"))
            {
                CMessageBox box = new CMessageBox();
                box.MessageInfo = "正在查询数据库";
                box.ShowDialog(this);
                this.Enabled = false;
                m_OneWaterMonth.SetFilter(StationID, Temp);
                m_OneWaterMonth.Visible = true;
                this.Enabled = true;
                box.CloseDialog();
            }
            if (tableType.Equals("墒  情"))
            {
                CMessageBox box = new CMessageBox();
                box.MessageInfo = "正在查询数据库";
                box.ShowDialog(this);
                this.Enabled = false;
                m_OneSoilMonth.SetFilter(StationID, Temp);
                m_OneSoilMonth.Visible = true;
                this.Enabled = true;
                box.CloseDialog();
            }
            this.Enabled = true;
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void export_Click(object sender, EventArgs e)
        {
            DateTime dt = date.Value;

            List<string> stationSelected = new List<string>();
            for (int i = 0; i < stationSelect.Items.Count; i++)
            {
                if (stationSelect.GetItemChecked(i))
                {
                    stationSelected.Add(stationSelect.GetItemText(stationSelect.Items[i]));
                }
            }
            string TargetStation = stationSelected[0];
            string[] targetArray = TargetStation.Split('|');
            string stationID = targetArray[0];
            string stationName = targetArray[1];
            // 导出到Excel文件
            if (m_OneWanMonth.Visible)
            {
                //m_OneWanMonth.ExportToExcel(dt, stationName);
                m_OneWanMonth.ExportToExcelNew(m_OneWanMonth, dt, stationName);
            }
            if (m_OneWaterMonth.Visible)
            {
                // m_OneWaterMonth.ExportToExcel(dt, stationName);
                m_OneWaterMonth.ExportToExcelNew(m_OneWaterMonth, dt, stationName);
            }
            if (m_OneSoilMonth.Visible)
            {
                //m_OneSoilMonth.ExportToExcel(dt, stationName);
                m_OneSoilMonth.ExportToExcelNew(m_OneSoilMonth, dt, stationName);
            }
            this.Focus();
        }

        private void cmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_OneWanMonth.Visible = true;
        }

        private void stationSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //帮助函数
        // 获取表格类型
        private string getType()
        {
            string result = "";
            foreach (var item in type.Controls)
            {
                if (item is RadioButton)
                {
                    RadioButton radioButton = item as RadioButton;
                    if (radioButton.Checked)
                    {
                        result = radioButton.Text.Trim();
                    }
                }
            }
            return result;
        }
        // 获取报表类型
        private void TableTypeChanged(object sender, EventArgs e)
        {
            string type = getType();

            if (type.Equals("雨  量"))
            {
                m_OneWanMonth.Visible = true;
                m_OneWaterMonth.Visible = false;
                m_OneSoilMonth.Visible = false;
            }
            else if (type.Equals("水  位"))
            {
                m_OneWanMonth.Visible = false;
                m_OneWaterMonth.Visible = true;
                m_OneSoilMonth.Visible = false;
            }
            else if (type.Equals("墒  情"))
            {
                m_OneWanMonth.Visible = false;
                m_OneWaterMonth.Visible = false;
                m_OneSoilMonth.Visible = true;
            }

        }



        // 帮助函数


    }
}
