using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Hydrology.DataMgr;
using Hydrology.Entity;
using System.Windows.Forms;
using Hydrology.Utils;
using Hydrology.Forms;

namespace Hydrology.CControls
{
    class CDataGridViewRainManage : CExDataGridView
    {
         #region 静态常量
        private static readonly string CS_Select = "选择测站";
        private static readonly string CS_StationID = "站号";
        private static readonly string CS_StationName = "站名";
        private static readonly string CS_StationGprs = "用户ID";
        private static readonly string CS_SubCenterName = "所属分中心";
#pragma warning disable CS0414 // 字段“CDataGridViewRainManage.CS_OnlineOrOffline”已被赋值，但从未使用过它的值
        private static readonly string CS_OnlineOrOffline = "在线状态";
#pragma warning restore CS0414 // 字段“CDataGridViewRainManage.CS_OnlineOrOffline”已被赋值，但从未使用过它的值
        private static readonly string CS_Rain = "雨量";
       // private static readonly string CS_CurrentState = "当前状态";
        #endregion ///<静态常量

       // public string cS_GprsOnlineOrOff = "--";
        List<CEntitySubCenter> m_listSubCenter;
        private ToolStripMenuItem m_itemOnlineFirst;
#pragma warning disable CS0169 // 从不使用字段“CDataGridViewRainManage.m_itemOfflineFirst”
        private ToolStripMenuItem m_itemOfflineFirst;
#pragma warning restore CS0169 // 从不使用字段“CDataGridViewRainManage.m_itemOfflineFirst”
        private ToolStripMenuItem m_itemRecoverDefault; //恢复默认

        public CDataGridViewRainManage()
            : base()
        {
            InitDataGridView();
            m_listSubCenter = CDBDataMgr.GetInstance().GetAllSubCenter();
            CProtocolEventManager.DownForUI += this.DownForUI_EventHandler_3;
        }

