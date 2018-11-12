using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using Hydrology.Entity;
using System.IO;
using System.Text;
using System.Diagnostics;
using Hydrology.DBManager.Interface;

namespace Hydrology.Forms
{
    public partial class CSoilStationMgrForm : Form
    {
        /// <summary>
        /// 墒情站的dgv
        /// </summary>
        private CDataGridViewSoilStation m_dgvSoilStatioin;
        private List<CEntitySoilStation> m_listImport;
        private IStationProxy m_proxyStation; //数据库接口对象
        private ISoilStationProxy m_proxySoilStation; //数据库接口对象

        private readonly string CS_All_Station = "所有站点";

        public CSoilStationMgrForm()
        {
            try
            {
                m_listImport = new List<CEntitySoilStation>();
                m_proxyStation = CDBDataMgr.Instance.GetStationProxy();
                m_proxySoilStation = CDBSoilDataMgr.Instance.GetSoilStationProxy();
                InitializeComponent();
                InitUI();
                InitDataSource();
                CreateMsgBinding();
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch (Exception e) { }
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
        }
        // 初始化数据
        public void InitDataSource()
        {
            try
            {
                m_dgvSoilStatioin.InitDataSource(CDBSoilDataMgr.Instance.GetSoilStationProxy());
                List<CEntitySubCenter> listSubCenter = CDBDataMgr.Instance.GetAllSubCenter();

                cmb_SubCenter.Items.Add(CS_All_Station);
                for (int i = 0; i < listSubCenter.Count; ++i)
                {
                    cmb_SubCenter.Items.Add(listSubCenter[i].SubCenterName);
                }
                this.cmb_SubCenter.SelectedIndex = 0;
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        #region 帮助方法

        private void InitUI()
        {
            tableLayoutPanel.SuspendLayout();
            m_dgvSoilStatioin = new CDataGridViewSoilStation();
            m_dgvSoilStatioin.AllowUserToAddRows = false;
            m_dgvSoilStatioin.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            //m_dgvRain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvSoilStatioin.Dock = DockStyle.Fill;
            //m_dgvUser.AutoSize = true;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvSoilStatioin.AllowUserToResizeRows = false;
            m_dgvSoilStatioin.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvSoilStatioin.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            m_dgvSoilStatioin.RowHeadersWidth = 50;
            m_dgvSoilStatioin.ColumnHeadersHeight = 25;

            m_dgvSoilStatioin.SetEditMode(true); //目前只支持编写了编辑模式
            this.cmb_SubCenter.SelectedIndexChanged += new EventHandler(EHSubCenterChanged);

            tableLayoutPanel.Controls.Add(m_dgvSoilStatioin, 0, 1);
            tableLayoutPanel.ResumeLayout(false);
        }

        private void EHFormLoad(object sender, EventArgs e)
        {
            // 加载数据
            m_dgvSoilStatioin.LoadData();
        }

        private void EHSoilStationCountChanged(object sender, Entity.CEventSingleArgs<int> e)
        {
            labelUserCount.Text = String.Format("共 {0} 个墒情站配置", e.Value);
        }

        /// <summary>
        /// 建立消息绑定
        /// </summary>
        private void CreateMsgBinding()
        {
            m_dgvSoilStatioin.E_SoilStationCountChanged += new EventHandler<Entity.CEventSingleArgs<int>>(EHSoilStationCountChanged);
            this.Load += new EventHandler(EHFormLoad);
            this.FormClosing += new FormClosingEventHandler(EHFormClosing);

            tsButAdd.Click += new EventHandler(EHAddSoilStation);
            tsButDelete.Click += new EventHandler(EHDeleteSoilStation);
            tsButExit.Click += new EventHandler(EHExit);
            tsButRevert.Click += new EventHandler(EHRevert);
            tsButSave.Click += new EventHandler(EHSave);
        }

        private void EHSave(object sender, EventArgs e)
        {
            m_dgvSoilStatioin.DoSave();
            this.cmb_SubCenter.SelectedIndex = 0;
        }

        private void EHFormClosing(object sender, FormClosingEventArgs e)
        {
            // 窗体关闭事件，检测是否需要保存数据
            if (!m_dgvSoilStatioin.Close())
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
            m_dgvSoilStatioin.Revert();
        }

        private void EHExit(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
        }

        private void EHDeleteSoilStation(object sender, EventArgs e)
        {
            m_dgvSoilStatioin.DoDelete();
        }

        private void EHAddSoilStation(object sender, EventArgs e)
        {
            // m_dgvSoilStatioin.AddNewSoilDataConfig();
            CSoilStationMgrForm2 form2 = new CSoilStationMgrForm2();
            form2.AddSoilStationEvent += new MyDelegateSoil(form2_MyEvent);
            if (form2 != null)
            {
                form2.ShowDialog();
            }
        }

        //处理
        void form2_MyEvent(CEntitySoilStation station)
        {
            //  m_dgvSoilStatioin.m_listAddedSoilStation.Add(station);
            //   m_dgvSoilStatioin.m_proxySoilStation.AddNewRow(station);
            m_dgvSoilStatioin.m_listAddedSoilStation.Add(station);
            m_dgvSoilStatioin.m_proxySoilStation.AddSoilStationRange(m_dgvSoilStatioin.m_listAddedSoilStation);
            //m_dgvStatioin.Revert();
            // 重新加载
            CDBSoilDataMgr.Instance.ReloadSoilStation();
            CDBSoilDataMgr.Instance.UpdateAllSoilStation();
            m_dgvSoilStatioin.Revert();
            m_dgvSoilStatioin.UpdateDataToUI();
            //this.listBox1.Items.RemoveAt(index);
            //this.listBox1.Items.Insert(index, text);
        }

        #endregion 帮助方法

        private void textBox_Search_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // 如果输入框中的文本发生变化，则更新列表框中显示的站点
                string filter = textBox_Search.Text;

                List<CEntitySoilStation> m_listSoilStation = CDBSoilDataMgr.Instance.GetAllSoilStation();
                if (filter != "")
                {

                    List<CEntitySoilStation> m_listStation1 = new List<CEntitySoilStation>();
                    foreach (CEntitySoilStation station in m_listSoilStation)
                    {
                        string tmp = GetDisplayStationName(station);
                        if (tmp.Contains(filter))
                        {
                            m_listStation1.Add(CDBSoilDataMgr.Instance.GetSoilStationInfoByStationId(station.StationID));
                            // listBox_StationName.Items.Add(tmp);
                            //  m_dgvSoilStatioin.SetSoilStation(m_listStation1);
                        }
                    }
                    m_dgvSoilStatioin.SuspendLayout();//暂停VIEW的刷新（datagridview的方法
                    m_dgvSoilStatioin.Hide();
                    m_dgvSoilStatioin.SetSoilStation(m_listStation1);
                    m_dgvSoilStatioin.Show();
                    m_dgvSoilStatioin.ResumeLayout();
                }
                else
                {
                    m_dgvSoilStatioin.SuspendLayout();//暂停VIEW的刷新（datagridview的方法
                    m_dgvSoilStatioin.Hide();
                    m_dgvSoilStatioin.SetSoilStation(m_listSoilStation);
                    m_dgvSoilStatioin.Show();
                    m_dgvSoilStatioin.ResumeLayout();
                }
                this.cmb_SubCenter.SelectedIndex = 0;
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        private void EHSubCenterChanged(object sender, EventArgs e)
        {
            try
            {
                int selectindex = cmb_SubCenter.SelectedIndex;
                if (0 == selectindex)
                {
                    m_dgvSoilStatioin.SetSubCenterName(null); //所有分中
                }
                else
                {
                    string subcentername = cmb_SubCenter.Text;
                    m_dgvSoilStatioin.SetSubCenterName(subcentername);
                }
                this.labelUserCount.Text = string.Format("共{0}个站点", m_dgvSoilStatioin.Rows.Count);
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        private string GetDisplayStationName(CEntitySoilStation station)
        {
            return string.Format("({0,-4}|{1})", station.StationID, station.StationName);
        }

        private void tsButImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            try
            {
                dlg.Title = "选择水位流量文件";
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
                   // List<string> gprs = m_proxySoilStation.getAllGprs();
                    List<string> gprs = m_proxyStation.getAllGprs();
                    List<string> soilgprs = m_proxySoilStation.getAllGprs();
                    for (int i = 0; i < soilgprs.Count; i++)
                    {
                        gprs.Add(soilgprs[i]);
                    }
                    while ((linebuffer = reader.ReadLine()) != null && linebuffer.Length > 18)
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
                        m_proxySoilStation.AddSoilStationRange(m_listImport);
                        for (int i = 0; i < m_listImport.Count; i++)
                        {
                            CDBSoilDataMgr.GetInstance().m_mapStaionSoilInfo.Add(m_listImport[i].StationID, m_listImport[i]);
                        }
                        m_listImport.Clear();
                    }

                    MessageBox.Show("数据导入成功");

                    m_dgvSoilStatioin.ClearAllRows();
                    CDBSoilDataMgr.Instance.UpdateAllSoilStation();
                    // 加载数据
                    m_dgvSoilStatioin.Revert();

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


        private bool DealWithLineData(string str, int lineNumber, ref string strErroInfo, ref List<string> gprs)
        {
            decimal a10, b10, c10, d10, m10, n10, a20, b20, c20, d20, m20, n20, a30, b30, c30, d30, m30, n30, a40, b40, c40, d40, m40, n40, a60, b60, c60, d60, m60, n60, voltagemin;
            string[] results = str.Split(',');
            if (43 == results.Length)
            {
                try
                {
                    string stationid = results[0];
                    int subcenterid = CDBDataMgr.Instance.GetSubCenterByName(results[3]).SubCenterID;
                    string cname = results[1];

                    //    ESoilStationType stationtype = CEnumHelper.UIStrToStationType(results[3]);
                    CEntitySoilStation soilStation = new CEntitySoilStation();
                    soilStation.StationID = stationid;
                    soilStation.SubCenterID = subcenterid;
                    soilStation.StationName = cname;
                    soilStation.StationType = CEnumHelper.UIStrToStationType(results[2]);
                    if (results[12] != "")
                    {
                        a10 = decimal.Parse(results[12]);
                        soilStation.A10 = a10;
                    }
                    if (results[13] != "")
                    {
                        b10 = decimal.Parse(results[13]);
                        soilStation.B10 = b10;

                    }
                    if (results[14] != "")
                    {
                        c10 = decimal.Parse(results[14]);
                        soilStation.C10 = c10;

                    }
                    if (results[15] != "")
                    {
                        d10 = decimal.Parse(results[15]);
                        soilStation.D10 = d10;

                    }
                    if (results[16] != "")
                    {
                        m10 = decimal.Parse(results[16]);
                        soilStation.M10 = m10;

                    }
                    if (results[17] != "")
                    {
                        n10 = decimal.Parse(results[17]);
                        soilStation.N10 = n10;
                    }
                    if (results[18] != "")
                    {
                        a20 = decimal.Parse(results[18]);
                        soilStation.A20 = a20;

                    }
                    if (results[19] != "")
                    {
                        b20 = decimal.Parse(results[19]);
                        soilStation.B20 = b20;

                    }
                    if (results[20] != "")
                    {
                        c20 = decimal.Parse(results[20]);
                        soilStation.C20 = c20;
                    }
                    if (results[21] != "")
                    {
                        d20 = decimal.Parse(results[21]);

                        soilStation.D20 = d20;

                    }
                    if (results[22] != "")
                    {
                        m20 = decimal.Parse(results[22]);
                        soilStation.M20 = m20;

                    }
                    if (results[23] != "")
                    {
                        n20 = decimal.Parse(results[23]);
                        soilStation.N20 = n20;
                    }
                    if (results[24] != "")
                    {
                        a30 = decimal.Parse(results[24]);
                        soilStation.A30 = a30;

                    }
                    if (results[25] != "")
                    {
                        b30 = decimal.Parse(results[25]);
                        soilStation.B30 = b30;

                    }
                    if (results[26] != "")
                    {
                        c30 = decimal.Parse(results[26]);
                        soilStation.C30 = c30;

                    }
                    if (results[27] != "")
                    {
                        d30 = decimal.Parse(results[27]);
                        soilStation.D30 = d30;

                    }
                    if (results[28] != "")
                    {
                        m30 = decimal.Parse(results[28]);
                        soilStation.M30 = m30;

                    }
                    if (results[29] != "")
                    {
                        n30 = decimal.Parse(results[29]);
                        soilStation.N30 = n30;
                    }
                    if (results[30] != "")
                    {
                        a40 = decimal.Parse(results[30]);
                        soilStation.A40 = a40;

                    }
                    if (results[31] != "")
                    {
                        b40 = decimal.Parse(results[31]);
                        soilStation.B40 = b40;

                    }
                    if (results[32] != "")
                    {
                        c40 = decimal.Parse(results[32]);
                        soilStation.C40 = c40;

                    }
                    if (results[33] != "")
                    {
                        d40 = decimal.Parse(results[33]);
                        soilStation.D40 = d40;

                    }
                    if (results[34] != "")
                    {
                        m40 = decimal.Parse(results[34]);
                        soilStation.M40 = m40;

                    }
                    if (results[35] != "")
                    {
                        n40 = decimal.Parse(results[35]);
                        soilStation.N40 = n40;
                    }
                    if (results[36] != "")
                    {
                        a60 = decimal.Parse(results[36]);
                        soilStation.A60 = a60;

                    }
                    if (results[37] != "")
                    {
                        b60 = decimal.Parse(results[37]);
                        soilStation.B60 = b60;

                    }
                    if (results[38] != "")
                    {
                        c60 = decimal.Parse(results[38]);
                        soilStation.C60 = c60;

                    }
                    if (results[39] != "")
                    {
                        d60 = decimal.Parse(results[39]);
                        soilStation.D60 = d60;

                    }
                    if (results[40] != "")
                    {
                        m60 = decimal.Parse(results[40]);
                        soilStation.M60 = m60;

                    }
                    if (results[41] != "")
                    {
                        n60 = decimal.Parse(results[41]);
                        soilStation.N60 = n60;
                    }
                    if (results[42] != "")
                    {
                        voltagemin = decimal.Parse(results[42]);
                        soilStation.VoltageMin = voltagemin;
                    }
                    soilStation.GSM = results[4];
                    if (results[5] != "" && gprs.Contains(results[5]))
                    {
                        strErroInfo = string.Format("行：{0} 数据格式错误", lineNumber);
                        return false;
                    }
                    else
                    {
                        soilStation.GPRS = results[5];
                        gprs.Add(results[5]);
                    }
                    soilStation.BDSatellite = results[6];
                    soilStation.BDMemberSatellite = results[7];
                    if (results[8] == "SX-GPRS" || results[8] == "GSM" || results[8] == "Beidou-Normal" || results[8] == "Beidou-500")
                    {
                        soilStation.Maintran = results[8];
                    }
                    else
                    {
                        strErroInfo = string.Format("行：{0} 数据格式错误,主信道不能为空", lineNumber);
                        return false;
                    }
                    //判定subtran不能为空
                    if (results[9] == "SX-GPRS" || results[9] == "GSM" || results[9] == "Beidou-Normal" || results[9] == "Beidou-500" || results[9] == "无")
                    {
                        soilStation.Subtran = results[9];
                    }
                    else
                    {
                        strErroInfo = string.Format("行：{0} 数据格式错误,备用信道不能为空", lineNumber);
                        return false;
                    }

                    if (results[10] == "LN")
                    {
                        soilStation.Datapotocol = results[10];
                    }
                    else
                    {
                        strErroInfo = string.Format("行：{0} 数据格式错误,数据协议出错", lineNumber);
                        return false;
                    }
                    if (results[11] == "1" || results[11] == "4" || results[11] == "8"
                                || results[11] == "12" || results[11] == "24" || results[11] == "48")
                    {
                        soilStation.Reportinterval = results[11];
                    }
                    else
                    {
                        strErroInfo = string.Format("行：{0} 数据格式错误,报讯断次不能为空", lineNumber);
                        return false;
                    }
                    m_listImport.Add(soilStation);

                    return true;


                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    strErroInfo = string.Format("行：{0} 数据格式错误", lineNumber);
                    return false;
                }
            }
            else
            {
                strErroInfo = string.Format("行：{0} 数据格式错误", lineNumber);
                return false;
            }
           // return false;
        }


    }
}
