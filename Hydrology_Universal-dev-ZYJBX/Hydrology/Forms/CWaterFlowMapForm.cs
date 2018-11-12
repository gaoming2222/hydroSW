using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DBManager.Interface;
using Hydrology.DataMgr;
using Hydrology.Entity;
using System.Diagnostics;
using System.IO;

namespace Hydrology.Forms
{
    public partial class CWaterFlowMapForm : Form
    {
        #region 成员变量
        private CDataGridViewWaterFlowMap m_dgvMap;     //表格控件
        private IWaterFlowMapProxy m_proxyWaterFlowMap; //数据库接口对象
        private List<CEntityStation> m_listStation = new List<CEntityStation>();     //所有站点的指针
        private Dictionary<string, List<CEntityWaterFlowMap>> m_mapStationWaterFlow; //站点的水位流量对应关系
        private string m_strCurrentStationId;   //当前的站点ID

        private Dictionary<string, CDataGridViewWaterFlowMap.SStatus> m_mapStationDGVStatus;
        private Dictionary<string, CDataGridViewWaterFlowMap.SModifiedData> m_mapModifiedData; // 修改过的数据，便于一次性提交

        private List<CEntityWaterFlowMap> m_listAdded;      //增加的记录
        private List<CEntityWaterFlowMap> m_listUpated;     //修改的记录
        private List<long> m_listDeleted;                   //删除的记录
        //10.4 LH
        private List<CEntityWaterFlowMap> m_listImport;     //修改的记录
        public DateTime lastTime;
        public int lastCurrQ;
        private readonly string CS_All_Station = "所有站点";
        #endregion 成员变量
        public CWaterFlowMapForm()
        {
            InitializeComponent();
            m_listImport = new List<CEntityWaterFlowMap>();
            lastTime = new DateTime();
            lastCurrQ = 0;
            try
            {
                InitDataSource();
                InitUI();
            }
#pragma warning disable CS0168 // 声明了变量“exp”，但从未使用过
            catch (Exception exp) { };
#pragma warning restore CS0168 // 声明了变量“exp”，但从未使用过
            FormHelper.InitUserModeEvent(this);
        }

        #region 事件响应

        private void textBox_Search_TextChanged(object sender, EventArgs e)
        {
            // 如果输入框中的文本发生变化，则更新列表框中显示的站点
            string filter = textBox_Search.Text;
            // 隐藏某些行，再说
            // 全部重新加载
            listBox_StationName.Items.Clear();
            foreach (CEntityStation station in m_listStation)
            {
                string tmp = GetDisplayStationName(station);
                if (tmp.Contains(filter))
                {
                    listBox_StationName.Items.Add(tmp);
                }
            }
        }

        private void listBox_StationName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 选择的列表项发生改变
            //if (listBox_StationName.SelectedIndex >= 0)
            //{
            string stationId = listBox_StationName.SelectedItem.ToString().Substring(1, 4).Trim();
            //if (m_strCurrentStationId != null)
            //{
            // 如果点击同一个站点，不管
            if (stationId == m_strCurrentStationId)
            {
                return;
            }
            // 不是第一次显示，需要保存当前的状态信息
#pragma warning disable CS0168 // 声明了变量“status”，但从未使用过
            CDataGridViewWaterFlowMap.SStatus status;
#pragma warning restore CS0168 // 声明了变量“status”，但从未使用过
#pragma warning disable CS0168 // 声明了变量“data”，但从未使用过
            CDataGridViewWaterFlowMap.SModifiedData data;
#pragma warning restore CS0168 // 声明了变量“data”，但从未使用过
            // 先保存状态，然后再获取，否则出错
            //if (m_dgvMap.SaveStatus(out status) && m_dgvMap.GetAllModifiedData(out data))
            //{
            // 成功,保存状态
            //if (m_mapStationDGVStatus.ContainsKey(m_strCurrentStationId))
            //{
            //    m_mapStationDGVStatus[m_strCurrentStationId] = status;
            //}
            //else
            //{
            //    m_mapStationDGVStatus.Add(m_strCurrentStationId, status);
            //}

            //if (m_mapModifiedData.ContainsKey(m_strCurrentStationId))
            //{
            //    m_mapModifiedData[m_strCurrentStationId] = data;
            //}
            //else
            //{
            //    m_mapModifiedData.Add(m_strCurrentStationId, data);
            //}

