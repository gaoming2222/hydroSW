using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using System.IO;
using System.Diagnostics;
using Hydrology.Entity;
using Hydrology.DBManager.Interface;

namespace Hydrology.Forms
{
    public partial class CStationMgrForm : Form
    {
        /// <summary>
        /// 水情站的dgv
        /// </summary>
        private CDataGridViewStation m_dgvStatioin;
        //10.5 LH
        private List<CEntityStation> m_listImport;     //导入的记录
        private IStationProxy m_proxyStation; //数据库接口对象
        private ISoilStationProxy m_proxySoilStation; //数据库接口对象

        private List<CEntityStation> m_listStation;
        private List<CEntityStation> m_listStation1;
        private readonly string CS_All_Station = "所有站点";

        public CStationMgrForm()
        {
            try
            {
                InitializeComponent();
                m_listImport = new List<CEntityStation>();
                m_proxyStation = CDBDataMgr.Instance.GetStationProxy();
                m_proxySoilStation = CDBSoilDataMgr.Instance.GetSoilStationProxy();
                InitUI();
                InitDataSource();
                CreateMsgBinding();
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch(Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
            {
                
            }
        }

        // 初始化数据源
        public void InitDataSource()
        {
            m_dgvStatioin.InitDataSource();
            List<CEntitySubCenter> listSubCenter = CDBDataMgr.Instance.GetAllSubCenter();

            m_listStation = CDBDataMgr.Instance.GetAllStation();
            m_listStation1 = new List<CEntityStation>();

            cmb_SubCenter.Items.Add(CS_All_Station);
            for (int i = 0; i < listSubCenter.Count; ++i)
            {
                cmb_SubCenter.Items.Add(listSubCenter[i].SubCenterName);
            }
            this.cmb_SubCenter.SelectedIndex = 0;
        }

        private void InitUI()
        {
            tableLayoutPanel.SuspendLayout();
            m_dgvStatioin = new CDataGridViewStation();
            m_dgvStatioin.AllowUserToAddRows = false;
            m_dgvStatioin.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            //m_dgvRain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvStatioin.Dock = DockStyle.Fill;
            //m_dgvUser.AutoSize = true;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvStatioin.AllowUserToResizeRows = false;
            m_dgvStatioin.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvStatioin.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            m_dgvStatioin.RowHeadersWidth = 60;
            m_dgvStatioin.ColumnHeadersHeight = 25;

            m_dgvStatioin.SetEditMode(true); //目前只支持编写了编辑模式


            tableLayoutPanel.Controls.Add(m_dgvStatioin, 0, 1);
            tableLayoutPanel.ResumeLayout(false);

          
            this.cmb_SubCenter.SelectedIndexChanged += new EventHandler(EHSubCenterChanged);
        }

        private void EHFormLoad(object sender, EventArgs e)
        {
            // 加载数据
            m_dgvStatioin.LoadData();
        }

        private void EHStationCountChanged(object sender, Entity.CEventSingleArgs<int> e)
        {
            labelUserCount.Text = String.Format("共 {0} 个水情测站配置", e.Value);
        }

        private void EHSave(object sender, EventArgs e)
        {       
            m_dgvStatioin.DoSave();
            this.cmb_SubCenter.SelectedIndex = 0;
        }

        private void EHFormClosing(object sender, FormClosingEventArgs e)
        {
            // 窗体关闭事件，检测是否需要保存数据
            if (!m_dgvStatioin.Close())
            {
                // 不让退出
                DialogResult result = MessageBox.Show("是否强行退出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    // 强行退出
                    //e.Cancel = true;
                }
                else
                {
                    e.Cancel = true;
                }
                //e.Cancel = true;
            }
        }

        private void EHRevert(object sender, EventArgs e)
        {
            m_dgvStatioin.Revert();
        }

        private void EHExit(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
        }

        private void EHSubCenterChanged(object sender, EventArgs e)
        {
            int selectindex = cmb_SubCenter.SelectedIndex;
            if (0 == selectindex)
            {
                m_dgvStatioin.SetSubCenterName(null); //所有分中
            }
            else
            {
                string subcentername = cmb_SubCenter.Text;
                m_dgvStatioin.SetSubCenterName(subcentername);
            }
            this.labelUserCount.Text = string.Format("共{0}个站点", m_dgvStatioin.Rows.Count);
        }

        private void EHAddStation(object sender, EventArgs e)
        {
            //  m_dgvStatioin.AddNewDataConfig();

            //注册form2_MyEvent方法的MyEvent事件

            CStationMgrForm2 form = new CStationMgrForm2();
            form.AddStationEvent += new MyDelegate(form_MyEvent);
            if (form != null)
            {
                form.ShowDialog();
            }
        }

        //处理
        void form_MyEvent(CEntityStation station)
        {
            m_dgvStatioin.m_listAddedStation.Add(station);
            m_dgvStatioin.m_proxyStation.AddRange(m_dgvStatioin.m_listAddedStation);
            //m_dgvStatioin.Revert();
            // 重新加载
            CDBDataMgr.Instance.UpdateAllStation();
            m_dgvStatioin.Revert();
            m_dgvStatioin.UpdateDataToUI();
            //this.listBox1.Items.RemoveAt(index);
            //this.listBox1.Items.Insert(index, text);
        }

        private void EHDeleteStation(object sender, EventArgs e)
        {
            m_dgvStatioin.DoDelete();
        }
    
        private bool DealWithLineData(string str, int lineNumber, ref string strErroInfo, ref List<string> gprs)
        {
            Boolean flag = false;
            //if (str.StartsWith("(") && str.EndsWith(")"))
            //{
            // gm 添加
            //string tmp = str.Substring(1, str.Length - 2);
            string[] arrayStr = str.Split(',');
            if (arrayStr.Length == 21)
            {
                if (arrayStr[0].Length == 4)
                    try
                    {
                        string stationId = arrayStr[0].Trim();
                        int subcenterid = 2;
                        try
                        {
                            subcenterid = CDBDataMgr.Instance.GetSubCenterByName(arrayStr[3]).SubCenterID;
                        }
#pragma warning disable CS0168 // 声明了变量“ew”，但从未使用过
                        catch (Exception ew)
#pragma warning restore CS0168 // 声明了变量“ew”，但从未使用过
                        {

                        }

                        if (CDBDataMgr.Instance.GetStationById(stationId) != null)
                        {
                            strErroInfo = string.Format("行：{0} 站点不能重复\"{1}\"", lineNumber, stationId);
                            return false;
                        }
                        //int subcenterid = int.Parse(arrayStr[1]);
                        string stationname = arrayStr[1].Trim();
                        EStationType stationtype = CEnumHelper.UIStrToStationType(arrayStr[2]);
                        if (stationtype == EStationType.ERainFall)
                        {

                            float RAccuracy = 0.5f;
                            try
                            {
                                RAccuracy = float.Parse(arrayStr[8]);
                            }
#pragma warning disable CS0168 // 声明了变量“e5”，但从未使用过
                            catch (Exception e5)
#pragma warning restore CS0168 // 声明了变量“e5”，但从未使用过
                            {
                                RAccuracy = 0.5f;
                            }

                            Decimal RChange = 20.0M;
                            try
                            {
                                RChange = Decimal.Parse(arrayStr[9]);
                            }
#pragma warning disable CS0168 // 声明了变量“e6”，但从未使用过
                            catch (Exception e6)
#pragma warning restore CS0168 // 声明了变量“e6”，但从未使用过
                            {
                                RChange = 20.0M;
                            }
                            string Gsm = arrayStr[10].Trim();
                            string Gprs = arrayStr[11].Trim();
                            if (Gprs != "" && gprs.Contains(Gprs))
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,GPRS与已有站点重复", lineNumber);
                                return false;
                            }
                            else
                            {
                                gprs.Add(Gprs);
                            }

                            string BDSatellite = arrayStr[12].Trim();
                            string BDmember = arrayStr[13].Trim();
                            float VoltageMin = 11;
                            try
                            {
                                VoltageMin = float.Parse(arrayStr[14]);
                            }
#pragma warning disable CS0168 // 声明了变量“er”，但从未使用过
                            catch (Exception er)
#pragma warning restore CS0168 // 声明了变量“er”，但从未使用过
                            {
                                VoltageMin = 11;
                            }

                            string maintran = arrayStr[15].Trim();
                            string subtran = arrayStr[16].Trim();
                            string dataprotocol = arrayStr[17].Trim();
                            //string watersensor = arrayStr[18].Trim();
                            string rainsensor = arrayStr[19].Trim();
                            string reportinterval = arrayStr[20].Trim();

                            CEntityStation tmp = new CEntityStation();
                            tmp.StationID = stationId;
                            tmp.SubCenterID = subcenterid;
                            tmp.StationName = stationname;
                            tmp.StationType = stationtype;
                            //tmp.DWaterBase = WBase;
                            //tmp.DWaterMax = WMax;
                            //tmp.DWaterMin = WMin;
                            //tmp.DWaterChange = WChange;
                            tmp.DRainAccuracy = RAccuracy;
                            tmp.DRainChange = RChange;
                            tmp.GSM = Gsm;
                            tmp.GPRS = Gprs;
                            tmp.BDSatellite = BDSatellite;
                            tmp.BDMemberSatellite = BDmember;
                            tmp.DVoltageMin = VoltageMin;
                            //判定maintrain不能为空
                            if (maintran == "SX-GPRS" || maintran == "HD-GPRS" || maintran == "GSM" || maintran == "Beidou-Normal" || maintran == "Beidou-500")
                            {
                                tmp.Maintran = maintran;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,主信道不能为空", lineNumber);
                                return false;
                            }
                            //判定subtran不能为空
                            if (subtran == "SX-GPRS" || subtran == "HD-GPRS" || subtran == "GSM" || subtran == "Beidou-Normal" || subtran == "Beidou-500" || subtran == "无")
                            {
                                tmp.Subtran = subtran;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,备用信道不能为空", lineNumber);
                                return false;
                            }
                            //判定数据协议不能为空
                            if (dataprotocol == "LN" || dataprotocol == "ZYJBX")
                            {
                                tmp.Datapotocol = dataprotocol;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,数据协议出错", lineNumber);
                                return false;
                            }
                            // 判定watersensor不能为空
                            //if (watersensor == "浮子水位" || watersensor == "气泡水位" || watersensor == "压阻水位" || watersensor == "雷达水位" || watersensor == "无")
                            //{
                            //    tmp.Watersensor = CEnumHelper.WaterSensorTypeToDBStr(maintran).ToString();
                            //}
                            //else
                            //{
                            //    strErroInfo = string.Format("行：{0} 数据格式错误,水位传感器不能为空", lineNumber);
                            //    return false;
                            //}
                            // 判定Rainsensor不能为空
                            if (rainsensor == "翻斗雨量" || rainsensor == "雨雪雨量" || rainsensor == "无")
                            {
                                tmp.Rainsensor = CEnumHelper.RainSensorTypeToDBStr(rainsensor).ToString();
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,雨量传感器不能为空", lineNumber);
                                return false;
                            }
                            //检测报讯断次
                            if (reportinterval == "1" || reportinterval == "4" || reportinterval == "8"
                                || reportinterval == "12" || reportinterval == "24" || reportinterval == "48")
                            {
                                tmp.Reportinterval = reportinterval;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,报讯断次不能为空", lineNumber);
                                return false;
                            }
                            tmp.Reportinterval = reportinterval;
                            m_listImport.Add(tmp);
                            flag = true;

                        }
                        else if (stationtype == EStationType.ERiverWater)
                        {
                            Decimal WBase = 0.0M;
                            try
                            {
                                WBase = Decimal.Parse(arrayStr[4]);
                            }
#pragma warning disable CS0168 // 声明了变量“e1”，但从未使用过
                            catch (Exception e1)
#pragma warning restore CS0168 // 声明了变量“e1”，但从未使用过
                            {
                                WBase = 0;
                            }
                            Decimal WMax = 100M;
                            try
                            {
                                WMax = Decimal.Parse(arrayStr[5]);
                            }
#pragma warning disable CS0168 // 声明了变量“e2”，但从未使用过
                            catch (Exception e2)
#pragma warning restore CS0168 // 声明了变量“e2”，但从未使用过
                            {
                                WMax = 100M;
                            }

                            Decimal WMin = 0.1M;
                            try
                            {
                                WMin = Decimal.Parse(arrayStr[6]);
                            }
#pragma warning disable CS0168 // 声明了变量“e3”，但从未使用过
                            catch (Exception e3)
#pragma warning restore CS0168 // 声明了变量“e3”，但从未使用过
                            {
                                WMin = 0.1M;
                            }
                            Decimal WChange = 2.2M;
                            try
                            {
                                WChange = Decimal.Parse(arrayStr[7]);
                            }
#pragma warning disable CS0168 // 声明了变量“e4”，但从未使用过
                            catch (Exception e4)
#pragma warning restore CS0168 // 声明了变量“e4”，但从未使用过
                            {
                                WChange = 2.2M;
                            }
                            //float RAccuracy = 0.5f;
                            //try
                            //{
                            //    RAccuracy = float.Parse(arrayStr[8]);
                            //}
                            //catch (Exception e5)
                            //{
                            //    RAccuracy = 0.5f;
                            //}
                            //Decimal RChange = 20.0M;
                            //try
                            //{
                            //    RChange = Decimal.Parse(arrayStr[9]);
                            //}
                            //catch (Exception e6)
                            //{
                            //    RChange = 20.0M;
                            //}
                            string Gsm = arrayStr[10].Trim();

                            string Gprs = arrayStr[11].Trim();
                            if (Gprs != "" && gprs.Contains(Gprs))
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,GPRS与已有站点重复", lineNumber);
                                return false;
                            }
                            else
                            {
                                gprs.Add(Gprs);
                            }

                            string BDSatellite = arrayStr[12].Trim();
                            string BDmember = arrayStr[13].Trim();
                            float VoltageMin = float.Parse(arrayStr[14]);
                            string maintran = arrayStr[15].Trim();
                            string subtran = arrayStr[16].Trim();
                            string dataprotocol = arrayStr[17].Trim();
                            string watersensor = arrayStr[18].Trim();
                            //string rainsensor = arrayStr[19].Trim();
                            string reportinterval = arrayStr[20].Trim();

                            CEntityStation tmp = new CEntityStation();
                            tmp.StationID = stationId;
                            tmp.SubCenterID = subcenterid;
                            tmp.StationName = stationname;
                            tmp.StationType = stationtype;
                            tmp.DWaterBase = WBase;
                            tmp.DWaterMax = WMax;
                            tmp.DWaterMin = WMin;
                            tmp.DWaterChange = WChange;
                            //tmp.DRainAccuracy = RAccuracy;
                            //tmp.DRainChange = RChange;
                            tmp.GSM = Gsm;
                            tmp.GPRS = Gprs;
                            tmp.BDSatellite = BDSatellite;
                            tmp.BDMemberSatellite = BDmember;
                            tmp.DVoltageMin = VoltageMin;
                            //判定maintrain不能为空
                            if (maintran == "SX-GPRS" || maintran == "HD-GPRS" || maintran == "GSM" || maintran == "Beidou-Normal" || maintran == "Beidou-500")
                            {
                                tmp.Maintran = maintran;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,主信道不能为空", lineNumber);
                                return false;
                            }
                            //判定subtran不能为空
                            if (subtran == "SX-GPRS" || subtran == "HD-GPRS" || subtran == "GSM" || subtran == "Beidou-Normal" || subtran == "Beidou-500" || subtran == "无")
                            {
                                tmp.Subtran = subtran;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,备用信道不能为空", lineNumber);
                                return false;
                            }
                            //判定数据协议不能为空
                            if (dataprotocol == "LN" || dataprotocol == "ZYJBX")
                            {
                                tmp.Datapotocol = dataprotocol;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,数据协议出错", lineNumber);
                                return false;
                            }
                            // 判定watersensor不能为空
                            if (watersensor == "浮子水位" || watersensor == "气泡水位" || watersensor == "压阻水位" || watersensor == "雷达水位" || watersensor == "无")
                            {
                                tmp.Watersensor = CEnumHelper.WaterSensorTypeToDBStr(watersensor).ToString();
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,水位传感器不能为空", lineNumber);
                                return false;
                            }
                            // 判定Rainsensor不能为空
                            //if (rainsensor == "翻斗雨量" || rainsensor == "雨雪雨量" || rainsensor == "无")
                            //{
                            //    tmp.Rainsensor = CEnumHelper.RainSensorTypeToDBStr(maintran).ToString();
                            //}
                            //else
                            //{
                            //    strErroInfo = string.Format("行：{0} 数据格式错误,雨量传感器不能为空", lineNumber);
                            //    return false;
                            //}
                            //检测报讯断次
                            if (reportinterval == "1" || reportinterval == "4" || reportinterval == "8"
                                || reportinterval == "12" || reportinterval == "24" || reportinterval == "48")
                            {
                                tmp.Reportinterval = reportinterval;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,报讯断次不能为空", lineNumber);
                                return false;
                            }
                            //tmp.Reportinterval = reportinterval;
                            m_listImport.Add(tmp);
                            flag = true;
                        }
                        else if (stationtype == EStationType.EHydrology)
                        {
                            Decimal WBase = 0.0M;
                            try
                            {
                                WBase = Decimal.Parse(arrayStr[4]);
                            }
#pragma warning disable CS0168 // 声明了变量“e1”，但从未使用过
                            catch (Exception e1)
#pragma warning restore CS0168 // 声明了变量“e1”，但从未使用过
                            {
                                WBase = 0;
                            }
                            Decimal WMax = 100M;
                            try
                            {
                                WMax = Decimal.Parse(arrayStr[5]);
                            }
#pragma warning disable CS0168 // 声明了变量“e2”，但从未使用过
                            catch (Exception e2)
#pragma warning restore CS0168 // 声明了变量“e2”，但从未使用过
                            {
                                WMax = 100M;
                            }

                            Decimal WMin = 0.1M;
                            try
                            {
                                WMin = Decimal.Parse(arrayStr[6]);
                            }
#pragma warning disable CS0168 // 声明了变量“e3”，但从未使用过
                            catch (Exception e3)
#pragma warning restore CS0168 // 声明了变量“e3”，但从未使用过
                            {
                                WMin = 0.1M;
                            }
                            Decimal WChange = 2.2M;
                            try
                            {
                                WChange = Decimal.Parse(arrayStr[7]);
                            }
#pragma warning disable CS0168 // 声明了变量“e4”，但从未使用过
                            catch (Exception e4)
#pragma warning restore CS0168 // 声明了变量“e4”，但从未使用过
                            {
                                WChange = 2.2M;
                            }
                            float RAccuracy = 0.5f;
                            try
                            {
                                RAccuracy = float.Parse(arrayStr[8]);
                            }
#pragma warning disable CS0168 // 声明了变量“e5”，但从未使用过
                            catch (Exception e5)
#pragma warning restore CS0168 // 声明了变量“e5”，但从未使用过
                            {
                                RAccuracy = 0.5f;
                            }
                            Decimal RChange = 20.0M;
                            try
                            {
                                RChange = Decimal.Parse(arrayStr[9]);
                            }
#pragma warning disable CS0168 // 声明了变量“e6”，但从未使用过
                            catch (Exception e6)
#pragma warning restore CS0168 // 声明了变量“e6”，但从未使用过
                            {
                                RChange = 20.0M;
                            }
                            string Gsm = arrayStr[10].Trim();

                            string Gprs = arrayStr[11].Trim();
                            if (Gprs != "" && gprs.Contains(Gprs))
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,GPRS与已有站点重复", lineNumber);
                                return false;
                            }
                            else
                            {
                                gprs.Add(Gprs);
                            }

                            string BDSatellite = arrayStr[12].Trim();
                            string BDmember = arrayStr[13].Trim();
                            float VoltageMin = float.Parse(arrayStr[14]);
                            string maintran = arrayStr[15].Trim();
                            string subtran = arrayStr[16].Trim();
                            string dataprotocol = arrayStr[17].Trim();
                            string watersensor = arrayStr[18].Trim();
                            string rainsensor = arrayStr[19].Trim();
                            string reportinterval = arrayStr[20].Trim();
                            CEntityStation tmp = new CEntityStation();
                            tmp.StationID = stationId;
                            tmp.SubCenterID = subcenterid;
                            tmp.StationName = stationname;
                            tmp.StationType = stationtype;
                            tmp.DWaterBase = WBase;
                            tmp.DWaterMax = WMax;
                            tmp.DWaterMin = WMin;
                            tmp.DWaterChange = WChange;
                            tmp.DRainAccuracy = RAccuracy;
                            tmp.DRainChange = RChange;
                            tmp.GSM = Gsm;
                            tmp.GPRS = Gprs;
                            tmp.BDSatellite = BDSatellite;
                            tmp.BDMemberSatellite = BDmember;
                            tmp.DVoltageMin = VoltageMin;
                            //判定maintrain不能为空
                            if (maintran == "SX-GPRS" || maintran == "HD-GPRS" || maintran == "GSM" || maintran == "Beidou-Normal" || maintran == "Beidou-500")
                            {
                                tmp.Maintran = maintran;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,主信道不能为空", lineNumber);
                                return false;
                            }
                            //判定subtran不能为空
                            if (subtran == "SX-GPRS" || subtran == "HD-GPRS" || subtran == "GSM" || subtran == "Beidou-Normal" || subtran == "Beidou-500" || subtran == "无")
                            {
                                tmp.Subtran = subtran;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,备用信道不能为空", lineNumber);
                                return false;
                            }
                            //判定数据协议不能为空
                            if (dataprotocol == "LN" || dataprotocol == "ZYJBX")
                            {
                                tmp.Datapotocol = dataprotocol;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,数据协议出错", lineNumber);
                                return false;
                            }
                            // 判定watersensor不能为空
                            if (watersensor == "浮子水位" || watersensor == "气泡水位" || watersensor == "压阻水位" || watersensor == "雷达水位" || watersensor == "无")
                            {
                                tmp.Watersensor = CEnumHelper.WaterSensorTypeToDBStr(watersensor).ToString();
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,水位传感器不能为空", lineNumber);
                                return false;
                            }
                            // 判定Rainsensor不能为空
                            if (rainsensor == "翻斗雨量" || rainsensor == "雨雪雨量" || rainsensor == "无")
                            {
                                tmp.Rainsensor = CEnumHelper.RainSensorTypeToDBStr(rainsensor).ToString();
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,雨量传感器不能为空", lineNumber);
                                return false;
                            }
                            //检测报讯断次
                            if (reportinterval == "1" || reportinterval == "4" || reportinterval == "8"
                                || reportinterval == "12" || reportinterval == "24" || reportinterval == "48")
                            {
                                tmp.Reportinterval = reportinterval;
                            }
                            else
                            {
                                strErroInfo = string.Format("行：{0} 数据格式错误,报讯断次不能为空", lineNumber);
                                return false;
                            }
                            tmp.Reportinterval = reportinterval;
                            m_listImport.Add(tmp);
                            flag = true;
                        }
                    }
                    catch (Exception e)
                    {
                        strErroInfo = string.Format("行：{0} 数据格式错误", lineNumber);
                        Debug.WriteLine(e.ToString());
                        return false;
                    }
            }
            else
            {
                strErroInfo = string.Format("行：{0} 数据格式错误", lineNumber);
              
                return false;
            }

