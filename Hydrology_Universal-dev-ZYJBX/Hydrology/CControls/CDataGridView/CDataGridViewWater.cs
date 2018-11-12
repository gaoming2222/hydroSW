using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.DBManager;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using System.Data;
using Hydrology.Forms;
using Hydrology.Utils;
using Hydrology.DataMgr;

namespace Hydrology.CControls
{
    /// <summary>
    /// 水量显示表格控件，包括编辑和非编辑模式
    /// </summary>
    class CDataGridViewWater : CExDataGridView
    {
        #region 静态常量
        public static readonly string CS_Delete = "删除";
        public static readonly string CS_StationID = "站号";
        public static readonly string CS_StationName = "站名";
        public static readonly string CS_TimeCollected = "采集时间";
        public static readonly string CS_WaterStage = "水位";
        public static readonly string CS_WaterFlow = "相应流量";

        public static readonly string CS_TimeReceived = "接收时间";
        public static readonly string CS_MsgType = "报文类型";
        public static readonly string CS_ChannelType = "通讯方式";
        public static readonly string CS_TimeFormat = "yyy-MM-dd HH:mm:ss";
      //  public static readonly string CS_WaterID = "水量ID"; //水量唯一ID
        public static readonly string CS_DataState = "状态"; //雨量唯一ID
        public static readonly string CS_DataState_Normal = "正常"; //雨量唯一ID
        public static readonly string CS_DataState_AbNormal = "不正常"; //雨量唯一ID
        #endregion  ///<STATIC_STRING

        #region 数据成员
        private bool m_bIsEditable; //编辑模式，默认非编辑模式
        private List<CEntityWater> m_listUpdated; //更新的水量记录
        private List<long> m_listDeleteWaters;    //删除的水量记
        private List<String> m_listDeleteWaters_StationId;    //删除的水量记录
        private List<String> m_listDeleteWaters_StationDate;    //删除的水量记录
        private List<CEntityWater> m_listAddedWater;   //新增的水量记录

        // 查询相关信息
        private IWaterProxy m_proxyWater;   //水量表的操作接口
        private string m_strStaionId;            //查询的测站ID
        private DateTime m_dateTimeStart;   //查询的起点日期
        private DateTime m_dateTimeEnd;     //查询的起点日期

        // 导出到Excel表格
        private ToolStripMenuItem m_menuItemExportToExcel;  //导出到Excel表格
        #endregion ///<DATA_MEMBER

        #region 属性
        public bool Editable
        {
            get { return m_bIsEditable; }
            set { SetEditable(value); }
        }
        #endregion ///<PROPERTY

        #region 公共方法
        public CDataGridViewWater()
            : base()
        {
            // 设定标题栏,默认有个隐藏列,默认非编辑模式
            this.Header = new string[] 
            { 
                CS_StationID,CS_StationName,CS_TimeCollected, CS_WaterStage, CS_WaterFlow ,CS_DataState ,CS_TimeReceived, CS_ChannelType, CS_MsgType 
            };

            // 设置一页的数量
            this.PageRowCount = CDBParams.GetInstance().UIPageRowCount;

            // 初始化成员变量
            m_bIsEditable = false; // 默认是非编辑模式
            m_listUpdated = new List<CEntityWater>();
            m_listDeleteWaters = new List<long>();
            m_listDeleteWaters_StationId = new List<String>();
            m_listDeleteWaters_StationDate = new List<String>();
            m_listAddedWater = new List<CEntityWater>();

        }

        /// <summary>
        /// 初始化数据来源，绑定与数据库的数据
        /// </summary>
        /// <param name="proxy"></param>
        public void InitDataSource(IWaterProxy proxy)
        {
            m_proxyWater = proxy;
        }