            // 切换站点
            m_strCurrentStationId = stationId;

            List<CEntityWaterFlowMap> m_list = new List<CEntityWaterFlowMap>();
            m_list = m_proxyWaterFlowMap.QueryMapsByStationId(m_strCurrentStationId);
            m_dgvMap.InitDatas(m_strCurrentStationId, m_list);
            // 先初始化数据源，然后在恢复状态
            // m_dgvMap.InitDatas(m_strCurrentStationId, m_mapStationWaterFlow[stationId]);
            //if (m_mapStationDGVStatus.ContainsKey(m_strCurrentStationId))
            //{
            //    // 恢复表格状态
            //    m_dgvMap.RestoreStatus(m_mapStationDGVStatus[m_strCurrentStationId]);
            //}
            m_dgvMap.UpdateDataToUI();
            //}
            //else
            //{
            //    // 数据错误
            //    DialogResult result = MessageBox.Show("当前所做修改无效，是否放弃？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            //    if (DialogResult.Cancel == result || DialogResult.No == result)
            //    {
            //        //  设置选中项为先前的站点,不做任何更改
            //        listBox_StationName.SelectedItem = GetDisplayStationName(CDBDataMgr.Instance.GetStationById(m_strCurrentStationId));
            //    }
            //    else if (DialogResult.Yes == result)
            //    {
            //        // 放弃当前修改,啥都不用做
            //        m_strCurrentStationId = stationId;
            //        m_dgvMap.InitDatas(m_strCurrentStationId, m_mapStationWaterFlow[stationId]);
            //    }
            //}

            //}
            //else
            //{
            //    // 也就是第一次点击站点，那么直接显示就行了，不需要保存状态之类的
            //    m_strCurrentStationId = stationId;
            //    m_dgvMap.InitDatas(m_strCurrentStationId, m_mapStationWaterFlow[stationId]);
            //}
            //}// end of is selectedindex >=0
        }

        private void tsButSave_Click(object sender, EventArgs e)
        {
            // 先获取当前的保存
            CDataGridViewWaterFlowMap.SModifiedData data;
            m_dgvMap.GetAllModifiedData(out data);
            if (m_mapModifiedData.ContainsKey(m_strCurrentStationId))
            {
                m_mapModifiedData[m_strCurrentStationId] = data;
            }
            else
            {
                m_mapModifiedData.Add(m_strCurrentStationId, data);
            }
            GenerateSaveData();
            if (m_listAdded.Count > 0 || m_listDeleted.Count > 0 || m_listUpated.Count > 0)
            {
                // 需要保存数据
                if (DoSave())
                {

                }
            }
            else
            {
                MessageBox.Show("没有任何修改，无需保存");
            }
        }

        private void tsButRevert_Click(object sender, EventArgs e)
        {
            // 撤销当前所做的任何操作
            m_mapStationWaterFlow = new Dictionary<string, List<CEntityWaterFlowMap>>(
                CDBDataMgr.Instance.GetStationWaterFlowMap());
            m_mapStationDGVStatus.Clear();
            m_mapModifiedData.Clear();
            listBox_StationName.SelectedItem = null;
            m_strCurrentStationId = null;
            m_dgvMap.ClearAllRows();
        }

        private void tsButImport_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("录入新的水位流量（库容）关系线，程序会删除老的水位流量（库容）关系线，是否继续？", "继续", messButton);
            if (dr == DialogResult.OK)
            {

                // MessageBox.Show("录入新的水位流量（库容）关系线，程序会删除老的水位流量（库容）关系线，是否继续？");
                // 从文件中导入，格式如(1,212.23,2230.344)，如果不对，不导入
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
                        string stationid = "";
                        while ((linebuffer = reader.ReadLine()) != null)
                        {
                            // 处理一行数据
                            linenumber += 1;
                            //   if (!DealWithLineData(linebuffer, linenumber, ref strErrorInfo))                           
                            if (!DealWithLineData(linebuffer, linenumber, ref strErrorInfo, ref stationid))
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
                            m_proxyWaterFlowMap.AddRange(m_listImport);
                            m_listImport.Clear();
                        }

                        this.Hide();
                        MessageBox.Show("数据导入成功");
                        m_dgvMap.ClearAllRows();
                        m_dgvMap.Revert(stationid);
                        this.Show();
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
            else
            {

            }
        }

