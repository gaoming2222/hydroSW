using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using Hydrology.DBManager.Interface;
using System.Windows.Forms;
using System.Diagnostics;
using Hydrology.DataMgr;
using Hydrology.Forms;
using Protocol.Channel.Interface;
using Protocol.Manager;

namespace Hydrology.CControls
{
    /// <summary>
    /// 墒情站点的DataGridView， 用于增加，删除和修改墒情站的参数
    /// </summary>
    class CDataGridViewSoilStation : CExDataGridView
    {
        #region 事件定义
        public event EventHandler<CEventSingleArgs<int>> E_SoilStationCountChanged;
        #endregion 事件定义

        #region 静态常量
        public static readonly string CS_Delete = "删除";
        public static readonly string CS_StationId = "站号";  // id是主键
        public static readonly string CS_StationName = "站名";
        public static readonly string CS_StationType = "站类";
        public static readonly string CS_StationSubcenter = "分中心";

        public static readonly string CS_DeviceNumber = "终端号";
        public static readonly string CS_VoltageMin = "电压阀值";
        public static readonly string CS_A10 = "10cm处a值";
        public static readonly string CS_B10 = "10cm处b值";
        public static readonly string CS_C10 = "10cm处c值";
        public static readonly string CS_D10 = "10cm处d值";
        public static readonly string CS_M10 = "10cm处m值";
        public static readonly string CS_N10 = "10cm处n值";

        public static readonly string CS_A20 = "20cm处a值";
        public static readonly string CS_B20 = "20cm处b值";
        public static readonly string CS_C20 = "20cm处c值";
        public static readonly string CS_D20 = "20cm处d值";
        public static readonly string CS_M20 = "20cm处m值";
        public static readonly string CS_N20 = "20cm处n值";

        public static readonly string CS_A30 = "30cm处a值";
        public static readonly string CS_B30 = "30cm处b值";
        public static readonly string CS_C30 = "30cm处c值";
        public static readonly string CS_D30 = "30cm处d值";
        public static readonly string CS_M30 = "30cm处m值";
        public static readonly string CS_N30 = "30cm处n值";

        public static readonly string CS_A40 = "40cm处a值";
        public static readonly string CS_B40 = "40cm处b值";
        public static readonly string CS_C40 = "40cm处c值";
        public static readonly string CS_D40 = "40cm处d值";
        public static readonly string CS_M40 = "40cm处m值";
        public static readonly string CS_N40 = "40cm处n值";

        public static readonly string CS_A60 = "60cm处a值";
        public static readonly string CS_B60 = "60cm处b值";
        public static readonly string CS_C60 = "60cm处c值";
        public static readonly string CS_D60 = "60cm处d值";
        public static readonly string CS_M60 = "60cm处m值";
        public static readonly string CS_N60 = "60cm处n值";

        public static readonly string CS_Gsm = "GSM号码";
        public static readonly string CS_Gprs = "GPRS号码";
        public static readonly string CS_BDsatellite = "北斗卫星终端号";
        public static readonly string CS_BDmember = "北斗卫星成员号";
        public static readonly string CS_Maintran = "主信道";
        public static readonly string CS_Subtran = "备信道";
        public static readonly string CS_Dataprotocol = "数据协议";
        public static readonly string CS_Reportinterval = "报汛段次";
        public static readonly string CS_PreStationId_None = "---";

        #endregion 静态常量

        #region 成员变量
        public IStationProxy m_proxyStation;
        public ISoilStationProxy m_proxySoilStation;   // 数据库连接的代理
        private List<CEntitySoilStation> m_listUpdatedSoilStation;    // 更新墒情站的集合
        private List<string> m_listDeletedStation;                       // 删除墒情站的集合
        public List<CEntitySoilStation> m_listAddedSoilStation;      // 添加墒情站的集合

        List<CEntitySubCenter> m_listSubCenter;

        /// <summary>
        /// 所有数据协议列表
        /// </summary>
        private List<string> m_listProtocolData;
        private XmlDllCollections m_dllCollections;
        #endregion ///<成员变量

        #region 公共方法
        public CDataGridViewSoilStation()
            : base()
        {
            // 关闭分页
            base.BPartionPageEnable = false;
            m_listAddedSoilStation = new List<CEntitySoilStation>();
            m_listDeletedStation = new List<string>();
            m_listUpdatedSoilStation = new List<CEntitySoilStation>();
        }

