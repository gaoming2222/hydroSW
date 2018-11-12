using Hydrology.CControls.CDataGridView;
using Hydrology.DataMgr;
using Hydrology.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hydrology.Forms
{
    public partial class OneStationYear : Form
    {
        #region 静态常量
        private static readonly string CS_Subcenter_All = "所有分中心";
        #endregion 静态常量

        #region 数据成员
        private OneWaterYear m_OneWaterYear;
        private OneRainYear m_OneRainYear;
        #endregion 数据成员
        public OneStationYear()
        {
            InitializeComponent();
            InitUI();
        }
        private void InitUI()
        {
            this.SuspendLayout();
            // 添加表格
            //单站月water表
            m_OneWaterYear = new OneWaterYear();
            m_OneWaterYear.AllowUserToAddRows = false;
            m_OneWaterYear.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            m_OneWaterYear.Dock = DockStyle.Fill;
            m_OneWaterYear.AllowUserToResizeRows = false;
            m_OneWaterYear.AllowUserToResizeColumns = true;
            m_OneWaterYear.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_OneWaterYear.RowHeadersWidth = 50;
            m_OneWaterYear.ColumnHeadersHeight = 25;
            m_OneWaterYear.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            m_OneRainYear = new OneRainYear();
            m_OneRainYear.AllowUserToAddRows = false;
            m_OneRainYear.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            m_OneRainYear.Dock = DockStyle.Fill;
            m_OneRainYear.AllowUserToResizeRows = false;
            m_OneRainYear.AllowUserToResizeColumns = true;
            m_OneRainYear.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_OneRainYear.RowHeadersWidth = 50;
            m_OneRainYear.ColumnHeadersHeight = 25;
            m_OneRainYear.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);


            this.panel3.Controls.Add(m_OneWaterYear);
            this.panel3.Controls.Add(m_OneRainYear);

            m_OneWaterYear.Visible = true;
            m_OneRainYear.Visible = false;


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
        private void InitDate()
        {
            date.CustomFormat = "yyyy年";
            date.Value = DateTime.Now;
        }


        private void center_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<CEntityStation> iniStations = getStations(center.Text);
            InitStations(iniStations);
            List<CEntitySoilStation> iniSoilStations = getSoilStations(center.Text);
            InitSoilStations(iniSoilStations);
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
            DateTime Temp = new DateTime(year, 1, 1, 9, 0, 0);
            if (tableType.Equals("雨  量"))
            {
                CMessageBox box = new CMessageBox();
                box.MessageInfo = "正在查询数据库";
                box.ShowDialog(this);
                this.Enabled = false;
                m_OneRainYear.SetFilter(StationID, Temp);
                m_OneRainYear.Visible = true;
                this.Enabled = true;
                box.CloseDialog();
                this.Enabled = true;
            }
            if (tableType.Equals("水  位"))
            {
                CMessageBox box = new CMessageBox();
                box.MessageInfo = "正在查询数据库";
                box.ShowDialog(this);
                this.Enabled = false;
                m_OneWaterYear.SetFilter(StationID, Temp);
                m_OneWaterYear.Visible = true;
                this.Enabled = true;
                box.CloseDialog();
                this.Enabled = true;
            }
            if (tableType.Equals("熵  情"))
            {
            }
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
            if (m_OneWaterYear.Visible)
            {
                //m_OneWaterYear.ExportToExcel(dt, stationName);
                m_OneWaterYear.ExportToExcelNew(m_OneWaterYear, dt, stationName);
            }
            if (m_OneRainYear.Visible)
            {
                //m_OneRainYear.ExportToExcel(dt, stationName);
                m_OneRainYear.ExportToExcelNew(m_OneRainYear, dt, stationName);
            }
            this.Focus();
        }

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

        private void stationSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        // 获取报表类型
        private void TableTypeChanged(object sender, EventArgs e)
        {
            string type = getType();
            if (type.Equals("雨  量"))
            {
                m_OneRainYear.Visible = true;
                m_OneWaterYear.Visible = false;

            }
            else if (type.Equals("水  位"))
            {
                m_OneRainYear.Visible = false;
                m_OneWaterYear.Visible = true;
            }
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
    }
}