        /// <summary>
        /// 选择所有或者全不选
        /// </summary>
        /// <param name="bSelectedAll"></param>
        /// <returns></returns>
        public bool SelectAllOrNot(bool bSelectedAll)
        {
            m_mutexDataTable.WaitOne();
            if (bSelectedAll)
            {
                for (int i = 0; i < m_dataTable.Rows.Count; ++i)
                {
                    m_dataTable.Rows[i][CS_Select] = "True";
                }
            }
            else
            {
                for (int i = 0; i < m_dataTable.Rows.Count; ++i)
                {
                    m_dataTable.Rows[i][CS_Select] = "False";
                }
            }
            m_mutexDataTable.ReleaseMutex();
            base.UpdateDataToUI();
            return true;
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        private void InitDataGridView()
        {
            // 设定标题栏,默认有个隐藏列
            this.Header = new string[] 
                { 
                    CS_Select, 
                    CS_StationID,
                    CS_StationName,
                    CS_StationGprs,
                    CS_SubCenterName,
                 //   CS_OnlineOrOffline,
                    CS_Rain,
                };
            var delCol = new DataGridViewCheckBoxColumn();
            base.SetColumnEditStyle(0, delCol);
        }

        /// <summary>
        /// 初始化站点
        /// </summary>
        /// <param name="listStation"></param>
        private void SetStationData(List<CEntityStation> listStation)
        {
            base.ClearAllRows();// 清空所有行数据
            List<string[]> listRows = new List<string[]>();
            List<CExDataGridView.EDataState> listStates = new List<CExDataGridView.EDataState>();
            for (int i = 0; i < listStation.Count; ++i)
            {
                if (String.IsNullOrEmpty(listStation[i].GPRS))
                {
                    continue;
                }
                CEntitySubCenter succenter = CDBDataMgr.Instance.GetSubCenterById(listStation[i].SubCenterID.Value);
                string subCenterName = "";
                if (succenter != null)
                {
                    subCenterName = succenter.SubCenterName;
                }
                //  默认是准备就绪状态
                listRows.Add(new string[] {
                    "False",
                    listStation[i].StationID,
                    listStation[i].StationName,
                    listStation[i].GPRS,
                    subCenterName,
               //     "--",
                    ""
                  //  GetStatusUIStr(EStationClockState.EReady)
                });
                listStates.Add(EDataState.ENormal);
            }
            base.AddRowRange(listRows, listStates);
        }

        /// <summary>
        /// 根据分中心站点来加载测站,如果为空，或者NULL,则加载所有分中心
        /// </summary>
        public void SetSubCenterName(string subcenterName)
        {
            if (subcenterName == null || subcenterName.Equals(""))
            {
                // 加载所有的用户分中心
                List<CEntityStation> listStation = CDBDataMgr.Instance.GetAllStation();
                SetStationData(listStation);
            }
            else
            {
                // 根据分中心查找测站
                List<CEntityStation> listAllStation = CDBDataMgr.Instance.GetAllStation();
                CEntitySubCenter subcenter = CDBDataMgr.Instance.GetSubCenterByName(subcenterName);
                if (null != subcenter)
                {
                    // 如果不为空
                    List<CEntityStation> listUseStation = new List<CEntityStation>();
                    for (int i = 0; i < listAllStation.Count; ++i)
                    {
                        if (listAllStation[i].SubCenterID == subcenter.SubCenterID && (listAllStation[i].StationType == EStationType.EHydrology || listAllStation[i].StationType == EStationType.ERainFall))
                        {
                            listUseStation.Add(listAllStation[i]);
                        }
                    }
                    this.SetStationData(listUseStation);
                }
                else
                {
                 //   System.Diagnostics.Debug.WriteLine("CDataGridViewSytemClock SetSubCenterName Error");
                    System.Diagnostics.Debug.WriteLine("CDataGridViewStorageWater SetSubCenterName Error");
                }
            }
            this.UpdateDataToUI();
        }

        /// <summary>
        /// 获取所有查询的测站
        /// </summary>
        /// <returns></returns>
        public List<CEntityStation> GetAllSelectedStation()
        {
            List<CEntityStation> listResults = new List<CEntityStation>();
            base.m_mutexDataTable.WaitOne();
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                // 获取编辑过的每一行
                string strChecked = m_dataTable.Rows[i][CS_Select].ToString();
                if (strChecked == "True")
                {
                    string stationId = m_dataTable.Rows[i][CS_StationID].ToString();
                    CEntityStation station = CDBDataMgr.Instance.GetStationById(stationId);
                    if (null != station)
                    {
                        listResults.Add(station);
                    }
                    //应该都会存在的
                }
                else
                {
                    // 没有选中
                }
            }
            base.m_mutexDataTable.ReleaseMutex();
            return listResults;
        }

        //public void RefreshGPRSInfo(List<CEntityStation> stations)
        //{
        //    if (null == stations || stations.Count == 0)
        //    {
        //        return;
        //    }
        //    // 判断是否在列中存在，如果不存在，则新建列
        //    // 先找到ID所在的行
        //    m_mutexDataTable.WaitOne();
        //    foreach (var station in stations)
        //    {
        //        CEntitySubCenter subcenter = CDBDataMgr.Instance.GetSubCenterById(station.SubCenterID.Value);
        //        string subCenterName = "";
        //        if (subcenter != null)
        //        {
        //            subCenterName = subcenter.SubCenterName;
        //        }

        //        if (!string.IsNullOrEmpty(station.GPRS))
        //        {
        //            // this.m_totalGprsCount += 1;
        //            base.AddRow(new string[] { "False", station.StationID, station.StationName, station.GPRS, subCenterName, "--", "" }, EDataState.ENormal);
        //        }
        //    }
        //    m_mutexDataTable.ReleaseMutex();
        //}

        //public void RefreshGPRSInfo(ushort port, string stationName, ModemInfoStruct dtu)
        //{
        //    string uid = ((uint)dtu.m_modemId).ToString("X").PadLeft(8, '0');
        //    string phoneno = CGprsUtil.Byte11ToPhoneNO(dtu.m_phoneno, 0);
        //    string dynIP = CGprsUtil.Byte4ToIP(dtu.m_dynip, 0);
        //    DateTime connetime = CGprsUtil.ULongToDatetime(dtu.m_conn_time);
        //    DateTime refreshTime = CGprsUtil.ULongToDatetime(dtu.m_refresh_time);
        //    //如果计算机当前时间与刷新时间间隔超过一小时，标记为红色
        //    //现在改为10分钟，不在线，红色标记
        //    EDataState state = EDataState.ENormal;

