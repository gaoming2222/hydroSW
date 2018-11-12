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
    public partial class MoreStationsDayForm : Form
    {
        #region 静态常量
        private static readonly string CS_Subcenter_All = "所有分中心";
        #endregion 静态常量

        private MoreSoilDay moreSoilDay;  //水位表控件
        private MoreWaterDay moreWaterDay; //雨量控件
        private MoreRainDay moreRainDay;

        public MoreStationsDayForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.SuspendLayout();

            this.checkBox1.CheckedChanged += new EventHandler(EHCheckAllChanged);
            //添加水位表控件
            moreRainDay = new MoreRainDay();
            moreRainDay.AllowUserToAddRows = false;
            moreRainDay.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            moreRainDay.Dock = DockStyle.Fill;
            moreRainDay.AllowUserToResizeRows = false;
            moreRainDay.AllowUserToResizeColumns = true;
            moreRainDay.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            moreRainDay.RowHeadersWidth = 50;
            moreRainDay.ColumnHeadersHeight = 25;
            moreRainDay.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            // 添加雨量控件
            moreWaterDay = new MoreWaterDay();
            moreWaterDay.AllowUserToAddRows = false;
            moreWaterDay.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            moreWaterDay.Dock = DockStyle.Fill;
            moreWaterDay.AllowUserToResizeRows = false;
            moreWaterDay.AllowUserToResizeColumns = true;
            moreWaterDay.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            moreWaterDay.RowHeadersWidth = 50;
            moreWaterDay.ColumnHeadersHeight = 50;
            moreWaterDay.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            //添加墒情控件
            moreSoilDay = new MoreSoilDay();
            moreSoilDay.AllowUserToAddRows = false;
            moreSoilDay.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            moreSoilDay.Dock = DockStyle.Fill;
            moreSoilDay.AllowUserToResizeRows = false;
            moreSoilDay.AllowUserToResizeColumns = true;
            moreSoilDay.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            moreSoilDay.RowHeadersWidth = 50;
            moreSoilDay.ColumnHeadersHeight = 25;
            moreSoilDay.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            this.TablePanel.Controls.Add(moreSoilDay);
            this.TablePanel.Controls.Add(moreRainDay);
            this.TablePanel.Controls.Add(moreWaterDay);

            moreRainDay.Visible = true;
            moreWaterDay.Visible = false;
            moreSoilDay.Visible = false;
            InitDate();
            InitSubCenter();
        }
        // 获取表格类型
        private string getType()
        {
            string result = "";
            foreach (var item in TableType.Controls)
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
        private void InitDate()
        {
            DateTimer.CustomFormat = "yyyy年MM月";
            DateTimer.Value = DateTime.Now;
        }
        private void InitSubCenter()
        {
            // 初始化分中心
            List<CEntitySubCenter> subcenter = CDBDataMgr.Instance.GetAllSubCenter();
            SubCenter.Items.Add(CS_Subcenter_All);
            for (int i = 0; i < subcenter.Count; ++i)
            {
                SubCenter.Items.Add(subcenter[i].SubCenterName);
            }
            this.SubCenter.SelectedIndex = 0;
            List<CEntityStation> iniStations = getStations(SubCenter.Text);
            List<CEntitySoilStation> iniSoilStations = getSoilStations(SubCenter.Text);
            InitStations(iniStations);
            InitSoilStations(iniSoilStations);

        }
        private List<CEntityStation> getStations(string subCenter)
        {
            List<CEntityStation> stations = new List<CEntityStation>();
            if (SubCenter.Text == CS_Subcenter_All)
            {
                // 统计所有站点的畅通率
                stations = CDBDataMgr.Instance.GetAllStation();
            }
            else
            {
                // 统计某个分中心的畅通率
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
            if (SubCenter.Text == CS_Subcenter_All)
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
            this.StationSelect.Items.Clear();
            //this.StationSelect.Items.Add("所有站点");
            for (int i = 0; i < stations.Count; i++)
            {
                this.StationSelect.Items.Add(stations[i].StationID + "|" + stations[i].StationName);
            }
        }

        private void InitSoilStations(List<CEntitySoilStation> stations)
        {
            if (stations.Count > 0)
            {
                //this.stationSelect.Items.Clear();
                for (int i = 0; i < stations.Count; i++)
                {
                    this.StationSelect.Items.Add(stations[i].StationID + "|" + stations[i].StationName);
                }
                if (this.StationSelect.Items.Count > 0)
                {
                    this.StationSelect.SetItemChecked(0, true);
                }
            }
        }

        private void search_Click(object sender, EventArgs e)
        {
            //获取 radioButton 的table类型值；
            string tableType = "";
            foreach (var item in TableType.Controls)
            {
                if (item is RadioButton)
                {
                    RadioButton radioButton = item as RadioButton;
                    if (radioButton.Checked)
                    {
                        tableType = radioButton.Text.Trim();

                    }
                }
            }
            // 获取分中心
            string subcenterName = SubCenter.Text;
            //获取时间  date.value;
            // 获取站点信息 默认为全部站点
            List<string> stationSelected = new List<string>();

            int flagInt = 0;
            for (int i = 0; i < StationSelect.Items.Count; i++)
            {
                if (StationSelect.GetItemChecked(i))
                {
                    flagInt++;
                }
            }
            if (flagInt == 0)
            {
                for (int i = 0; i < StationSelect.Items.Count; i++)
                {
                    stationSelected.Add(StationSelect.GetItemText(StationSelect.Items[i]));
                }
            }
            else if (StationSelect.GetItemChecked(0))
            {
                for (int i = 0; i < StationSelect.Items.Count; i++)
                {
                    stationSelected.Add(StationSelect.GetItemText(StationSelect.Items[i]));
                }
            }
            else
            {
                for (int i = 0; i < StationSelect.Items.Count; i++)
                {
                    if (StationSelect.GetItemChecked(i))
                    {
                        stationSelected.Add(StationSelect.GetItemText(StationSelect.Items[i]));
                    }
                }
            }

            DateTime dt1 = DateTimer.Value;
            DateTime dt = dt1.Date;
            string type = getType();

            if (type.Equals("雨    量"))
            {
                CMessageBox box = new CMessageBox();
                box.MessageInfo = "正在查询数据库";
                box.ShowDialog(this);
                this.Enabled = false;
                moreRainDay.SetFilter(stationSelected, dt);
                moreRainDay.Visible = true;
                this.Enabled = true;
                box.CloseDialog();
            }
            if (type.Equals("水    位"))
            {
                CMessageBox box = new CMessageBox();
                box.MessageInfo = "正在查询数据库";
                box.ShowDialog(this);
                this.Enabled = false;
                //moreStationDay.SetFilterTest(stationSelected, dt); //water月表的显示
                moreWaterDay.SetFilter(stationSelected, dt);
                moreWaterDay.Visible = true;
                this.Enabled = true;
                box.CloseDialog();
            }
            if (type.Equals("墒    情"))
            {
                CMessageBox box = new CMessageBox();
                box.MessageInfo = "正在查询数据库";
                box.ShowDialog(this);
                this.Enabled = false;
                //m_MoreStationSoil.SetFilterTest(stationSelected, dt); //water月表的显示
                moreSoilDay.SetFilter(stationSelected, dt);
                moreSoilDay.Visible = true;
                this.Enabled = true;
                box.CloseDialog();
            }
            this.Enabled = true;
        }

        private void export_Click(object sender, EventArgs e)
        {
            // 导出到Excel文件
            DateTime dt = DateTimer.Value;
            // 导出到Excel文件
            if (moreRainDay.Visible)
            {
                //moreRainDay.ExportToExcel(dt);
                moreRainDay.ExportToExcelNew(moreRainDay, dt);
            }
            if (moreWaterDay.Visible)
            {
                //moreWaterDay.ExportToExcel(dt);
                moreWaterDay.ExportToExcelNew(moreWaterDay, dt);
            }
            if (moreSoilDay.Visible)
            {
                //moreSoilDay.ExportToExcel(dt);
                moreSoilDay.ExportToExcelNew(moreSoilDay, dt);
            }
            this.Focus();

        }

        private void center_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<CEntityStation> iniStations = getStations(SubCenter.Text);
            InitStations(iniStations);
            List<CEntitySoilStation> iniSoilStations = getSoilStations(SubCenter.Text);
            InitSoilStations(iniSoilStations);
        }
        private void TableTypeChanged(object sender, EventArgs e)
        {
            string type = getType();

            if (type.Equals("雨    量"))
            {
                moreRainDay.Visible = true;
                moreWaterDay.Visible = false;
                moreSoilDay.Visible = false;
            }
            else if (type.Equals("水    位"))
            {
                moreRainDay.Visible = false;
                moreWaterDay.Visible = true;
                moreSoilDay.Visible = false;
            }
            else if (type.Equals("墒    情"))
            {
                moreRainDay.Visible = false;
                moreWaterDay.Visible = false;
                moreSoilDay.Visible = true;
            }

        }

        private void EHCheckAllChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                // 全选
                for (int i = 0; i < StationSelect.Items.Count; i++)
                {
                    this.StationSelect.SetItemChecked(i, true);
                }

            }
            else
            {
                // 全不选
                for (int i = 0; i < StationSelect.Items.Count; i++)
                {
                    this.StationSelect.SetItemChecked(i, false);
                }

            }
        }
    }
}