        // 设置显示的雨量记录
        public void SetWaters(List<CEntityWater> listWater)
        {
            // 清空所有数据,是否一定要这样？好像可以考虑其它方式
            base.m_dataTable.Rows.Clear();
            // 判断状态值
            List<string[]> newRows = new List<string[]>();
            List<EDataState> states = new List<EDataState>();
            if (!m_bIsEditable)
            {
                string[] newRow;
                // 只读模式
                for (int i = 0; i < listWater.Count; ++i)
                {
                    EDataState state = EDataState.ENormal; //默认所有数据都是正常的
                    string strStationName = "";
                    string strStationId = "";
                    string state_1 = CS_DataState_Normal;
                    CEntityStation station = CDBDataMgr.Instance.GetStationById(listWater[i].StationID);
                    if (null != station)
                    {

                        strStationName = station.StationName;
                        strStationId = station.StationID;
                    }
                    if (listWater[i].state == 0)
                    {
                        // 不正常
                        state = CExDataGridView.EDataState.EError;//红色显示 
                        state_1 = CS_DataState_AbNormal;
                    }              
                   //水位可以小于0,流量不行
                    //if (listWater[i].WaterFlow < 0 && listWater[i].WaterStage !=-9999)
                    //{
                    //    newRow = new string[]
                    //        {
                    //            strStationId,
                    //            strStationName,/*站名*/
                    //            listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                    //            listWater[i].WaterStage.ToString(), /*水位*/
                    //             "--",
                    //            listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                    //            CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                    //            CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType) /*报文类型*/
                    //        };
                    //}
                    if (listWater[i].WaterStage == -9999 && listWater[i].WaterFlow > 0)
                    {
                        newRow = new string[]
                    {
                        strStationId,
                        strStationName,/*站名*/
                        listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        "--", /*水位*/
                        listWater[i].WaterFlow.HasValue?listWater[i].WaterFlow.Value.ToString():"", /*流量*/
                        state_1,
                        listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType) /*报文类型*/
                    };
                    }
                    else if (listWater[i].WaterStage !=-9999 && listWater[i].WaterFlow < 0)
                    {
                        newRow = new string[]
                    {
                        strStationId,
                        strStationName,/*站名*/
                        listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        listWater[i].WaterStage.ToString(), /*水位*/
                         "--",
                        state_1,
                        listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType) /*报文类型*/
                    };
                    }
                    else if (listWater[i].WaterStage ==-9999 && listWater[i].WaterFlow < 0)
                    {
                        newRow = new string[]
                    {
                        strStationId,
                        strStationName,/*站名*/
                        listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        "--",
                        "--",
                        state_1,
                        listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType) /*报文类型*/
                    };
                    }
                    else
                    {
                        newRow = new string[]
                    {
                        m_strStaionId,
                        strStationName,/*站名*/
                        listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        listWater[i].WaterStage.ToString(), /*水位*/
                        listWater[i].WaterFlow.ToString(), /*流量*/
                        state_1,
                        listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType), /*报文类型*/
                //        listWater[i].WaterID.ToString() /*水量ID*/
                    };
                    }
                    newRows.Add(newRow);
                    states.Add(state);
                }
                // 添加到集合的数据表中
                base.AddRowRange(newRows, states);
            }
            else
            {
                string[] newRow;
                // 编辑模式，需要将更新的数据和删除的数据，与当前数据进行合并
                for (int i= 0; i < listWater.Count; ++i)
                {
                    EDataState state = EDataState.ENormal; //默认所有数据都是正常的
                    string strStationName = "";
                    string state_1 = CS_DataState_Normal;
                    CEntityStation station = CDBDataMgr.Instance.GetStationById(listWater[i].StationID);
                    if (null != station)
                    {
                        strStationName = station.StationName;
                    }
                    if (listWater[i].state == 0)
                    {
                        // 不正常
                        state = CExDataGridView.EDataState.EError;//红色显示 
                        state_1 = CS_DataState_AbNormal;
                    }   
                //    string[] newRow = new string[]
                //    {
                //        "False", /*未选中*/
                //        m_strStaionId,
                //        strStationName,/*站名*/
                //        listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                //        listWater[i].WaterStage.ToString(), /*水位*/
                //        listWater[i].WaterFlow.ToString(), /*流量*/
                //        listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                //        CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                //        CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType), /*报文类型*/
                ////        listWater[i].WaterID.ToString() /*水量ID*/
                //    };
                  
                    if (listWater[i].WaterStage == -9999 && listWater[i].WaterFlow > 0)
                    {
                        newRow = new string[]
                    {
                        "False", /*未选中*/
                        m_strStaionId,
                        strStationName,/*站名*/
                        listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        "", /*水位*/
                        listWater[i].WaterFlow.HasValue?listWater[i].WaterFlow.Value.ToString():"", /*流量*/
                        state_1,
                        listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType) /*报文类型*/
                    };
                    }
                    else if (listWater[i].WaterStage != -9999 && listWater[i].WaterFlow < 0)
                    {
                        newRow = new string[]
                    {
                       "False", /*未选中*/
                        m_strStaionId,
                        strStationName,/*站名*/
                        listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        listWater[i].WaterStage.ToString(), /*水位*/
                         "",
                         state_1,
                        listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType) /*报文类型*/
                    };
                    }
                    else if (listWater[i].WaterStage == -9999 && listWater[i].WaterFlow < 0)
                    {
                        newRow = new string[]
                    {
                       "False", /*未选中*/
                        m_strStaionId,
                        strStationName,/*站名*/
                        listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        "",
                        "",
                        state_1,
                        listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType) /*报文类型*/
                    };
                    }
                    else
                    {
                        newRow = new string[]
                    {
                         "False", /*未选中*/
                        m_strStaionId,
                        strStationName,/*站名*/
                        listWater[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        listWater[i].WaterStage.ToString(), /*水位*/
                        listWater[i].WaterFlow.ToString(), /*流量*/
                        state_1,
                        listWater[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listWater[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listWater[i].MessageType), /*报文类型*/
                //        listWater[i].WaterID.ToString() /*水量ID*/
                    };
                    }
                    newRows.Add(newRow);
                    states.Add(state);
                   
                }
                // 添加到集合的数据表中
                base.AddRowRange(newRows, states);
            }

            
           
        }

        // 设置查询条件
        public bool SetFilter(string strStationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect)
        {
            ClearAllState();
            m_strStaionId = strStationId;
            m_dateTimeStart = timeStart;
            m_dateTimeEnd = timeEnd;
            m_proxyWater.SetFilter(strStationId, timeStart, timeEnd, TimeSelect);
            if (-1 == m_proxyWater.GetPageCount())
            {
                // 查询失败
                MessageBox.Show("数据库忙，查询失败，请稍后再试！");
                return false;
            }
            else
            {
                // 并查询数据，显示第一页
                this.OnMenuFirstPage(this, null);
                base.TotalPageCount = m_proxyWater.GetPageCount();
                base.TotalRowCount = m_proxyWater.GetRowCount();
                SetWaters(m_proxyWater.GetPageData(1));
                return true;
            }

        }

        // 添加电压记录
        public void AddWater(CEntityWater entity)
        {
            m_listAddedWater.Add(entity);
        }

        // 保存数据
        public override bool DoSave()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            //base.DoSave();
            // 更新
            GetUpdatedData();
            if (m_listAddedWater.Count > 0 || m_listUpdated.Count > 0 || m_listDeleteWaters_StationId.Count > 0)
            {
                bool result = true;
                // 增加
                if (m_listAddedWater.Count > 0)
                {
                    //直接添加，不需要等待1分钟
                    m_proxyWater.AddNewRows_1(m_listAddedWater);
                    m_listAddedWater.Clear();
                }
                // 修改
                if (m_listUpdated.Count > 0)
                {
                    result = result && m_proxyWater.UpdateRows(m_listUpdated);
                    m_listUpdated.Clear();
                }
                // 删除
                if (m_listDeleteWaters_StationId.Count > 0)
                {
                    result = result && m_proxyWater.DeleteRows(m_listDeleteWaters_StationId,m_listDeleteWaters_StationDate);
                    m_listDeleteWaters.Clear();
                }
                if (result)
                {
                    //MessageBox.Show("保存成功，新增记录稍有延迟");
                }
                else
                {
                    // 保存失败
                    //MessageBox.Show("保存失败");
                    return false;
                }
                SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage));
            }
            else
            {
                //MessageBox.Show("没有任何修改，无需保存");
            }
            return true;
        }

        // 判断当前是否有修改尚未保存
        public bool IsModifiedUnSaved()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            if (m_listAddedWater.Count > 0 || base.m_listMaskedDeletedRows.Count > 0 || base.m_listEditedRows.Count > 0)
            {
                return true;
            }
            return false;
        }

        public void SetEditable(bool bEditable)
        {
            m_bIsEditable = bEditable;
            if (m_bIsEditable)
            {
                // 设定标题栏,默认有个隐藏列
                this.Header = new string[] 
                { 
                    CS_Delete,CS_StationID,CS_StationName,CS_TimeCollected, CS_WaterStage, CS_WaterFlow,CS_DataState, CS_TimeReceived, CS_ChannelType, CS_MsgType
                };

               // this.HideColomns = new int[] { 8 };

                //开启编辑模式,设置可编辑列

                DataGridViewCheckBoxColumn deleteCol = new DataGridViewCheckBoxColumn();
                base.SetColumnEditStyle(0, deleteCol);

                //// 设置采集时间编辑列
                //CalendarColumn collectionCol = new CalendarColumn();
                //base.SetColumnEditStyle(2, collectionCol);

                // 水位编辑列
                DataGridViewNumericUpDownColumn waterStage = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 2 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(4, waterStage);

                // 时段雨量编辑列
                DataGridViewNumericUpDownColumn waterFlow = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 3 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(5, waterFlow);

                // 数据状态，可编辑列
                DataGridViewComboBoxColumn dataStateCol = new DataGridViewComboBoxColumn();
                dataStateCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dataStateCol.Items.AddRange(new object[] { CS_DataState_Normal, CS_DataState_AbNormal });
                base.SetColumnEditStyle(6, dataStateCol);

                // 接收时间
                CalendarColumn recvTime = new CalendarColumn();
                base.SetColumnEditStyle(7, recvTime);

                // 通讯方式，不可编辑

                // 报文类型，不可编辑

                // 设置删除列的宽度
                this.Columns[0].Width = 40; //删除列宽度为20
                this.Columns[3].Width = 125;
                this.Columns[7].Width = 125;
            }
            else
            {
                this.Columns[2].Width = 125;
                this.Columns[5].Width = 125;
            }
        }

        #endregion ///< PUBLIC_METHOD

        #region 事件处理
        private void EH_MI_ExportToExcel_Click(object sender, EventArgs e)
        {
            // 弹出对话框，并导出到Excel文件
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                DataTable dataTable = new DataTable();
               // dataTable.Columns.Add(CS_WaterID, typeof(Int64));
                dataTable.Columns.Add(CS_StationID);
                dataTable.Columns.Add(CS_StationName, typeof(string));
                dataTable.Columns.Add(CS_WaterStage, typeof(string));
                dataTable.Columns.Add(CS_WaterFlow, typeof(string)); //可空吗？？
                dataTable.Columns.Add(CS_TimeCollected, typeof(DateTime));
                dataTable.Columns.Add(CS_TimeReceived, typeof(DateTime));
                dataTable.Columns.Add(CS_MsgType, typeof(string));
                dataTable.Columns.Add(CS_ChannelType, typeof(string));
                // 逐页读取数据
                for (int i = 0; i < m_iTotalPage; ++i)
                {
                    List<CEntityWater> tmpWaters = m_proxyWater.GetPageData(i + 1);
                    foreach (CEntityWater water in tmpWaters)
                    {
                        // 赋值到dataTable中去
                        DataRow row = dataTable.NewRow();
                       // row[CS_WaterID] = water.WaterID;
                        row[CS_StationID] = water.StationID;
                        row[CS_StationName] = CDBDataMgr.Instance.GetStationById(water.StationID).StationName;
                        if (water.WaterStage != -9999)
                        {
                            row[CS_WaterStage] = water.WaterStage;
                        }
                        else
                        {
                            row[CS_WaterStage] = "";
                        }
                        if (water.WaterFlow != -9999)
                        {
                            row[CS_WaterFlow] = water.WaterFlow;
                        }
                        else
                        {
                            row[CS_WaterFlow] = "";
                        }
                        row[CS_TimeCollected] = water.TimeCollect;
                        row[CS_TimeReceived] = water.TimeRecieved;
                        row[CS_MsgType] = CEnumHelper.MessageTypeToUIStr(water.MessageType);
                        row[CS_ChannelType] = CEnumHelper.ChannelTypeToUIStr(water.ChannelType);
                        dataTable.Rows.Add(row);
                    }
                }
                // 显示提示框
                CMessageBox box = new CMessageBox() { MessageInfo = "正在导出表格，请稍候" };
                box.ShowDialog(this);
                if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, "水位流量表"))
                {
                    //box.Invoke((Action)delegate { box.Close(); });
                    box.CloseDialog();
                    MessageBox.Show(string.Format("导出成功,保存在文件\"{0}\"中", dlg.FileName));
                }
                else
                {
                    //box.Invoke((Action)delegate { box.Close(); });
                    box.CloseDialog();
                    MessageBox.Show("导出失败");
                }
            }//end of if dialog okay
        }
        #endregion 事件处理

        #region 重载
        // 重写上一页事件
        protected override void OnMenuPreviousPage(object sender, EventArgs e)
        {
            // 获取当前修改的日期
            // GetUpdatedData(); //换页修改数据丢失问题
            if (this.IsModifiedUnSaved())
            {
                DialogResult result = MessageBox.Show("当前页所做修改尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DialogResult.Cancel == result)
                {
                   //取消的话，不换页码
                }
                else if (DialogResult.Yes == result)
                {
                    // 保存当前修改
                    if (!DoSave())
                    {
                        // 如果保存失败，不允许退出
                        MessageBox.Show("保存失败,请检查数据库连接及其配置");
                        return;
                    }
                    MessageBox.Show("保存成功");
                    // 保存成功，换页
                    SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage - 1));
                    base.OnMenuPreviousPage(sender, e);
                }
                else if (DialogResult.No == result)
                {
                    //不保存，直接换页，直接退出
                    //清楚所有状态位
                    base.ClearAllState();
                    SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage - 1));
                    base.OnMenuPreviousPage(sender, e);
                }
            }
            else
            {
                // 没有修改，直接换页
                SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage - 1));
                base.OnMenuPreviousPage(sender, e);
            }
            
        }

        // 重写下一页事件
        protected override void OnMenuNextPage(object sender, EventArgs e)
        {
            // GetUpdatedData(); //换页数据丢失问题
            if (this.IsModifiedUnSaved())
            {
                DialogResult result = MessageBox.Show("当前页所做修改尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DialogResult.Cancel == result)
                {
                    //取消的话，不换页码
                }
                else if (DialogResult.Yes == result)
                {
                    // 保存当前修改
                    if (!DoSave())
                    {
                        // 如果保存失败，不允许退出
                        MessageBox.Show("保存失败,请检查数据库连接及其配置");
                        return;
                    }
                    MessageBox.Show("保存成功");
                    // 保存成功，换页
                    SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage + 1));
                    base.OnMenuNextPage(sender, e);
                }
                else if (DialogResult.No == result)
                {
                    //不保存，直接换页，直接退出
                    //清楚所有状态位
                    base.ClearAllState();
                    SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage + 1));
                    base.OnMenuNextPage(sender, e);
                }
            }
            else
            {
                // 没有修改，直接换页
                SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage + 1));
                base.OnMenuNextPage(sender, e);
            }
        }

        /// <summary>
        /// 重写首页事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMenuFirstPage(object sender, EventArgs e)
        {
            if (this.IsModifiedUnSaved())
            {
                DialogResult result = MessageBox.Show("当前页所做修改尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DialogResult.Cancel == result)
                {
                    //取消的话，不换页码
                }
                else if (DialogResult.Yes == result)
                {
                    // 保存当前修改
                    if (!DoSave())
                    {
                        // 如果保存失败，不允许退出
                        MessageBox.Show("保存失败,请检查数据库连接及其配置");
                        return;
                    }
                    MessageBox.Show("保存成功");
                    // 保存成功，换页
                    base.OnMenuFirstPage(sender, e);
                    SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage));
                    this.UpdateDataToUI();
                }
                else if (DialogResult.No == result)
                {
                    //不保存，直接换页，直接退出
                    //清楚所有状态位
                    base.ClearAllState();
                    base.OnMenuFirstPage(sender, e);
                    SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage));
                    this.UpdateDataToUI();
                }
            }
            else
            {
                // 没有修改，直接换页
                base.OnMenuFirstPage(sender, e);
                SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage));
                this.UpdateDataToUI();
            }
            
        }

        /// <summary>
        /// 重写尾页事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMenuLastPage(object sender, EventArgs e)
        {
            if (this.IsModifiedUnSaved())
            {
                DialogResult result = MessageBox.Show("当前页所做修改尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DialogResult.Cancel == result)
                {
                    //取消的话，不换页码
                }
                else if (DialogResult.Yes == result)
                {
                    // 保存当前修改
                    if (!DoSave())
                    {
                        // 如果保存失败，不允许退出
                        MessageBox.Show("保存失败,请检查数据库连接及其配置");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("保存成功");
                        // 保存成功，换页
                        base.OnMenuFirstPage(sender, e);
                        SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage));
                        this.UpdateDataToUI();
                    }
                }
                else if (DialogResult.No == result)
                {
                    //不保存，直接换页，直接退出
                    //清楚所有状态位
                    base.ClearAllState();
                    base.OnMenuLastPage(sender, e);
                    SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage));
                    this.UpdateDataToUI();
                }
            }
            else
            {
                // 没有修改，直接换页
                base.OnMenuLastPage(sender, e);
                SetWaters(m_proxyWater.GetPageData(base.m_iCurrentPage));
                this.UpdateDataToUI();
            }
        }

        // 重写Cell值改变事件
        protected override void EHValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int iPreValue = this.FirstDisplayedScrollingRowIndex;
                if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
                {
                    if (base.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Equals("True"))
                    {
                        // 删除项
                        base.MarkRowDeletedOrNot(e.RowIndex, true);
                    }
                    else
                    {
                        base.MarkRowDeletedOrNot(e.RowIndex, false);
                    }
                }
                base.EHValueChanged(sender, e);
                base.UpdateDataToUI();
                FocusOnRow(iPreValue, false);
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        // 重写双击事件
        protected override void OnCellMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            try
            {
                int iPreValue = this.FirstDisplayedScrollingRowIndex;
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
                FocusOnRow(iPreValue, false);
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        // 单击事件
        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            try
            {
                int iPreValue = this.FirstDisplayedScrollingRowIndex;
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
                FocusOnRow(iPreValue, false);
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        protected override void InitContextMenu()
        {
            base.InitContextMenu();
            ToolStripSeparator seperator = new ToolStripSeparator();
            m_menuItemExportToExcel = new ToolStripMenuItem("导出Excel...");

            base.m_contextMenu.Items.Add(seperator);
            base.m_contextMenu.Items.Add(m_menuItemExportToExcel);

            // 绑定事件
            m_menuItemExportToExcel.Click += new EventHandler(EH_MI_ExportToExcel_Click);
        }

        protected override void OnSizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(sender, e);
        }

        protected override void ClearAllState()
        {
            base.ClearAllState();
            m_listAddedWater.Clear();
        }
        #endregion ///<OVERWRITE

        #region 帮助方法
        // 生成更新过的数据列表
        private void GetUpdatedData()
        {
            // 如果标记为删除的就不需要再更新了
            List<int> listUpdatedRows = new List<int>();
            for (int i = 0; i < m_listEditedRows.Count; ++i)
            {
                if (!m_listMaskedDeletedRows.Contains(m_listEditedRows[i]))
                {
                    // 如果不在删除列中，则需要更新
                    listUpdatedRows.Add(m_listEditedRows[i]);
                }
            }
            // 获取更新过的数据
            for (int i = 0; i < listUpdatedRows.Count; ++i)
            {
                CEntityWater water = new CEntityWater();
              //  water.WaterID = long.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_WaterID].Value.ToString());
                water.StationID = m_strStaionId;
                water.TimeCollect = DateTime.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_TimeCollected].Value.ToString());
                water.TimeRecieved = DateTime.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_TimeReceived].Value.ToString());
                water.WaterStage = Decimal.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_WaterStage].Value.ToString());
                if (!base.Rows[listUpdatedRows[i]].Cells[CS_WaterFlow].Value.ToString().Equals(""))
                {
                    water.WaterFlow = Decimal.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_WaterFlow].Value.ToString());
                }
                else
                {
                    water.WaterFlow = null;
                }
                // 数据状态
                string tmpDataState = base.Rows[listUpdatedRows[i]].Cells[CS_DataState].Value.ToString();
                if (tmpDataState.Equals(CS_DataState_Normal))
                {
                    water.state = 1;
                }
                else if (tmpDataState.Equals(CS_DataState_AbNormal))
                {
                    water.state= 0;
                }
                water.MessageType = CEnumHelper.UIStrToMesssageType(base.Rows[listUpdatedRows[i]].Cells[CS_MsgType].Value.ToString());
                water.ChannelType = CEnumHelper.UIStrToChannelType(base.Rows[listUpdatedRows[i]].Cells
                    [CS_ChannelType].Value.ToString());
                m_listUpdated.Add(water);
            }
            // 获取删除过的数据
            for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
            {
               // m_listDeleteWaters.Add(long.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_WaterID].Value.ToString()));

                m_listDeleteWaters_StationId.Add(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_StationID].Value.ToString());
                m_listDeleteWaters_StationDate.Add(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_TimeCollected].Value.ToString());
            }
            m_listEditedRows.Clear();   //清空此次记录
            m_listMaskedDeletedRows.Clear(); //清空标记为删除的记录
        }
        #endregion 帮助方法

    }
}