            return flag;

            //if (str.StartsWith("(") && str.EndsWith(")"))
            //{

            //    return false;
            //}
            //else
            //{
            //    // 格式不对
            //    strErroInfo = (string.Format("行：{0} 开始结束符号\"(\"\")\"格式错误", lineNumber));
            //    return false;
            //}
        }

        /// <summary>
        /// 建立消息绑定
        /// </summary>
        private void CreateMsgBinding()
        {
            m_dgvStatioin.E_StationCountChanged += new EventHandler<Entity.CEventSingleArgs<int>>(EHStationCountChanged);
            this.Load += new EventHandler(EHFormLoad);
            this.FormClosing += new FormClosingEventHandler(EHFormClosing);

            tsButAdd.Click += new EventHandler(EHAddStation);
            tsButDelete.Click += new EventHandler(EHDeleteStation);
            tsButExit.Click += new EventHandler(EHExit);
            tsButRevert.Click += new EventHandler(EHRevert);
            tsButSave.Click += new EventHandler(EHSave);
          //  tsButImport.Click += new EventHandler(EHImport);
        }

        private void textBox_Search_TextChanged(object sender, EventArgs e)
        {
            // 如果输入框中的文本发生变化，则更新列表框中显示的站点
            string filter = textBox_Search.Text;

            List<CEntityStation> m_listStation = CDBDataMgr.Instance.GetAllStation();
            if (filter != "")
            {
                //    List<CEntityStation> m_listStation1 = new List<CEntityStation>();
                m_listStation1.Clear();
                foreach (CEntityStation station in m_listStation)
                {
                    string tmp = GetDisplayStationName(station);
                    if (tmp.Contains(filter))
                    {
                        //m_listStation1.Add(CDBDataMgr.Instance.GetStationById(station.StationID));
                        //1107gm
                        m_listStation1.Add(station);
                        // listBox_StationName.Items.Add(tmp);                      
                    }
                }
                m_dgvStatioin.SuspendLayout();//暂停VIEW的刷新（datagridview的方法
                m_dgvStatioin.Hide();
                m_dgvStatioin.SetStation(m_listStation1);
                m_dgvStatioin.Show();
                m_dgvStatioin.ResumeLayout();
            }
            else
            {
                m_dgvStatioin.SuspendLayout();//暂停VIEW的刷新（datagridview的方法
                m_dgvStatioin.Hide();
                m_dgvStatioin.SetStation(m_listStation);
                this.cmb_SubCenter.SelectedIndex = 0;
                m_dgvStatioin.Show();
                m_dgvStatioin.ResumeLayout();
            }
        }

