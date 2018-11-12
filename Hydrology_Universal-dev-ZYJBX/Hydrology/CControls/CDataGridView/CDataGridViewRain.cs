using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.DBManager;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using System.Data;
using Hydrology.Utils;
using Hydrology.Forms;
using Hydrology.DataMgr;

namespace Hydrology.CControls
{
    /// <summary>
    /// 雨量显示表格控件，包括编辑和非编辑模式
    /// </summary>
    class CDataGridViewRain : CExDataGridView
    {
        #region 静态常量
        public static readonly string CS_Delete = "删除";
        public static readonly string CS_StationID = "站号";
        public static readonly string CS_StationName = "站名";
        public static readonly string CS_DayRain = "日雨量";
        public static readonly string CS_TotalRain = "累计雨量";
        public static readonly string CS_DifferenceRain = "差值雨量";
        public static readonly string CS_PeriodRain = "时段雨量";
        public static readonly string CS_TimeCollected = "采集时间";
        public static readonly string CS_TimeReceived = "接收时间";
        public static readonly string CS_MsgType = "报文类型";
        public static readonly string CS_ChannelType = "通讯方式";
        public static readonly string CS_TimeFormat = "yyy-MM-dd HH:mm:ss";
        // public static readonly string CS_RainID = "雨量ID"; //雨量唯一ID
        public static readonly string CS_DataState = "状态"; //雨量唯一ID
        public static readonly string CS_DataState_Normal = "正常"; //雨量唯一ID
        public static readonly string CS_DataState_AbNormal = "不正常"; //雨量唯一ID
        public static readonly string CS_DataState_Warning = "警告"; //雨量唯一ID
        #endregion  ///<STATIC_STRING

        #region 数据成员
        private bool m_bIsEditable; //编辑模式，默认非编辑模式
        private List<CEntityRain> m_listUpdated; //更新的雨量记录
        private List<long> m_listDeleteRains;    //删除的雨量记录
        private List<String> m_listDeleteRains_StationId;    //删除的雨量记录
        private List<String> m_listDeleteRains_StationDate;    //删除的雨量记录
        private List<CEntityRain> m_listAdded;   //新增的雨量记录

        // 查询相关信息
        private IRainProxy m_proxyRain; //雨量表的操作接口
        private string m_strStaionId;    //查询的车站ID
        private DateTime m_dateTimeStart;   //查询的起点日期
        private DateTime m_dateTimeEnd;   //查询的起点日期

        // 导出到Excel表格
        private ToolStripMenuItem m_menuItemExportToExcel;  //导出到Excel表格
        #endregion ///<DATA_MEMBER

        #region PROPERTY
        public bool Editable
        {
            get { return m_bIsEditable; }
            set { SetEditable(value); }
        }
        #endregion ///<PROPERTY

        #region 公共方法
        public CDataGridViewRain()
            : base()
        {
            // 设定标题栏,默认有个隐藏列
            this.Header = new string[]
            {
                CS_StationID,CS_StationName,CS_TimeCollected, CS_TotalRain,CS_DifferenceRain, CS_PeriodRain, CS_DayRain, CS_DataState,CS_TimeReceived, CS_ChannelType, CS_MsgType
            };

            // 设置一页的数量
            this.PageRowCount = CDBParams.GetInstance().UIPageRowCount;

            // 初始化成员变量
            m_bIsEditable = false;
            m_listUpdated = new List<CEntityRain>();
            m_listDeleteRains = new List<long>();
            m_listDeleteRains_StationId = new List<String>();
            m_listDeleteRains_StationDate = new List<String>();
            m_listAdded = new List<CEntityRain>();

        }

        /// <summary>
        /// 初始化数据来源，绑定与数据库的数据
        /// </summary>
        /// <param name="proxy"></param>
        public void InitDataSource(IRainProxy proxy)
        {
            m_proxyRain = proxy;
        }