        private void tsButAddMap_Click(object sender, EventArgs e)
        {
            if (m_strCurrentStationId == null || m_strCurrentStationId.Equals(""))
            {
                MessageBox.Show("请先选中需要添加水位流量关系的测站");
                return;
            }
            // 将当前的站点显示
            CEntityStation station = CDBDataMgr.Instance.GetStationById(m_strCurrentStationId);
            listBox_StationName.TopIndex = listBox_StationName.FindString(GetDisplayStationName(station));
            //listBox_StationName.SelectedIndex = -1;
            m_dgvMap.AddNewRecord();
        }

        private void tsButDelete_Click(object sender, EventArgs e)
        {
            if (m_strCurrentStationId == null || m_strCurrentStationId.Equals(""))
            {
                MessageBox.Show("请先选中需要删除水位流量关系的测站");
                return;
            }
            m_dgvMap.DoDelete();
        }

        private void tsButExit_Click(object sender, EventArgs e)
        {
            // 先获取当前的保存
            //if (listBox_StationName.SelectedItem != null)
            //{
            //    CDataGridViewWaterFlowMap.SModifiedData data;
            //    if (!m_dgvMap.GetAllModifiedData(out data))
            //    {
            //        // 保存失败
            //        return;
            //    }
            //    if (m_mapModifiedData.ContainsKey(m_strCurrentStationId))
            //    {
            //        m_mapModifiedData[m_strCurrentStationId] = data;
            //    }
            //    else
            //    {
            //        m_mapModifiedData.Add(m_strCurrentStationId, data);
            //    }
            //}
            try
            {
                this.Close();
            }
#pragma warning disable CS0168 // 声明了变量“exp”，但从未使用过
            catch (Exception exp) { }
#pragma warning restore CS0168 // 声明了变量“exp”，但从未使用过
        }

        private void CWaterFlowMapForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            return;
            //// 先获取当前的保存
            //if (listBox_StationName.SelectedItem != null)
            //{
            //    CDataGridViewWaterFlowMap.SModifiedData data;
            //    if (m_dgvMap.GetAllModifiedData(out data))
            //    {
            //        // 保存成功
            //        if (m_mapModifiedData.ContainsKey(m_strCurrentStationId))
            //        {
            //            m_mapModifiedData[m_strCurrentStationId] = data;
            //        }
            //        else
            //        {
            //            m_mapModifiedData.Add(m_strCurrentStationId, data);
            //        }
            //    }
            //    else
            //    {
            //        // 保存失败
            //        DialogResult result = MessageBox.Show("是否强行退出?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        if (result == DialogResult.Yes)
            //        {
            //            // 强行退出  
            //        }
            //        else
            //        {
            //            // 不退出
            //            e.Cancel = true;
            //        }
            //        return;
            //    }
            //}

            //// 判断是否有修改尚未保存
            //GenerateSaveData();
            //if (m_listAdded.Count > 0 || m_listDeleted.Count > 0 || m_listUpated.Count > 0)
            //{
            //    // 数据错误
            //    DialogResult result = MessageBox.Show("当前所做修改尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            //    if (DialogResult.Cancel == result)
            //    {
            //        //  取消的话，啥都不管
            //        e.Cancel = true;

            //    }
            //    else if (DialogResult.Yes == result)
            //    {
            //        // 保存当前修改
            //        if (!DoSave())
            //        {
            //            e.Cancel = true; //保存失败，不允许退出
            //        }