        private string GetDisplayStationName(CEntityStation station)
        {
            return string.Format("({0,-4}|{1})", station.StationID, station.StationName);
        }

        private void tsButImport_Click(object sender, EventArgs e)
        {
            // 从文件中导入，格式如(1,212.23,2230.344)，如果不对，不导入
            OpenFileDialog dlg = new OpenFileDialog();
            try
            {
                dlg.Title = "选择水情测站文件";
                dlg.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                DialogResult result = dlg.ShowDialog();
                if (DialogResult.OK == result)
                {
                    // 打开文件，并进行处理
                    //CMessageBox msgBox = new CMessageBox() { MessageInfo = "正在处理数据" };
                    //msgBox.ShowDialog(this);
                    StreamReader reader = new StreamReader(dlg.FileName, Encoding.Default);
                    string linebuffer;
                    int linenumber = 0;
                    string strErrorInfo = "";
                    // 获取数据库中已经存在的GPRS号码 gm 1107

                    List<string> gprs = m_proxyStation.getAllGprs();
                    List<string> soilgprs = m_proxySoilStation.getAllGprs();
                    for (int i = 0; i < soilgprs.Count; i++)
                    {
                        gprs.Add(soilgprs[i]);
                    }
                        while ((linebuffer = reader.ReadLine()) != null && linebuffer.Length > 20)
                        {
                            // 处理一行数据
                            linenumber += 1;
                            if (!DealWithLineData(linebuffer, linenumber, ref strErrorInfo, ref gprs))
                            {
                                // 数据非法
                                //msgBox.CloseDialog();
                                MessageBox.Show(strErrorInfo); // 显示错误信息
                                return;
                            }
                        }
                    reader.Close();
                    //msgBox.CloseDialog();

                    if (m_listImport.Count > 0)
                    {
                        m_proxyStation.AddRange(m_listImport);
                        for (int i = 0; i < m_listImport.Count; i++)
                        {
                            CDBDataMgr.GetInstance().m_mapStation.Add(m_listImport[i].StationID, m_listImport[i]);
                        }
                        m_listImport.Clear();
                    }
                    //if (m_listImport.Count > 0)
                    //{
                    //    m_proxyStation.AddRange(m_listImport);
                    //    for (int i = 0; i < m_listImport.Count; i++)
                    //    {
                    //        CDBDataMgr.GetInstance().m_mapStation.Add(m_listImport[i].StationID, m_listImport[i]);
                    //    }
                    //    m_listImport.Clear();
                    //}


                    MessageBox.Show("数据导入成功");

                    m_dgvStatioin.ClearAllRows();
                    CDBDataMgr.Instance.UpdateAllStation();
                    m_dgvStatioin.Revert();

                    // 加载数据
                    //m_dgvStatioin.LoadData();
                }//end of ok
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                this.Focus(); //防止窗体最小化
            }
        }
       
     
    }
}