        //    //  if ((DateTime.Now - refreshTime).TotalMinutes > 60)
        //    //超过10分钟，表示不在线
        //    if ((DateTime.Now - refreshTime).TotalMinutes > 10)
        //    {
        //        this.cS_GprsOnlineOrOff = "离线";
        //        //      state = EDataState.EError;
        //    }
        //    else
        //    {
        //        this.cS_GprsOnlineOrOff = "在线";
        //        //      this.m_onlineGprsCount += 1;
        //    }
        //    // 判断是否在列中存在，如果不存在，则新建列
        //    // 先找到ID所在的行
        //    m_mutexDataTable.WaitOne();
        //    for (int i = 0; i < base.m_dataTable.Rows.Count; ++i)
        //    {
        //        if (m_dataTable.Rows[i][CS_StationGprs].ToString() == uid)
        //        {
        //      //      m_mutexDataTable.ReleaseMutex();

        //            CEntityStation entity = CDBDataMgr.Instance.GetStationByGprs(uid);
        //            if (entity.StationID.ToString() != "")
        //            {
        //                // 找到匹配，更新行的内容
        //                base.UpdateRowData(i, new string[] { "False", entity.StationID, stationName, uid, m_listSubCenter[int.Parse(entity.SubCenterID.ToString()) - 1].SubCenterName.ToString(), cS_GprsOnlineOrOff.ToString(), "" }, state);

        //                CBatchManagement.ListOnlineStation3.Add(CDBDataMgr.Instance.GetStationByGprs(uid));
        //            }
        //            return;
        //        }
        //    }
        //    // 没有找到匹配，添加新的行记录
        //    m_mutexDataTable.ReleaseMutex();
        //    //CEntityStation entity1 = CDBDataMgr.Instance.GetStationByGprs(uid);
        //    //base.AddRow(new string[] { "False", entity1.StationID, stationName, uid, m_listSubCenter[int.Parse(entity1.SubCenterID.ToString()) - 1].SubCenterName.ToString(), cS_GprsOnlineOrOff.ToString(), "" }, state);
        //    //CBatchManagement.ListOnlineStation3.Add(entity1);
        //    // this.m_totalGprsCount += 1;
        //}

        #region 重写
        protected override void OnSizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(sender, e);
            this.Columns[0].Width = 80;//选择
        }
        #endregion 重写