        // 设置显示的雨量记录
        public void SetRain(List<CEntityRain> listRain)
        {
            // 清空所有数据,是否一定要这样？好像可以考虑其它方式
            base.m_dataTable.Rows.Clear();
            // 判断状态值
            List<string[]> newRows = new List<string[]>();
            List<EDataState> states = new List<EDataState>();
            if (!m_bIsEditable)
            {
                // 只读模式
                for (int i = 0; i < listRain.Count; ++i)
                {
                    EDataState state = EDataState.ENormal; //默认所有数据都是正常的
                    string strStationName = "";
                    string strStationId = "";
                    string state_1 = CS_DataState_Normal;
                    CEntityStation station = CDBDataMgr.Instance.GetStationById(listRain[i].StationID);
                    if (null != station)
                    {
                        strStationName = station.StationName;
                        strStationId = station.StationID;
                    }

                    if (listRain[i].BState == 0)
                    {
                        // 不正常
                        state = CExDataGridView.EDataState.EError;//红色显示 
                        state_1 = CS_DataState_AbNormal;
                    }
                    else if (listRain[i].BState == 2)
                    {
                        // 不正常
                        state = CExDataGridView.EDataState.EWarning;//黄色显示
                        state_1 = CS_DataState_Warning;
                    }
                    string[] newRow = new string[]
                    {
                        strStationId,
                        strStationName,
                        listRain[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        listRain[i].TotalRain.ToString(), /*累计雨量*/
                        (listRain[i].DifferneceRain<0?0:listRain[i].DifferneceRain).ToString(), /*差值雨量*/
                        (listRain[i].PeriodRain<0?0:listRain[i].PeriodRain).ToString(), /*时段雨量*/
                        (listRain[i].DayRain<0?0:listRain[i].DayRain).ToString(), /*日雨量,貌似只有8点钟才有,注意*/
                        state_1,
                        listRain[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listRain[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listRain[i].MessageType) /*报文类型*/
                        
                    };

                    newRows.Add(newRow);
                    states.Add(state);
                }
            }
            else
            {
                // 编辑模式，需要将更新的数据和删除的数据，与当前数据进行合并，不弄这个
                for (int i = 0; i < listRain.Count; ++i)
                {
                    EDataState state = EDataState.ENormal; //默认所有数据都是正常的
                    string strStationName = "";
                    string strStationId = "";
                    string state_1 = CS_DataState_Normal;
                    CEntityStation station = CDBDataMgr.Instance.GetStationById(listRain[i].StationID);
                    if (null != station)
                    {
                        strStationName = station.StationName;
                        strStationId = station.StationID;
                    }
                    if (listRain[i].BState == 0)
                    {
                        // 不正常
                        state = CExDataGridView.EDataState.EError;//红色显示 
                        state_1 = CS_DataState_AbNormal;
                    }
                    else if (listRain[i].BState == 2)
                    {
                        // 不正常
                        state = CExDataGridView.EDataState.EWarning;//黄色显示
                        state_1 = CS_DataState_Warning;
                    }
                    string[] newRow = new string[]
                    {
                        "False", /*未选中*/
                        strStationId,
                        strStationName, /*站名*/
                        listRain[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        listRain[i].TotalRain.ToString(), /*累计雨量*/
                        (listRain[i].DifferneceRain<0?0:listRain[i].DifferneceRain).ToString(), /*差值雨量*/
                        (listRain[i].PeriodRain<0?0:listRain[i].PeriodRain).ToString(), /*时段雨量*/
                        (listRain[i].DayRain<0?0:listRain[i].DayRain).ToString(), /*日雨量,貌似只有8点钟才有,注意*/
                        state_1,
                        listRain[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listRain[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listRain[i].MessageType), /*报文类型*/
                    //    listRain[i].RainID.ToString() /*雨量ID*/
                    };

                    newRows.Add(newRow);
                    states.Add(state);
                }
            }
            // 添加到集合的数据表中
            base.AddRowRange(newRows, states);
        }

        // 设置查询条件
        public bool SetFilter(string iStationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect)
        {
            ClearAllState();
            m_strStaionId = iStationId;
            m_dateTimeStart = timeStart;
            m_dateTimeEnd = timeEnd;
            m_proxyRain.SetFilter(iStationId, timeStart, timeEnd,TimeSelect);
            if (-1 == m_proxyRain.GetPageCount())
            {
                // 查询失败
                MessageBox.Show("数据库忙，查询失败，请稍后再试！");
                return false;
            }
            else
            {
                // 并查询数据，显示第一页
                this.OnMenuFirstPage(this, null);
                base.TotalPageCount = m_proxyRain.GetPageCount();
                base.TotalRowCount = m_proxyRain.GetRowCount();
                SetRain(m_proxyRain.GetPageData(1));
                return true;
            }
        }

        // 添加雨量记录
        public void AddRain(CEntityRain entity)
        {
            m_listAdded.Add(entity);
        }

        // 保存数据
        public override bool DoSave()
        {
            try
            {
                //int iPreValue = this.FirstDisplayedScrollingRowIndex;
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("当前雨量表数据有修改，是否保存？", "保存", messButton);
                if (dr == DialogResult.OK)
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
                    if (m_listAdded.Count > 0 || m_listUpdated.Count > 0 || m_listDeleteRains_StationId.Count > 0)
                    {
                        // 更新
                        bool result = true;
                        if (m_listAdded.Count > 0)
                        {
                            // 异步添加，需要考虑整点问题
                            //m_proxyRain.AddNewRows(m_listAdded);
                            m_proxyRain.AddNewRows_DataModify(m_listAdded);
                            m_proxyRain.UpdateRows_DataModify(m_listAdded);
                            m_proxyRain.UpdateOtherRows_DataModify(m_listAdded);
                            m_listAdded.Clear();
                            //添加雨量数据,统一更新
                        }
                        if (m_listUpdated.Count > 0)
                        {
                            //result = result && m_proxyRain.UpdateRows(m_listUpdated);
                            result = result && m_proxyRain.UpdateRows_DataModify(m_listUpdated);
                            //修改雨量数据,统一更新
                            m_proxyRain.UpdateOtherRows_DataModify(m_listUpdated);
                            m_listUpdated.Clear();
                        }
                        // 删除
                        if (m_listDeleteRains_StationId.Count > 0)
                        {
                            DialogResult dr_delete = MessageBox.Show("当前雨量表数据被标记为删除，是否删除所选数据？", "删除", messButton);
                            if (dr_delete == DialogResult.OK)
                            {
                                result = result && m_proxyRain.DeleteRows(m_listDeleteRains_StationId, m_listDeleteRains_StationDate);
                                List<CEntityRain> tmpStationList = new List<CEntityRain>();
                                CEntityRain tmpStation = new CEntityRain();
                                for (int j = 0; j < m_listDeleteRains_StationId.Count; j++)
                                {
                                    tmpStation.StationID = m_listDeleteRains_StationId[j];
                                    tmpStation.TimeCollect = DateTime.Parse(m_listDeleteRains_StationDate[j]);
                                    tmpStationList.Add(tmpStation);
                                }
                                m_proxyRain.UpdateOtherRows_DataModify_1(tmpStationList);
                            }
                            //删除雨量数据，统一更新
                            m_listDeleteRains.Clear();
                            m_listDeleteRains_StationDate.Clear();
                            m_listDeleteRains_StationId.Clear();
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
                        SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage));
                    }
                    else
                    {
                        //MessageBox.Show("没有任何修改，无需保存");
                    }
                    //FocusOnRow(iPreValue, false);
                    return true;
                }
                else
                {
                    //FocusOnRow(iPreValue, false);
                    return false;
                }
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { return false; }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        public void SetEditable(bool bEditable)
        {
            m_bIsEditable = bEditable;
            if (m_bIsEditable)
            {
                // 设定标题栏,默认有个隐藏列
                this.Header = new string[]
                {
                    CS_Delete,CS_StationID,CS_StationName,CS_TimeCollected, CS_TotalRain,CS_DifferenceRain, CS_PeriodRain, CS_DayRain, CS_DataState,CS_TimeReceived, CS_ChannelType, CS_MsgType
                };
                //  this.HideColomns = new int[] { 10 };

                //开启编辑模式,设置可编辑列

                DataGridViewCheckBoxColumn deleteCol = new DataGridViewCheckBoxColumn();
                base.SetColumnEditStyle(0, deleteCol);

                //// 设置采集时间编辑列
                //CalendarColumn collectionCol = new CalendarColumn();
                //base.SetColumnEditStyle(3, collectionCol);

                // 累计雨量编辑列
                DataGridViewNumericUpDownColumn totalRain = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 1 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(4, totalRain);

                //// 差值雨量编辑列
                //DataGridViewNumericUpDownColumn differneceRain = new DataGridViewNumericUpDownColumn()
                //{
                //    Minimum = 0,
                //    Maximum = 65537,
                //    DecimalPlaces = 1 /*好像是设置小数点后面的位数*/

                //};
                //base.SetColumnEditStyle(5, differneceRain);

                //// 时段雨量编辑列
                //DataGridViewNumericUpDownColumn periodRain = new DataGridViewNumericUpDownColumn()
                //{
                //    Minimum = 0,
                //    Maximum = 65537,
                //    DecimalPlaces = 1 /*好像是设置小数点后面的位数*/

                //};
                //base.SetColumnEditStyle(6, periodRain);

                //// 日雨量
                //DataGridViewNumericUpDownColumn dayRain = new DataGridViewNumericUpDownColumn()
                //{
                //    Minimum = 0,
                //    Maximum = 65537,
                //    DecimalPlaces = 1 /*好像是设置小数点后面的位数*/

                //};
                //base.SetColumnEditStyle(7, dayRain);

                // 数据状态，可编辑列
                DataGridViewComboBoxColumn dataStateCol = new DataGridViewComboBoxColumn();
                dataStateCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dataStateCol.Items.AddRange(new object[] { CS_DataState_Normal, CS_DataState_AbNormal, CS_DataState_Warning });
                base.SetColumnEditStyle(8, dataStateCol);

                // 接收时间
                CalendarColumn recvTime = new CalendarColumn();
                base.SetColumnEditStyle(9, recvTime);


                // 通讯方式，不可编辑

                // 报文类型，不可编辑


                // 设置删除列的宽度
                this.Columns[0].Width = 40; //删除列宽度为20
                this.Columns[1].Width = 60;
                this.Columns[3].Width = 125;
                this.Columns[4].Width = 70;
                this.Columns[5].Width = 70;
                this.Columns[6].Width = 70;
                this.Columns[7].Width = 70;
                this.Columns[8].Width = 60;
                this.Columns[9].Width = 125; // 接收时间
            }
            else
            {
                this.Columns[0].Width = 60;
                this.Columns[2].Width = 125;
                this.Columns[3].Width = 70;
                this.Columns[4].Width = 70;
                this.Columns[5].Width = 70;
                this.Columns[6].Width = 70;
                this.Columns[7].Width = 60;
                this.Columns[8].Width = 125; // 接收时间
                //this.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                //this.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
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
            if (m_listAdded.Count > 0 || base.m_listMaskedDeletedRows.Count > 0 || base.m_listEditedRows.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion ///< PUBLIC_METHOD

        #region 事件处理
        private void EH_MI_ExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // 弹出对话框，并导出到Excel文件
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    // 保存到Excel表格中
                    DataTable dataTable = new DataTable();
                    //CS_StationID,CS_StationName
                    //  dataTable.Columns.Add(CS_RainID, typeof(Int64));
                    dataTable.Columns.Add(CS_StationID);
                    dataTable.Columns.Add(CS_StationName, typeof(string));
                    dataTable.Columns.Add(CS_TotalRain, typeof(string));
                    dataTable.Columns.Add(CS_DifferenceRain, typeof(string));
                    dataTable.Columns.Add(CS_DayRain, typeof(string));
                    // dataTable.Columns.Add(CS_PeriodRain, typeof(Decimal));
                    dataTable.Columns.Add(CS_PeriodRain, typeof(string));
                    dataTable.Columns.Add(CS_DataState, typeof(string));
                    dataTable.Columns.Add(CS_TimeCollected, typeof(DateTime));
                    dataTable.Columns.Add(CS_TimeReceived, typeof(DateTime));
                    dataTable.Columns.Add(CS_MsgType, typeof(string));
                    dataTable.Columns.Add(CS_ChannelType, typeof(string));
                    // 逐页读取数据
                    for (int i = 0; i < m_iTotalPage; ++i)
                    {
                        List<CEntityRain> tmpRains = m_proxyRain.GetPageData(i + 1);
                        foreach (CEntityRain rain in tmpRains)
                        {
                            // 赋值到dataTable中去
                            DataRow row = dataTable.NewRow();
                            //  row[CS_RainID] = rain.RainID;
                            row[CS_StationID] = rain.StationID;
                            row[CS_StationName] = CDBDataMgr.Instance.GetStationById(rain.StationID).StationName;
                            row[CS_DayRain] = rain.DayRain;
                            row[CS_TotalRain] = rain.TotalRain;
                            row[CS_DayRain] = rain.DayRain;
                            row[CS_PeriodRain] = rain.PeriodRain;
                            row[CS_DataState] = rain.BState == 1 ? "正常" : "不正常";
                            row[CS_TimeCollected] = rain.TimeCollect;
                            row[CS_TimeReceived] = rain.TimeRecieved;
                            row[CS_MsgType] = CEnumHelper.MessageTypeToUIStr(rain.MessageType);
                            row[CS_ChannelType] = CEnumHelper.ChannelTypeToUIStr(rain.ChannelType);
                            dataTable.Rows.Add(row);
                        }
                    }
                    // 显示提示框
                    CMessageBox box = new CMessageBox() { MessageInfo = "正在导出表格，请稍候" };
                    box.ShowDialog(this);
                    if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, "雨量表"))
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
            catch (Exception ex)
            {
                MessageBox.Show("导出失败" + ex.ToString());
            }
        }
        #endregion 事件处理

        #region 重写

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
                    SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage - 1));
                    base.OnMenuPreviousPage(sender, e);
                }
                else if (DialogResult.No == result)
                {
                    //不保存，直接换页，直接退出
                    //清楚所有状态位
                    base.ClearAllState();
                    SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage - 1));
                    base.OnMenuPreviousPage(sender, e);
                }
            }
            else
            {
                // 没有修改，直接换页
                SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage - 1));
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
                    SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage + 1));
                    base.OnMenuNextPage(sender, e);
                }
                else if (DialogResult.No == result)
                {
                    //不保存，直接换页，直接退出
                    //清楚所有状态位
                    base.ClearAllState();
                    SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage + 1));
                    base.OnMenuNextPage(sender, e);
                }
            }
            else
            {
                // 没有修改，直接换页
                SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage + 1));
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
                    SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage));
                    this.UpdateDataToUI();
                }
                else if (DialogResult.No == result)
                {
                    //不保存，直接换页，直接退出
                    //清楚所有状态位
                    base.ClearAllState();
                    base.OnMenuFirstPage(sender, e);
                    SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage));
                    this.UpdateDataToUI();
                }
            }
            else
            {
                // 没有修改，直接换页
                base.OnMenuFirstPage(sender, e);
                SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage));
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
                        base.OnMenuLastPage(sender, e);
                        SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage));
                        this.UpdateDataToUI();
                    }
                }
                else if (DialogResult.No == result)
                {
                    //不保存，直接换页，直接退出
                    //清楚所有状态位
                    base.ClearAllState();
                    base.OnMenuLastPage(sender, e);
                    SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage));
                    this.UpdateDataToUI();
                }
            }
            else
            {
                // 没有修改，直接换页
                base.OnMenuLastPage(sender, e);
                SetRain(m_proxyRain.GetPageData(base.m_iCurrentPage));
                this.UpdateDataToUI();
            }

        }

        // 重写Cell值改变事件
        protected override void EHValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0)
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

        // 重写初始化菜单方法，添加导出菜单项
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
            //base.OnSizeChanged(sender, e);
        }

        protected override void ClearAllState()
        {
            base.ClearAllState();
            m_listAdded.Clear();
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
                CEntityRain rain = new CEntityRain();
                rain.StationID = m_strStaionId;
                rain.TimeCollect = DateTime.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_TimeCollected].Value.ToString());
                rain.TimeRecieved = DateTime.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_TimeReceived].Value.ToString());
                if (!base.Rows[listUpdatedRows[i]].Cells[CS_DayRain].Value.ToString().Equals(""))
                {
                    rain.DayRain = Decimal.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_DayRain].Value.ToString());
                }
                else
                {
                    rain.DayRain = null;
                }
                if (!base.Rows[listUpdatedRows[i]].Cells[CS_PeriodRain].Value.ToString().Equals(""))
                {
                    rain.PeriodRain = Decimal.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_PeriodRain].Value.ToString());
                }
                else
                {
                    rain.PeriodRain = null;
                }
                rain.TotalRain = Decimal.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_TotalRain].Value.ToString());
                rain.MessageType = CEnumHelper.UIStrToMesssageType(base.Rows[listUpdatedRows[i]].Cells[CS_MsgType].Value.ToString());
                rain.ChannelType = CEnumHelper.UIStrToChannelType(base.Rows[listUpdatedRows[i]].Cells[CS_ChannelType].Value.ToString());
                // 数据状态
                string tmpDataState = base.Rows[listUpdatedRows[i]].Cells[CS_DataState].Value.ToString();
                if (tmpDataState.Equals(CS_DataState_Normal))
                {
                    rain.BState = 1;
                }
                else if (tmpDataState.Equals(CS_DataState_AbNormal))
                {
                    rain.BState = 0;
                }
                else if (tmpDataState.Equals(CS_DataState_Warning))
                {
                    rain.BState = 2;
                }
                //   rain.RainID = long.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_RainID].Value.ToString());
                m_listUpdated.Add(rain);
            }
            // 获取删除过的数据
            for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
            {
                //m_listDeleteRains.Add(long.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_RainID].Value.ToString()));

                m_listDeleteRains_StationId.Add(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_StationID].Value.ToString());
                m_listDeleteRains_StationDate.Add(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_TimeCollected].Value.ToString());
            }
            // 清空编辑过的数据
            m_listEditedRows.Clear();
            m_listMaskedDeletedRows.Clear();   //清空此次记录
        }
        #endregion ///<HELP_METHOD

    }
}