            //    }
            //    else if (DialogResult.No == result)
            //    {
            //        // 不保存
            //    }
            //}

        }



        #endregion 事件响应

        #region 帮助方法

        private void InitUI()
        {
            this.SuspendLayout();
            //tsButRevert.Enabled = false;
            //tsButAddStation.Enabled = false;
            //tsButDelete.Enabled = false;
            m_dgvMap = new CDataGridViewWaterFlowMap();
            m_dgvMap.Dock = DockStyle.Fill;
            m_dgvMap.AllowUserToAddRows = false;
            m_dgvMap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
           // m_dgvMap.AutoSize = true;
            m_dgvMap.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvMap.RowHeadersWidth = 50;
            m_dgvMap.ColumnHeadersHeight = 25;
            // m_dgvMap.SetEditMode(true);
            m_dgvMap.SetEditMode(false);
            panel_Right.Controls.Add(m_dgvMap);

            // 初始化所有站点
            // 格式化方便取得ID
            foreach (CEntityStation station in m_listStation)
            {
                listBox_StationName.Items.Add(GetDisplayStationName(station));
            }

            this.cmb_SubCenter.SelectedIndexChanged += new EventHandler(EHSubCenterChanged);
            this.ResumeLayout(false);
        }

        private void InitDataSource()
        {
            m_proxyWaterFlowMap = CDBDataMgr.Instance.GetWaterFlowMapProxy();

            //  只加载水文站和水位站，不加载雨量站等其他类型的站点
            var lists = CDBDataMgr.Instance.GetAllStation();
            foreach (var item in lists)
            {
                if (item.StationType == EStationType.EHydrology || item.StationType == EStationType.ERiverWater)
                {
                    m_listStation.Add(item);
                }
            }
            // 下面这一句是深拷贝？？
            m_mapStationWaterFlow =
                new Dictionary<string, List<CEntityWaterFlowMap>>(CDBDataMgr.Instance.GetStationWaterFlowMap());
            m_strCurrentStationId = null;
            m_mapStationDGVStatus = new Dictionary<string, CDataGridViewWaterFlowMap.SStatus>();
            m_mapModifiedData = new Dictionary<string, CDataGridViewWaterFlowMap.SModifiedData>();

            List<CEntitySubCenter> listSubCenter = CDBDataMgr.Instance.GetAllSubCenter();

            cmb_SubCenter.Items.Add(CS_All_Station);
            for (int i = 0; i < listSubCenter.Count; ++i)
            {
                cmb_SubCenter.Items.Add(listSubCenter[i].SubCenterName);
            }
            this.cmb_SubCenter.SelectedIndex = 0;
        }

        private string GetDisplayStationName(CEntityStation station)
        {
            return string.Format("({0,-4}|{1})", station.StationID, station.StationName);
        }

        private void GenerateSaveData()
        {
            try
            {
                // 遍历Map，提取出修改的数据
                m_listAdded = new List<CEntityWaterFlowMap>();
                m_listDeleted = new List<long>();
                m_listUpated = new List<CEntityWaterFlowMap>();
                // m_listDeleteByUser = new List<CEntityWaterFlowMap>();
                foreach (KeyValuePair<string, CDataGridViewWaterFlowMap.SModifiedData> entity in m_mapModifiedData)
                {
                    m_listAdded.AddRange(entity.Value.listAdded);
                    m_listDeleted.AddRange(entity.Value.listDeleted);
                    m_listUpated.AddRange(entity.Value.listUpdated);
                }
                // 清空内容
                m_mapModifiedData.Clear();
            }
#pragma warning disable CS0168 // 声明了变量“exp”，但从未使用过
            catch (Exception exp) { }
#pragma warning restore CS0168 // 声明了变量“exp”，但从未使用过
        }

        private bool DoSave()
        {
            CMessageBox box = new CMessageBox() { MessageInfo = "正在保存" };
            box.ShowDialog(this);
            bool result = true;
            //if (m_listImport.Count > 0)
            //{
            //    result = result && m_proxyWaterFlowMap.AddRange(m_listImport);
            //}
            if (m_listAdded.Count > 0)
            {
                result = result && m_proxyWaterFlowMap.AddRange(m_listAdded);
                m_listImport.Clear();
            }
            if (m_listUpated.Count > 0)
            {
                result = result && m_proxyWaterFlowMap.UpdateRange(m_listUpated);
            }
            if (m_listDeleted.Count > 0)
            {
                //       result = result && m_proxyWaterFlowMap.DeleteRange(m_listDeleted);
            }
            box.CloseDialog();
            if (result)
            {
                CMessageBox box2 = new CMessageBox();
                box2.MessageInfo = "正在更新数据";
                box2.ShowDialog(this);
                // 清空所有内容,和状态
                m_mapStationDGVStatus.Clear();
                // 通知DataMgr修改
                CDBDataMgr.Instance.UpdateStationWaterFlowMap();
                box2.CloseDialog();
                MessageBox.Show("保存成功");
            }
            else
            {
                MessageBox.Show("保存失败");
            }
            return result;
        }

        /// <summary>
        /// 处理单行数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool DealWithLineData(string str, int lineNumber, ref string strErroInfo, ref string stationidstr)
        {
            //if (str.StartsWith("(") && str.EndsWith(")"))
            //{
            // 按照逗号分隔每个数值，第一个是站号，第二个是水位，第三个是流量
            //str = str.Substring(1, str.Length - 2);
            string[] results = str.Split(',');
            if (11 == results.Length)
            {
                try
                {
                    // 刚好11个的话，正好
                    string stationId = results[0].Trim();
                    //BGTM
                    DateTime timeTmp = DateTime.Parse(results[1].Trim());

                    if (lineNumber == 1)
                    {
                        lastTime = timeTmp;

                        CEntityWaterFlowMap entity1 = new CEntityWaterFlowMap();
                        entity1.StationID = stationId;
                        entity1.BGTM = timeTmp;
                        m_proxyWaterFlowMap.DeleteLine(entity1);
                        stationidstr = stationId;
                    }
                    else
                    {
                        if (lastTime != timeTmp)
                        {
                            strErroInfo = string.Format("行：{0} 时间数据错误", lineNumber);
                            return false;
                        }
                        if (stationidstr != stationId)
                        {
                            strErroInfo = string.Format("行：{0} 站号不一致", lineNumber);
                            return false;
                        }
                    }

                    //PTNO
                    int ptno = int.Parse(results[2].Trim());
                    if (ptno != lineNumber)
                    {
                        strErroInfo = string.Format("行：{0} 点序列数据错误", lineNumber);
                        return false;
                    }
                    decimal q1 = -1, q2 = -1, q3 = -1, q4 = -1, q5 = -1, q6 = -1;
                    int count = 0;
                    // ZR
                    decimal zr = decimal.Parse(results[3].Trim());

                    //Q1
                    if (results[4].Trim() != "")
                    {
                        q1 = decimal.Parse(results[4].Trim());
                        count++;
                    }
                    if (results[5].Trim() != "")
                    {
                        q2 = decimal.Parse(results[5].Trim());
                        count++;
                    }
                    //Q3
                    if (results[6].Trim() != "")
                    {
                        q3 = decimal.Parse(results[6].Trim());
                        count++;
                    }
                    //Q4
                    if (results[7].Trim() != "")
                    {
                        q4 = decimal.Parse(results[7].Trim());
                        count++;
                    }
                    //Q5
                    if (results[8].Trim() != "")
                    {
                        q5 = decimal.Parse(results[8].Trim());
                        count++;
                    }
                    //Q6
                    if (results[9].Trim() != "")
                    {
                        q6 = decimal.Parse(results[9].Trim());
                        count++;
                    }
                    //CURRQ
                    int currQ = int.Parse(results[10].Trim());
                    if (currQ > count)
                    {
                        strErroInfo = string.Format("行：{0} 流量数据错误", lineNumber);
                        return false;
                    }
                    if (lineNumber == 1)
                    {
                        lastCurrQ = currQ;
                    }
                    else
                    {
                        if (lastCurrQ != currQ)
                        {
                            strErroInfo = string.Format("行：{0} 流量数据错误", lineNumber);
                            return false;
                        }
                    }
                    //Decimal waterStage = Decimal.Parse(results[1]);
                    //Decimal waterFlow = Decimal.Parse(results[2]);
                    if (null == CDBDataMgr.Instance.GetStationById(stationId))
                    {
                        strErroInfo = string.Format("行：{0} 未知站点\"{2}\"", lineNumber, stationId);
                        return false;
                    }
                    CEntityWaterFlowMap entity = new CEntityWaterFlowMap();
                    entity.StationID = stationId;
                    entity.BGTM = timeTmp;
                    entity.PTNO = ptno;
                    entity.ZR = zr;
                    entity.Q1 = q1;
                    entity.Q2 = q2;
                    entity.Q3 = q3;
                    entity.Q4 = q4;
                    entity.Q5 = q5;
                    entity.Q6 = q6;
                    
                    //entity.Q2 = q2;
                    //entity.Q3 = q3;
                    //entity.Q4 = q4;
                    //entity.Q5 = q5;
                    //entity.Q6 = q6;
                    entity.currQ = currQ;
                    m_listImport.Add(entity);
                    //entity.WaterStage = waterStage;
                    //entity.WaterFlow = waterFlow;
                    entity.RecordId = -1;
                    // 写入内存，如果之间不编辑，就直接保存，这个ModifiedData就有用了
                    if (m_mapModifiedData.ContainsKey(stationId))
                    {
                        m_mapModifiedData[stationId].listAdded.Add(entity);
                    }
                    else
                    {
                        // 新建一个
                        CDataGridViewWaterFlowMap.SModifiedData data = new CDataGridViewWaterFlowMap.SModifiedData();
                        data.listAdded = new List<CEntityWaterFlowMap>();
                        data.listUpdated = new List<CEntityWaterFlowMap>();
                        data.listDeleted = new List<long>();
                        data.listAdded.Add(entity);
                        m_mapModifiedData.Add(stationId, data);
                    }
                    // 写入表格,伪装成点击按钮添加的结果
                    m_mapStationWaterFlow[stationId].Add(entity);
                    if (m_mapStationDGVStatus.ContainsKey(stationId))
                    {
                        // 添加到最后一个
                        m_mapStationDGVStatus[stationId].listEditedRows.Add(m_mapStationWaterFlow[stationId].Count - 1);
                    }
                    else
                    {
                        // 新建一个表格状态
                        CDataGridViewWaterFlowMap.SStatus status = new CDataGridViewWaterFlowMap.SStatus();
                        status.listDoDeletes = new List<long>();
                        status.listEditedRows = new List<int>();
                        status.listMarkDeleteRows = new List<int>();
                        status.listEditedRows.Add(m_mapStationWaterFlow[stationId].Count - 1);
                        m_mapStationDGVStatus.Add(stationId, status);
                    }
                    return true;
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    strErroInfo = string.Format("行：{0} 数据格式错误", lineNumber);
                    return false;
                }

            }
            else
            {
                strErroInfo = string.Format("行：{0} 数据格式错误", lineNumber);
                return false;
            }

            //}
            //else
            //{
            //    // 格式不对
            //    strErroInfo = (string.Format("行：{0} 开始结束符号\"(\"\")\"格式错误", lineNumber));
            //    return false;
            //}
        }

        private void EHSubCenterChanged(object sender, EventArgs e)
        {
            int selectindex = cmb_SubCenter.SelectedIndex;
            if (0 == selectindex)
            {
                //     m_dgvMap.SetSubCenterName(null); //所有分中心
                listBox_StationName.Items.Clear();
                List<CEntityStation> listAllStation = CDBDataMgr.Instance.GetAllStation();
                //   m_dgvStatioin.SetSubCenterName(null); //所有分中心
                for (int i = 0; i < listAllStation.Count; ++i)
                {
                    if (listAllStation[i].StationType == EStationType.EHydrology || listAllStation[i].StationType == EStationType.ERiverWater)
                    {
                        listBox_StationName.Items.Add(string.Format("({0,-4}|{1})", listAllStation[i].StationID, listAllStation[i].StationName));
                    }
                }
            }
            else
            {
                string subcentername = cmb_SubCenter.Text;

                listBox_StationName.Items.Clear();

                // 根据分中心查找测站
                List<CEntityStation> listAllStation = CDBDataMgr.Instance.GetAllStation();
                CEntitySubCenter subcenter = CDBDataMgr.Instance.GetSubCenterByName(subcentername);
                if (null != subcenter)
                {
                    // 如果不为空           
                    for (int i = 0; i < listAllStation.Count; ++i)
                    {
                        if (listAllStation[i].SubCenterID == subcenter.SubCenterID)
                        {
                            if (listAllStation[i].StationType == EStationType.EHydrology || listAllStation[i].StationType == EStationType.ERiverWater)
                            {
                                listBox_StationName.Items.Add(string.Format("({0,-4}|{1})", listAllStation[i].StationID, listAllStation[i].StationName));
                            }
                        }
                    }
                    //      this.cmbStation.Text = this.cmbStation.m_listBoxStation.Items[0].ToString();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("分中心 Error");
                }
            }
            //    this.labelUserCount.Text = string.Format("共{0}个站点", m_dgvStatioin.Rows.Count);
        }

        #endregion 帮助方法
    }
}