        /// <summary>
        /// 初始化数据源，连接数据库
        /// </summary>
        /// <param name="proxy"></param>
        public void InitDataSource(ISoilStationProxy proxy)
        {
            m_proxyStation = CDBDataMgr.GetInstance().GetStationProxy();
            m_proxySoilStation = proxy;
            m_listSubCenter = CDBDataMgr.GetInstance().GetAllSubCenter();
        }
        /// <summary>
        /// 将数据标记为删除，从集合中删除当前行
        /// </summary>
        public void DoDelete()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            //GetUpdatedData();
            // 获取数据
            for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
            {
                // -1表示新添加的用户
                string strPreStationId = base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_StationId].Value.ToString();
                // 并且从编辑项中减去这一行
                m_listEditedRows.Remove(m_listMaskedDeletedRows[i]);
                if (strPreStationId != CS_PreStationId_None)
                {
                    m_listDeletedStation.Add(strPreStationId);
                }
            }
            // 将某些行标记为不可见
            for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
            {
                //base.Rows[i].Visible = false;
                base.DeleteRowData(m_listMaskedDeletedRows[i]);
            }
            m_listMaskedDeletedRows.Clear(); //清空
            base.UpdateDataToUI();
            UpdateSoilStationCount(base.m_dataTable.Rows.Count);

        }

        public override bool DoSave()
        {
            try
            {
                if (this.IsCurrentCellInEditMode)
                {
                    //MessageBox.Show("请完成当前的编辑");
                    this.EndEdit();
                    //return false;
                }

                GetUpdatedData();
                if (!AssertInputData(m_listUpdatedSoilStation))
                {
                    return false;
                }

                GetDeletedData();

                if ((m_listAddedSoilStation.Count > 0 || m_listDeletedStation.Count > 0 || m_listUpdatedSoilStation.Count > 0) &&
                    (IsDataChanged()))
                {
                    // 应该作为一个事物一起处理
                    //return base.DoSave();
                    // 删除
                    bool bResults = m_proxySoilStation.DeleteSoilStationRange(m_listDeletedStation);
                    // 增加
                    bResults = bResults && m_proxySoilStation.AddSoilStationRange(m_listAddedSoilStation);
                    // 更新
                    bResults = bResults && m_proxySoilStation.UpdateSoilStation(m_listUpdatedSoilStation);

                    if (bResults)
                    {
                        this.Hide();
                        MessageBox.Show("保存成功", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Show();
                    }
                    else
                    {
                        this.Hide();
                        MessageBox.Show("保存失败,请先删除该墒情站相应的数据！", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Show();
                        return false;
                    }
                    // 重新加载
                    CDBSoilDataMgr.Instance.ReloadSoilStation();
                    CDBSoilDataMgr.Instance.UpdateAllSoilStation();
                    ClearAllState();
                    this.SuspendLayout();//暂停VIEW的刷新（datagridview的方法
                    this.Hide();
                    LoadData();

                    this.Show();
                    this.ResumeLayout();
                    UpdateDataToUI();
                }
                else
                {
                    //this.Revert();
                    this.Hide();
                    MessageBox.Show("没有任何修改，无需保存");
                    this.Show();
                }
                return true;
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
                this.Hide();
                MessageBox.Show("请在同一界面完成修改");
                this.Show();
                return false;
            }
        }

        /// <summary>
        /// 关闭事件，是否取消关闭
        /// </summary>
        /// <returns>false的话，表示取消关闭</returns>
        public bool Close()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            // 关闭方法
            if ((base.m_listEditedRows.Count > 0 || m_listUpdatedSoilStation.Count > 0 || base.m_listMaskedDeletedRows.Count > 0) && (base.IsDataChanged()))
            {
                this.Hide();
                DialogResult result = MessageBox.Show("当前所做修改尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DialogResult.Cancel == result)
                {
                    this.Show();
                    return false;
                }
                else if (DialogResult.Yes == result)
                {
                    this.Show();
                    // 保存当前修改
                    return DoSave();
                }
                else if (DialogResult.No == result)
                {
                    this.Show();
                    return true;
                }
                this.Show();
            }
            return true;
        }

        public void Revert()
        {
            // 清空所有状态
            this.ClearAllState();
            this.Hide();
            this.LoadData();
            this.Show();
        }


        public bool LoadData()
        {
            if (m_proxySoilStation != null)
            {
                //this.SetSoilStation(m_proxySoilStation.QueryAllSoilStation());
                this.SetSoilStation(CDBSoilDataMgr.Instance.GetAllSoilStationData());
                return true;
            }
            return false;
        }


        /// <summary>
        /// 是否开启编辑模式
        /// </summary>
        /// <param name="bEnable"></param>
        public void SetEditMode(bool bEnable)
        {
            if (bEnable)
            {
                // 设定标题栏,默认有个隐藏列
                this.Header = new string[]
                {
                    CS_Delete, CS_StationId, CS_StationName,CS_StationType,CS_StationSubcenter,
                    CS_Gsm,CS_Gprs,CS_BDsatellite,CS_BDmember,CS_Maintran,CS_Subtran,CS_Dataprotocol,CS_Reportinterval,
                    CS_VoltageMin,
                    CS_A10, CS_B10, CS_C10, CS_D10, CS_M10, CS_N10,
                    CS_A20, CS_B20, CS_C20, CS_D20, CS_M20, CS_N20,
                    CS_A30, CS_B30, CS_C30, CS_D30, CS_M30, CS_N30,
                    CS_A40, CS_B40, CS_C40, CS_D40, CS_M40, CS_N40,
                    CS_A60, CS_B60, CS_C60, CS_D60, CS_M60, CS_N60,

                };
                base.HideColomns = new int[] { 14, 15, 16, 17, 20, 21, 22, 23, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 38, 39, 40, 41, 42, 43 };
                List<string> listHeader = new List<string>(this.Header);
                //开启编辑模式,设置可编辑列
                DataGridViewCheckBoxColumn deleteCol = new DataGridViewCheckBoxColumn();
                base.SetColumnEditStyle(listHeader.IndexOf(CS_Delete), deleteCol);


                //// 设置站名可编辑
                DataGridViewTextBoxColumn stationNumber = new DataGridViewTextBoxColumn();
                base.SetColumnEditStyle(listHeader.IndexOf(CS_StationName), stationNumber);

                ////// 设置站点类型下拉列表
                //var cmb_StationType = new DataGridViewComboBoxColumn();
                //cmb_StationType.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                //    cmb_StationType.Items.Add(CS_None);

                ////// 初始化站点类型
                //cmb_StationType.Items.Add(CEnumHelper.StationTypeToUIStr(EStationType.ESoil));
                //cmb_StationType.Items.Add(CEnumHelper.StationTypeToUIStr(EStationType.ESoilHydrology));
                //cmb_StationType.Items.Add(CEnumHelper.StationTypeToUIStr(EStationType.ESoilRain));
                //cmb_StationType.Items.Add(CEnumHelper.StationTypeToUIStr(EStationType.ESoilWater));
                //base.SetColumnEditStyle(listHeader.IndexOf(CS_StationType), cmb_StationType);

                ////所属分中心下拉列表
                var subcenterId = new DataGridViewComboBoxColumn();
                subcenterId.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                List<CEntitySubCenter> listSubCenter = CDBDataMgr.GetInstance().GetAllSubCenter();
                // 初始化分中心
                foreach (CEntitySubCenter subcenter in listSubCenter)
                {
                    subcenterId.Items.Add(subcenter.SubCenterName);
                }
                //subcenterId.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                base.SetColumnEditStyle(listHeader.IndexOf(CS_StationSubcenter), subcenterId);

                //// 设置站点ID下拉列表
                //DataGridViewComboBoxColumn stationId = new DataGridViewComboBoxColumn();
                //stationId.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                //List<CEntityStation> listStation = CDBDataMgr.Instance.GetAllStation();
                //foreach (CEntityStation station in listStation)
                //{
                //    // 只添加墒情站点的类型
                //    if (station.StationType == EStationType.ESoil ||
                //        station.StationType == EStationType.ESoilHydrology ||
                //        station.StationType == EStationType.ESoilRain ||
                //        station.StationType == EStationType.ESoilWater)
                //    {
                //        stationId.Items.Add(station.StationID);
                //    }
                //}
                //base.SetColumnEditStyle(listHeader.IndexOf(CS_StationId), stationId);

                //// 设置设备号可编辑
                //DataGridViewTextBoxColumn deviceNumber = new DataGridViewTextBoxColumn();
                //base.SetColumnEditStyle(listHeader.IndexOf(CS_DeviceNumber), deviceNumber);
                base.SetColumnEditStyle(listHeader.IndexOf(CS_VoltageMin), GenerateColumnStype_1());
                //设置A10，B10，C10，D10的编辑列
                //水位编辑列

                base.SetColumnEditStyle(listHeader.IndexOf(CS_A10), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_B10), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_C10), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_D10), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_M10), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_N10), GenerateColumnStype());

                // 设置A20，B20，C20，D20的编辑列
                base.SetColumnEditStyle(listHeader.IndexOf(CS_A20), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_B20), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_C20), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_D20), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_M20), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_N20), GenerateColumnStype());

                // 设置A30，B30，C30，D30的编辑列
                base.SetColumnEditStyle(listHeader.IndexOf(CS_A30), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_B30), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_C30), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_D30), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_M30), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_N30), GenerateColumnStype());

                // 设置A40，B40，C40，D40的编辑列
                base.SetColumnEditStyle(listHeader.IndexOf(CS_A40), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_B40), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_C40), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_D40), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_M40), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_N40), GenerateColumnStype());

                // 设置A60，B60，C60，D60的编辑列
                base.SetColumnEditStyle(listHeader.IndexOf(CS_A60), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_B60), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_C60), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_D60), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_M60), GenerateColumnStype());
                base.SetColumnEditStyle(listHeader.IndexOf(CS_N60), GenerateColumnStype());

                DataGridViewTextBoxColumn gsmNumber = new DataGridViewTextBoxColumn();
                base.SetColumnEditStyle(listHeader.IndexOf(CS_Gsm), gsmNumber);

                //// 设置GPRS号码,北斗卫星终端号,北斗卫星成员号的编辑列
                DataGridViewTextBoxColumn gprsNumber = new DataGridViewTextBoxColumn();
                base.SetColumnEditStyle(listHeader.IndexOf(CS_Gprs), gprsNumber);

                DataGridViewTextBoxColumn beidouNumber = new DataGridViewTextBoxColumn();
                base.SetColumnEditStyle(listHeader.IndexOf(CS_BDsatellite), beidouNumber);

                DataGridViewTextBoxColumn beidouMemberNumber = new DataGridViewTextBoxColumn();
                base.SetColumnEditStyle(listHeader.IndexOf(CS_BDmember), beidouMemberNumber);


                //主信道,备信道，通信协议
                XmlDocManager.Instance.ReadFromXml();
                m_dllCollections = Protocol.Manager.XmlDocManager.Instance.DllInfo;
                var MaintranNumber = new DataGridViewComboBoxColumn();
                var SubtranNumber = new DataGridViewComboBoxColumn();
                MaintranNumber.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                SubtranNumber.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                foreach (XmlDllInfo info in m_dllCollections.Infos)
                {
                    //  不显示已经被禁用的协议
                    if (!info.Enabled)
                        continue;
                    // 显示信道协议
                    if (info.Type == "channel")
                    {
                        //   m_mapChannelInfo.Add(info.Name, info);
                        MaintranNumber.Items.Add(info.Name);
                        SubtranNumber.Items.Add(info.Name);
                        // comb_PrepareRoad.Items.Add(info.Name);
                    }

                }
                SubtranNumber.Items.Add("无");
                MaintranNumber.Items.Add("未知通讯协议");
                SubtranNumber.Items.Add("未知通讯协议");


                base.SetColumnEditStyle(listHeader.IndexOf(CS_Maintran), MaintranNumber);
                base.SetColumnEditStyle(listHeader.IndexOf(CS_Subtran), SubtranNumber);

                var DataprotocolNumber = new DataGridViewComboBoxColumn();
                DataprotocolNumber.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                m_listProtocolData = XmlDocManager.Instance.DataProtocolNames;
                for (int i = 0; i < m_listProtocolData.Count; i++)
                {
                    DataprotocolNumber.Items.Add(m_listProtocolData[i]);
                }
                base.SetColumnEditStyle(listHeader.IndexOf(CS_Dataprotocol), DataprotocolNumber);

                var ReportintervalNumber = new DataGridViewComboBoxColumn();
                ReportintervalNumber.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                ReportintervalNumber.Items.Add("1");
                ReportintervalNumber.Items.Add("4");
                ReportintervalNumber.Items.Add("8");
                ReportintervalNumber.Items.Add("12");
                ReportintervalNumber.Items.Add("24");
                ReportintervalNumber.Items.Add("48");
                base.SetColumnEditStyle(listHeader.IndexOf(CS_Reportinterval), ReportintervalNumber);
                ////// 设置删除列的宽度
                //////this.Columns[0].Width = 15; //删除列宽度为20
            }
            else
            {

            }

        }

        /// <summary>
        /// 根据分中心站点来加载测站,如果为空，或者NULL,则加载所有分中心
        /// </summary>
        public void SetSubCenterName(string subcenterName)
        {
            if (subcenterName == null || subcenterName.Equals(""))
            {
                // 加载所有的用户分中心
                List<CEntitySoilStation> listSoilStation = CDBSoilDataMgr.Instance.GetAllSoilStationData();
                this.SuspendLayout();//暂停VIEW的刷新（datagridview的方法
                this.Hide();
                SetSoilStation(listSoilStation);
                this.Show();
                this.ResumeLayout();
            }
            else
            {
                // 根据分中心查找测站
                List<CEntitySoilStation> listAllSoilStation = CDBSoilDataMgr.Instance.GetAllSoilStationData();
                CEntitySubCenter subcenter = CDBDataMgr.Instance.GetSubCenterByName(subcenterName);
                if (null != subcenter)
                {
                    // 如果不为空
                    List<CEntitySoilStation> listUseStation = new List<CEntitySoilStation>();
                    for (int i = 0; i < listAllSoilStation.Count; ++i)
                    {
                        if (listAllSoilStation[i].SubCenterID == subcenter.SubCenterID)
                        {
                            listUseStation.Add(listAllSoilStation[i]);
                        }
                    }
                    this.SuspendLayout();//暂停VIEW的刷新（datagridview的方法
                    this.Hide();
                    this.SetSoilStation(listUseStation);
                    this.Show();
                    this.ResumeLayout();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("测站管理分中心 Error");
                }
            }
            this.UpdateDataToUI();
        }
        #endregion 公共方法

        #region 重写
        // 重写Cell值改变事件
        protected override void EHValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int iPreValue = this.FirstDisplayedScrollingRowIndex;
            if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
            {
                // 删除列
                if (base.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Equals("True"))
                {
                    // 删除项
                    base.MarkRowDeletedOrNot(e.RowIndex, true);
                    //base.UpdateDataToUI();

                }
                else
                {
                    base.MarkRowDeletedOrNot(e.RowIndex, false);
                    //base.UpdateDataToUI();
                    //FocusOnRow(e.RowIndex);
                }
            }
            else if (base.m_arrayStrHeader[e.ColumnIndex] == CS_StationId)
            {
                // 站点id发生变化，站名跟着变化
                string stationId = base.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                CEntityStation station = CDBDataMgr.Instance.GetStationById(stationId);
                base.Rows[e.RowIndex].Cells[CS_StationName].Value = station.StationName;
            }
            else
            {
                // 非删除列
                base.EHValueChanged(sender, e);
            }
            base.UpdateDataToUI();
            //FocusOnRow(this.FirstDisplayedScrollingRowIndex);
            FocusOnRow(iPreValue, false);
            //base.Rows[e.RowIndex].Selected = true;
            //this.VerticalScrollBar.Value = iPreValue;
        }


        // 重写双击事件
        protected override void OnCellMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            if (base.m_listMaskedDeletedRows.Contains(e.RowIndex))
            {
                if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
                {
                    //开启编辑
                    base.OnCellMouseDoubleClick(e);
                }
                else
                {
                    //不编辑
                }
            }
            else
            {
                // 开启编辑
                base.OnCellMouseDoubleClick(e);
            }
        }

        // 单击事件
        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            if (base.m_listMaskedDeletedRows.Contains(e.RowIndex))
            {
                if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
                {
                    //开启编辑
                    base.OnCellClick(e);
                }
                else
                {
                    //不编辑
                }
            }
            else
            {
                // 开启编辑
                base.OnCellClick(e);
            }

        }
        #endregion 重写

        #region 帮助方法
        /// <summary>
        /// 设置显示的数据
        /// </summary>
        /// <param name="listSoilStation"></param>
        public void SetSoilStation(List<CEntitySoilStation> listSoilStation)
        {
            base.m_dataTable.Rows.Clear();
            XmlDocManager.Instance.ReadFromXml();
            m_dllCollections = Protocol.Manager.XmlDocManager.Instance.DllInfo;
            List<string> xmlDll = new List<string>();
            foreach (XmlDllInfo info in m_dllCollections.Infos)
            {
                xmlDll.Add(info.Name);
            }
            // 将用户信息显示到表格上
            foreach (CEntitySoilStation station in listSoilStation)
            {
                //var s = CDBDataMgr.Instance.GetStationById(station.StationID);
                string subcerterName = "";
                int reportInterval = 24;
                var s = station;
                // 1103 gm
                //限制分中心名称
                for (int i = 0; i < m_listSubCenter.Count; i++)
                {
                    if (s.SubCenterID == m_listSubCenter[i].SubCenterID)
                    {
                        subcerterName = m_listSubCenter[i].SubCenterName;
                        break;
                    }
                }
                //限制报讯段次
                try
                {
                    int report = int.Parse(s.Reportinterval);
                    if (report == 1 || report == 4 || report == 8 || report == 12 || report == 24 || report == 48)
                    {
                        reportInterval = report;
                    }
                }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                {
                    reportInterval = 24;
                }
                // 限制主信道
                if (!xmlDll.Contains(s.Maintran))
                {
                    s.Maintran = "未知通讯协议";
                }
                // 限制备用信道
                if (!xmlDll.Contains(s.Subtran) && s.Subtran != "无")
                {
                    s.Subtran = "未知通讯协议";
                }
                if (s.StationType == EStationType.ESoil ||
                    s.StationType == EStationType.ESoilHydrology ||
                    s.StationType == EStationType.ESoilRain ||
                    s.StationType == EStationType.ESoilWater)
                {

                    base.AddRow(new string[]
                    {
                        "False",s.StationID, s.StationName.ToString(),
                        //CEnumHelper.StationTypeToUIStr(s.StationType),m_listSubCenter[int.Parse(s.SubCenterID.ToString())-1].SubCenterName.ToString(),
                        CEnumHelper.StationTypeToUIStr(s.StationType),subcerterName,
                        station.GSM,station.GPRS,station.BDSatellite,station.BDMemberSatellite,station.Maintran,station.Subtran,station.Datapotocol,reportInterval.ToString(),
                        s.VoltageMin.ToString(),
                        station.A10.ToString(),station.B10.ToString(), station.C10.ToString(), station.D10.ToString(), station.M10.ToString(), station.N10.ToString(),
                        station.A20.ToString(),station.B20.ToString(), station.C20.ToString(), station.D20.ToString(), station.M20.ToString(), station.N20.ToString(),
                        station.A30.ToString(),station.B30.ToString(), station.C30.ToString(), station.D30.ToString(), station.M30.ToString(), station.N30.ToString(),
                        station.A40.ToString(),station.B40.ToString(), station.C40.ToString(), station.D40.ToString(), station.M40.ToString(), station.N40.ToString(),
                        station.A60.ToString(),station.B60.ToString(), station.C60.ToString(), station.D60.ToString(), station.M60.ToString(), station.N60.ToString(),

                    }, EDataState.ENormal);
                }
            }
            UpdateDataToUI();
            UpdateSoilStationCount(listSoilStation.Count);
        }

        /// <summary>
        /// 更新配置个数
        /// </summary>
        /// <param name="userCount"></param>
        private void UpdateSoilStationCount(int userCount)
        {
            if (E_SoilStationCountChanged != null)
            {
                E_SoilStationCountChanged.Invoke(this, new CEventSingleArgs<int>(userCount));
            }
        }

        /// <summary>
        /// 获取更新过的数据,包括增加的用户记录
        /// </summary>
        private void GetUpdatedData()
        {
            try
            {
                // 标记为删除的就不需要添加的修改或者添加的分中心中了
                List<int> listEditRows = new List<int>();
                foreach (int item in base.m_listEditedRows)
                {
                    if (!m_listMaskedDeletedRows.Contains(item))
                    {
                        listEditRows.Add(item);
                    }
                }
                // 将去重后的项赋给编辑项
                base.m_listEditedRows = listEditRows;
                for (int i = 0; i < base.m_listEditedRows.Count; ++i)
                {
                    CEntitySoilStation soilStation = new CEntitySoilStation();
                    soilStation.StationID = base.Rows[m_listEditedRows[i]].Cells[CS_StationId].Value.ToString();
                    soilStation.SubCenterID = CDBDataMgr.Instance.GetSubCenterByName(base.Rows[m_listEditedRows[i]].Cells[CS_StationSubcenter].Value.ToString()).SubCenterID;
                    // soilStation.SubCenterID = Int32.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_StationSubcenter].Value.ToString());
                    soilStation.StationName = base.Rows[m_listEditedRows[i]].Cells[CS_StationName].Value.ToString();
                    soilStation.VoltageMin = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_VoltageMin].Value.ToString());
                    soilStation.StationType = CEnumHelper.UIStrToStationType(base.Rows[m_listEditedRows[i]].Cells[CS_StationType].Value.ToString());


                    //  soilStation.StrDeviceNumber = base.Rows[m_listEditedRows[i]].Cells[CS_DeviceNumber].Value.ToString();
                    soilStation.A10 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_A10].Value.ToString());
                    soilStation.A20 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_A20].Value.ToString());
                    soilStation.A30 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_A30].Value.ToString());
                    soilStation.A40 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_A40].Value.ToString());
                    soilStation.A60 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_A60].Value.ToString());

                    soilStation.B10 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_B10].Value.ToString());
                    soilStation.B20 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_B20].Value.ToString());
                    soilStation.B30 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_B30].Value.ToString());
                    soilStation.B40 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_B40].Value.ToString());
                    soilStation.B60 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_B60].Value.ToString());

                    soilStation.C10 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_C10].Value.ToString());
                    soilStation.C20 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_C20].Value.ToString());
                    soilStation.C30 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_C30].Value.ToString());
                    soilStation.C40 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_C40].Value.ToString());
                    soilStation.C60 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_C60].Value.ToString());

                    soilStation.D10 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_D10].Value.ToString());
                    soilStation.D20 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_D20].Value.ToString());
                    soilStation.D30 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_D30].Value.ToString());
                    soilStation.D40 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_D40].Value.ToString());
                    soilStation.D60 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_D60].Value.ToString());

                    soilStation.M10 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_M10].Value.ToString());
                    soilStation.M20 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_M20].Value.ToString());
                    soilStation.M30 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_M30].Value.ToString());
                    soilStation.M40 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_M40].Value.ToString());
                    soilStation.M60 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_M60].Value.ToString());

                    soilStation.N10 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_N10].Value.ToString());
                    soilStation.N20 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_N20].Value.ToString());
                    soilStation.N30 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_N30].Value.ToString());
                    soilStation.N40 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_N40].Value.ToString());
                    soilStation.N60 = CStringFromatHelper.ConvertToNullableDecimal(base.Rows[m_listEditedRows[i]].Cells[CS_N60].Value.ToString());

                    soilStation.GSM = base.Rows[m_listEditedRows[i]].Cells[CS_Gsm].Value.ToString();
                    soilStation.GPRS = base.Rows[m_listEditedRows[i]].Cells[CS_Gprs].Value.ToString();
                    soilStation.BDSatellite = base.Rows[m_listEditedRows[i]].Cells[CS_BDsatellite].Value.ToString();
                    soilStation.BDMemberSatellite = base.Rows[m_listEditedRows[i]].Cells[CS_BDmember].Value.ToString();
                    soilStation.Maintran = base.Rows[m_listEditedRows[i]].Cells[CS_Maintran].Value.ToString();
                    soilStation.Subtran = base.Rows[m_listEditedRows[i]].Cells[CS_Subtran].Value.ToString();
                    soilStation.Datapotocol = base.Rows[m_listEditedRows[i]].Cells[CS_Dataprotocol].Value.ToString();
                    soilStation.Reportinterval = base.Rows[m_listEditedRows[i]].Cells[CS_Reportinterval].Value.ToString();

                    String preSoilStation = base.Rows[m_listEditedRows[i]].Cells[CS_StationId].Value.ToString();
                    if (preSoilStation == CS_PreStationId_None)
                    {
                        // 添加的新墒情站配置
                        m_listAddedSoilStation.Add(soilStation);
                    }
                    else
                    {
                        // 如果墒情站的测站id没有改变，那么是更新，如果发生了改变，那么需要先删除，然后添加新的
                        if (preSoilStation == soilStation.StationID)
                        {
                            m_listUpdatedSoilStation.Add(soilStation);
                        }
                        else
                        {
                            // 先删除之前的
                            m_listDeletedStation.Add(preSoilStation);
                            m_listAddedSoilStation.Add(soilStation); //然后再添加新的墒情站
                        }

                    }

                }
                m_listEditedRows.Clear();   //清空此次记录
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
                this.Hide();
                MessageBox.Show("请在同一界面完成修改");
                this.Show();
            }
        }

        /// <summary>
        /// 获取删除过的数据
        /// </summary>
        private void GetDeletedData()
        {
            try
            {
                for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
                {
                    // -1是新增加的用户
                    string strPreStationId = base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_StationId].Value.ToString();
                    if (CS_PreStationId_None != strPreStationId)
                    {
                        // 如果更改后的不一样站点的话，那么，也行吧
                        m_listDeletedStation.Add(strPreStationId);
                    }
                }
                m_listMaskedDeletedRows.Clear();   //清空此次记录
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
                this.Hide();
                MessageBox.Show("请在同一界面完成修改");
                this.Show();
            }
        }

        // 清空所有数据，恢复到刚加载的状态
        protected override void ClearAllState()
        {
            base.ClearAllState();
            m_listAddedSoilStation.Clear();
            m_listDeletedStation.Clear();
            m_listUpdatedSoilStation.Clear();
        }

        /// <summary>
        /// 判断数据数据是否合法
        /// </summary>
        /// <returns></returns>
        private bool AssertInputData(List<CEntitySoilStation> listStation)
        {
            List<int> listEditRows = new List<int>();
            foreach (int item in base.m_listEditedRows)
            {
                if (!m_listMaskedDeletedRows.Contains(item))
                {
                    listEditRows.Add(item);
                }
            }
            // 判断有没有不合法的数据
            for (int i = 0; i < listEditRows.Count; ++i)
            {
                if (base.Rows[m_listEditedRows[i]].Cells[CS_StationId].Value.ToString().Equals(""))
                {
                    this.Hide();
                    MessageBox.Show("测站不能为空");
                    this.Show();
                    return false;
                }
            }
            // 测站是否重复
            List<string> allSoilStation = new List<string>();
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                allSoilStation.Add(m_dataTable.Rows[i][CS_StationId].ToString());
            }

            for (int i = 0; i < listEditRows.Count; ++i)
            {
                string name = base.Rows[listEditRows[i]].Cells[CS_StationId].Value.ToString();
                if (allSoilStation.IndexOf(name) !=
                    allSoilStation.LastIndexOf(name))
                {
                    this.Hide();
                    MessageBox.Show(string.Format("不能重复设置站点\"{0}\"的参数", name));
                    this.Show();
                    return false;
                }
            }

            foreach (CEntitySoilStation station in listStation)
            {
                //  1.站名不能为空，不能超过50个字符
                if (station.StationName.Equals(""))
                {
                    this.Hide();
                    MessageBox.Show(station.StationID + "站名不能为空");
                    this.Show();
                    return false;
                }
                if (System.Text.Encoding.Default.GetByteCount(station.StationName) > 50)
                {
                    this.Hide();
                    MessageBox.Show(station.StationID + "站名不能超过50个字符");
                    this.Show();
                    return false;
                }
                // 判断GPRS是否重复
                //List<string> gprs = m_proxyStation.getAllGprs();
                //List<string> soilgprs = m_proxySoilStation.getAllGprs();
                //for (int i = 0; i < soilgprs.Count; i++)
                //{
                //    gprs.Add(soilgprs[i]);
                //}
                //if (station.GPRS != "" && gprs.Contains(station.GPRS))
                //{
                //    this.Hide();
                //    MessageBox.Show(station.StationID + "站点GPRS不能重复");
                //    this.Show();
                //    return false;
                //}
                //else
                //{
                //    gprs.Add(station.GPRS);
                //}
                //List<CEntityStation> gprsStation = m_proxyStation.getAllGprs_1();
                //List<CEntitySoilStation> gprsSoilStation = m_proxySoilStation.getAllGprs_1();
                List<CEntityStation> gprsStation = CDBDataMgr.Instance.GetAllStation();
                List<CEntitySoilStation> gprsSoilStation = CDBSoilDataMgr.Instance.GetAllSoilStation();
                List<String> gprs = new List<string>();
                for (int i = 0; i < gprsStation.Count; i++)
                {
                    gprs.Add(gprsStation[i].GPRS);
                }
                for (int j = 0; j < gprsSoilStation.Count; j++)
                {
                    if (gprsSoilStation[j].StationID != station.StationID)
                    {
                        gprs.Add(gprsSoilStation[j].GPRS);
                    }
                }
                if (station.GPRS != "" && gprs.Contains(station.GPRS))
                {
                    this.Hide();
                    MessageBox.Show(station.StationID + "站点GPRS不能重复");
                    this.Show();
                    return false;
                }
                else
                {
                    gprs.Add(station.GPRS);
                }

                if (station.Maintran.ToString() == "未知通讯协议")
                {
                    this.Hide();
                    MessageBox.Show(station.StationID + "请配置正确的主信道！");
                    this.Show();
                    return false;
                }
                if (station.Subtran.ToString() == "未知通讯协议")
                {
                    this.Hide();
                    MessageBox.Show(station.StationID + "请配置正确的备信道！");
                    this.Show();
                    return false;
                }
            }

            return true;
        }

        private DataGridViewNumericUpDownColumn GenerateColumnStype()
        {
            DataGridViewNumericUpDownColumn C10_4 = new DataGridViewNumericUpDownColumn()
            {
                Minimum = -9999999,
                Maximum = 9999999,
                DecimalPlaces = 4, /*好像是设置小数点后面的位数*/
                Increment = (Decimal)0.0001 /*增量*/
            };
            return C10_4;
        }

        private DataGridViewNumericUpDownColumn GenerateColumnStype_1()
        {
            DataGridViewNumericUpDownColumn C10_4 = new DataGridViewNumericUpDownColumn()
            {
                Minimum = -9999999,
                Maximum = 9999999,
                DecimalPlaces = 2, /*好像是设置小数点后面的位数*/
                Increment = (Decimal)0.01 /*增量*/
            };
            return C10_4;
        }
        #endregion 帮助方法
    }
}