        private void DownForUI_EventHandler_3(object sender, DownEventArgs e)
        {
            try
            {
                CDownConf info = e.Value;
                //$60031G12000828
                //$60121G020845
                string rawData = e.RawData.Replace(" " , "");
                if (info == null)
                    return;
                string stationid = rawData.Substring(1, 4);
                string type = rawData.Substring(5, 4);
                string data = rawData.Substring(9, 4);
              //  if (this.IsHandleCreated && type=="1G02")
                if (type == "1G02")
                {
                    #region 更新UI
                    if (info.Rain.HasValue)
                    {
                        try
                        {
                            UpdateRainStorage(stationid, data);
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    #endregion
                }
            }
            catch (Exception exp) { Debug.WriteLine(exp.Message); }
        }

        /// 更新站点状态
        /// </summary>
        /// <returns></returns>
        public bool UpdateRainStorage(string stationId, string data)
        {
            base.m_mutexDataTable.WaitOne();
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                if (stationId == m_dataTable.Rows[i][CS_StationID].ToString())
                {
                    // 如果找到匹配
                    EDataState showstate = CExDataGridView.EDataState.ENormal;
                    //if (state == Hydrology.CControls.CDataGridViewSystemClock.EStationClockState.EAdjustFailed)
                    //{
                    //    // 对时失败，显示成红色
                    //    showstate = CExDataGridView.EDataState.EError;
                    //}
                    //else if (state == Hydrology.CControls.CDataGridViewSystemClock.EStationClockState.EAjustSuccess)
                    //{
                    //    showstate = CExDataGridView.EDataState.EPink;
                    //}
                    // $60191G021584
                    string raindata_1="";
                    try{
                        float raindata=float.Parse(data);
                        float a=CDBDataMgr.Instance.GetStationById(stationId).DRainAccuracy;
                        if(a.ToString()=="0.1"||a.ToString()=="0.2"||a.ToString()=="0.5"||a.ToString()=="1")
                            raindata_1 = (a * raindata).ToString();

#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
                    }catch(Exception ex){
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
                        raindata_1="--";
                    }
                    string[] newRow = new string[] 
                    { 
                        m_dataTable.Rows[i][CS_Select].ToString(), 
                        m_dataTable.Rows[i][CS_StationID].ToString(),
                        m_dataTable.Rows[i][CS_StationName].ToString(),
                        m_dataTable.Rows[i][CS_StationGprs].ToString(),
                        m_dataTable.Rows[i][CS_SubCenterName].ToString(),
                        raindata_1
                        //float.Parse(
                   //     m_dataTable.Rows[i][CS_OnlineOrOffline].ToString(),
                    //    float.Parse(data.Substring(0,3)).ToString()+"."+data.Substring(3,1).ToString()
                    };
                    base.m_mutexDataTable.ReleaseMutex();
                    base.UpdateRowData(i, newRow, showstate);
                    return true;
                }
            }
            return false;
        }

        // 初始化右键菜单项
        protected override void InitContextMenu()
        {
            base.InitContextMenu();

            // 添加警告优先，正常优先，以及筛选器选项
            m_itemOnlineFirst = new ToolStripMenuItem() { Text = "成功优先" };
            //    m_itemOfflineFirst = new ToolStripMenuItem() { Text = "离线优先" };
            m_itemRecoverDefault = new ToolStripMenuItem() { Text = "恢复默认" };
            // ToolStripMenuItem itemFilter = new ToolStripMenuItem() { Text = "筛选条件..." };

            ToolStripSeparator seperator = new ToolStripSeparator();
            m_contextMenu.Items.Add(seperator);
            m_contextMenu.Items.Add(m_itemOnlineFirst);
            //    m_contextMenu.Items.Add(m_itemOfflineFirst);
            m_contextMenu.Items.Add(m_itemRecoverDefault);
            seperator = new ToolStripSeparator();
            //m_contextMenu.Items.Add(seperator);
            //m_contextMenu.Items.Add(itemFilter);

            // 绑定消息
            m_itemOnlineFirst.Click += EHOnlineFirst;
            //m_itemOfflineFirst.Click += EHOfflineFirst;
            m_itemRecoverDefault.Click += EHRecoverDefault;
        }


        //在线优先事件
        public void EHOnlineFirst(object sender, EventArgs e)
        {
            //在线优先
            this.m_itemOnlineFirst.Checked = true;
            this.m_itemRecoverDefault.Checked = false;
            if (this.Columns.Count > 0)
            {
                // 排序
                this.Sort(this.Columns[CS_Rain], System.ComponentModel.ListSortDirection.Descending);
            }
        }

        //离线优先事件
        private void EHOfflineFirst(object sender, EventArgs e)
        {
            ////离线优先
            //m_itemOnlineFirst.Checked = false;
            //m_itemOfflineFirst.Checked = true;
            //m_itemRecoverDefault.Checked = false;
            //if (this.Columns.Count > 0)
            //{
            //    //for (int i = 0; i < this.RowCount; i++)
            //    //{
            //    //    if (this.Rows[i].Cells[8].Value.ToString() == "0")
            //    //    {
            //    //        this.Rows[i].Cells[8].Value = "3";
            //    //    }
            //    //}
            //    // 排序
            //    this.Sort(this.Columns[1], System.ComponentModel.ListSortDirection.Ascending);
            //}
        }

        //恢复默认
        public void EHRecoverDefault(object sender, EventArgs e)
        {
            //恢复默认
            m_itemOnlineFirst.Checked = false;
            m_itemRecoverDefault.Checked = true;
            if (this.Columns.Count > 0)
            {
                // 排序
                this.Sort(this.Columns[CS_Rain], System.ComponentModel.ListSortDirection.Ascending);
                //DataRow[] rows = base.m_dataTable.Select(string.Format("{0} LIKE '*'", CS_StationId), string.Format("Convert({0},'System.Int32') ASC", CS_StationId));
            }
        }
    }
}
