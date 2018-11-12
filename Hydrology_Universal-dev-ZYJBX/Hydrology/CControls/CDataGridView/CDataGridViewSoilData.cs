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
    /// 墒情数据显示表格控件，包括编辑和非编辑模式
    /// </summary>
    class CDataGridViewSoilData : CExDataGridView
    {
        #region 静态常量
        public static readonly string CS_Delete = "删除";
        public static readonly string CS_StationID = "站号";
        public static readonly string CS_StationName = "站名";
        public static readonly string CS_TimeCollected = "采集时间";
        public static readonly string CS_Voltage = "电压值";
        public static readonly string CS_MsgType = "报文类型";
        public static readonly string CS_ChannelType = "通讯方式";
        public static readonly string CS_TimeFormat = "yyy-MM-dd HH:mm:ss";
        public static readonly string CS_V10 = "10CM电压";
        public static readonly string CS_M10 = "10CM含水量";
        public static readonly string CS_V20 = "20CM电压";
        public static readonly string CS_M20 = "20CM含水量";
        public static readonly string CS_V30 = "30CM电压";
        public static readonly string CS_M30 = "30CM含水量";
        public static readonly string CS_V40 = "40CM电压";
        public static readonly string CS_M40 = "40CM含水量";
        public static readonly string CS_V60 = "60CM电压";
        public static readonly string CS_M60 = "60CM含水量";
        public static readonly string CS_ReciveTime = "接收时间";

        #endregion  ///<STATIC_STRING

        #region 数据成员
        private bool m_bIsEditable; //编辑模式，默认非编辑模式

        // 查询相关信息
        private ISoilDataProxy m_proxySoilData; //墒情历史数据的操作接口
        private string m_strStaionId;           //查询的站点ID
        private DateTime m_dateTimeStart;       //查询的起点日期
        private DateTime m_dateTimeEnd;         //查询的起点日期

        // 导出到Excel表格
        private ToolStripMenuItem m_menuItemExportToExcel;  //导出到Excel表格

        private List<CEntitySoilData> m_listAdded;   //新增的墒情记录
        private List<CEntitySoilData> m_listUpdated; //更新的墒情记录
        private List<long> m_listDeleteSoilData;    //删除的墒情记录
        private List<String> m_listDeleteSoilDatas_StationId;    //删除的墒情记录
        private List<String> m_listDeleteSoilDatas_StationDate;    //删除的墒情记录
        #endregion ///<DATA_MEMBER

        #region 属性
        public bool Editable
        {
            get { return m_bIsEditable; }
            set { SetEditable(value); }
        }
        #endregion ///<PROPERTY

        #region 公共方法
        public CDataGridViewSoilData()
            : base()
        {
            // 设定标题栏,默认有个隐藏列
            this.Header = new string[] 
            { 
                CS_StationID,CS_StationName,CS_TimeCollected, CS_Voltage,
                CS_V10, CS_M10, CS_V20, CS_M20, CS_V30, CS_M30,
                CS_V40, CS_M40, CS_V60, CS_M60,
                CS_MsgType, CS_ChannelType,CS_ReciveTime
            };
            base.HideColomns = new int[] { 8, 9, 12, 13, 15 };
            //11.12
            // this.BPartionPageEnable = false;

            // 设置一页的数量
            this.PageRowCount = CDBParams.GetInstance().UIPageRowCount;

            // 初始化成员变量
            m_bIsEditable = false;
            m_listAdded = new List<CEntitySoilData>();
            m_listUpdated = new List<CEntitySoilData>();
            m_listDeleteSoilData = new List<long>();
            m_listDeleteSoilDatas_StationId = new List<String>();
            m_listDeleteSoilDatas_StationDate = new List<String>();
            //this.Columns[2].Width = 125;
            //this.Columns[2].Width = 125;
        }

        /// <summary>
        /// 初始化数据来源，绑定与数据库的数据
        /// </summary>
        /// <param name="proxy"></param>
        public void InitDataSource(ISoilDataProxy proxy)
        {
            m_proxySoilData = proxy;
        }

        public void SetEditable(bool bEditable)
        {
            m_bIsEditable = bEditable;
            if (m_bIsEditable)
            {
                // 设定标题栏,默认有个隐藏列
                this.Header = new string[] 
                { 
                    CS_Delete,CS_StationID,CS_StationName,CS_TimeCollected, CS_Voltage,
                    CS_V10, CS_M10, CS_V20, CS_M20, CS_V30, CS_M30,
                    CS_V40, CS_M40, CS_V60, CS_M60,
                    CS_ChannelType,CS_MsgType,CS_ReciveTime
                };
                //  this.HideColomns = new int[] { 10 };

                //开启编辑模式,设置可编辑列

                DataGridViewCheckBoxColumn deleteCol = new DataGridViewCheckBoxColumn();
                base.SetColumnEditStyle(0, deleteCol);

                //// 设置采集时间编辑列
                //CalendarColumn collectionCol = new CalendarColumn();
                //base.SetColumnEditStyle(3, collectionCol);

                // 电压编辑列
                DataGridViewNumericUpDownColumn Voltage = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 2 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(4, Voltage);

                // 10CM电压编辑列
                DataGridViewNumericUpDownColumn V10 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 2/*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(5, V10);

                // 10CM含水量编辑列
                DataGridViewNumericUpDownColumn M10 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 3 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(6, M10);

                // 20CM电压编辑列
                DataGridViewNumericUpDownColumn V20 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 2 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(7, V20);

                // 20CM含水量编辑列
                DataGridViewNumericUpDownColumn M20 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 3 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(8, M20);

                // 30CM电压编辑列
                DataGridViewNumericUpDownColumn V30 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 2 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(9, V30);


                // 30CM含水量编辑列
                DataGridViewNumericUpDownColumn M30 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 3 /*好像是设置小数点后面的位数*/

                };
                //M30.Displayed= DataGridViewComboBoxDisplayStyle.Nothing;
                //M30.
                base.SetColumnEditStyle(10, M30);

                // 40CM电压编辑列
                DataGridViewNumericUpDownColumn V40 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 2 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(11, V40);


                // 40CM含水量编辑列
                DataGridViewNumericUpDownColumn M40 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 3 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(12, M40);

                // 40CM电压编辑列
                DataGridViewNumericUpDownColumn V60 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 2 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(13, V60);


                // 40CM含水量编辑列
                DataGridViewNumericUpDownColumn M60 = new DataGridViewNumericUpDownColumn()
                {
                    Minimum = 0,
                    Maximum = 65537,
                    DecimalPlaces = 3 /*好像是设置小数点后面的位数*/

                };
                base.SetColumnEditStyle(14, M60);

                //// 数据状态，可编辑列
                //DataGridViewComboBoxColumn dataStateCol = new DataGridViewComboBoxColumn();
                //dataStateCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                //dataStateCol.Items.AddRange(new object[] { CS_DataState_Normal, CS_DataState_AbNormal });
                //base.SetColumnEditStyle(8, dataStateCol);

                // 接收时间
                //11.11
                //CalendarColumn recvTime = new CalendarColumn();
                //base.SetColumnEditStyle(17, recvTime);


                // 通讯方式，不可编辑

                // 报文类型，不可编辑


                // 设置删除列的宽度
                this.Columns[0].Width = 40; //删除列宽度为20
                this.Columns[3].Width = 125;
                this.Columns[17].Width = 125;
            }
            else
            {
                this.Columns[2].Width = 125;
                this.Columns[16].Width = 125;
            }
        }

        // 设置显示的雨量记录
        public void SetSoilData(List<CEntitySoilData> listSoilData)
        {
            // 清空所有数据,是否一定要这样？好像可以考虑其它方式
            base.m_dataTable.Rows.Clear();
            // 判断状态值
            List<string[]> newRows = new List<string[]>();
            List<EDataState> states = new List<EDataState>();
            if (!m_bIsEditable)
            {
                // 只读模式
                for (int i = 0; i < listSoilData.Count; ++i)
                {
                    EDataState state = EDataState.ENormal; //默认所有数据都是正常的
                    string strStationID = "";
                    string strStationName = "";
                    //  CEntityStation station = CDBDataMgr.Instance.GetStationById(listSoilData[i].StationID);
                    CEntitySoilStation station = CDBSoilDataMgr.Instance.GetSoilStationInfoByStationId(listSoilData[i].StationID);
                    if (null != station)
                    {
                        strStationID = station.StationID;
                        strStationName = station.StationName;
                    }
                    string[] newRow = new string[]
                    {
                        strStationID,
                        strStationName,
                        listSoilData[i].DataTime.ToString(CS_TimeFormat), /*采集时间*/
                        listSoilData[i].DVoltage.ToString(), /*电压值*/

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage10),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture10),

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage20),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture20),

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage30),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture30),

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage40),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture40),

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage60),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture60),

                        CEnumHelper.ChannelTypeToUIStr(listSoilData[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listSoilData[i].MessageType), /*报文类型*/
                        
                        listSoilData[i].reciveTime.ToString()
                        
                    };
                    newRows.Add(newRow);
                    states.Add(state);
                }
            }
            else
            {
                // 编辑模式
                for (int i = 0; i < listSoilData.Count; ++i)
                {
                    EDataState state = EDataState.ENormal; //默认所有数据都是正常的
                    string strStationID = "";
                    string strStationName = "";
                    //  CEntityStation station = CDBDataMgr.Instance.GetStationById(listSoilData[i].StationID);
                    CEntitySoilStation station = CDBSoilDataMgr.Instance.GetSoilStationInfoByStationId(listSoilData[i].StationID);
                    if (null != station)
                    {
                        strStationID = station.StationID;
                        strStationName = station.StationName;
                    }
                    string[] newRow = new string[]
                    {
                        "False", /*未选中*/
                        strStationID,
                        strStationName,
                        listSoilData[i].DataTime.ToString(CS_TimeFormat), /*采集时间*/
                        listSoilData[i].DVoltage.ToString(), /*电压值*/

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage10),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture10),

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage20),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture20),

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage30),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture30),

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage40),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture40),

                        CStringFromatHelper.ToUIString2Number(listSoilData[i].Voltage60),
                        CStringFromatHelper.ToUIString3Number(listSoilData[i].Moisture60),

                        CEnumHelper.ChannelTypeToUIStr(listSoilData[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listSoilData[i].MessageType), /*报文类型*/
                        
                        listSoilData[i].reciveTime.ToString()
                        
                    };
                    newRows.Add(newRow);
                    states.Add(state);
                }
            }
            // 添加到集合的数据表中
            base.AddRowRange(newRows, states);
        }

        // 设置查询条件
        public bool SetFilter(string iStationId, DateTime timeStart, DateTime timeEnd)
        {
            ClearAllState();
            m_strStaionId = iStationId;
            m_dateTimeStart = timeStart;
            m_dateTimeEnd = timeEnd;
            List<CEntitySoilData> listData = m_proxySoilData.QueryByStationAndTime(iStationId, timeStart, timeEnd);
            this.SetSoilData(listData);
            m_iTotalRowCount = listData.Count;
            m_iTotalPage = 1; // 没有换页
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
            if (m_listAdded.Count > 0 || base.m_listMaskedDeletedRows.Count > 0 || base.m_listEditedRows.Count > 0)
            {
                return true;
            }
            return false;
        }

        // 保存当前的所有修改
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
            if (m_listAdded.Count > 0 || m_listUpdated.Count > 0 || m_listDeleteSoilDatas_StationId.Count > 0)
            {
                bool result = true;
                // 新增
                if (m_listAdded.Count > 0)
                {
                    m_proxySoilData.AddSoilDataRange(m_listAdded);
                    m_listAdded.Clear();
                }
                // 修改
                if (m_listUpdated.Count > 0)
                {
                    result = result && m_proxySoilData.UpdateRows(m_listUpdated);
                    m_listUpdated.Clear();
                }
                // 删除
                if (m_listDeleteSoilDatas_StationId.Count > 0)
                {
                    result = result && m_proxySoilData.DeleteRows(m_listDeleteSoilDatas_StationId, m_listDeleteSoilDatas_StationDate);
                    m_listDeleteSoilData.Clear();
                }
                if (!result)
                {
                    // 保存失败
                    return false;
                }
                // 重新刷新界面
                SetSoilData(m_proxySoilData.QueryByStationAndTime(m_strStaionId, m_dateTimeStart, m_dateTimeEnd));

            }
            else
            {

            }

            return true;
        }

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
                CEntitySoilData soildata = new CEntitySoilData();
                //    voltage.VoltageID = long.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_VoltageID].Value.ToString());
                try
                {
                    soildata.StationID = m_strStaionId;
                    soildata.DataTime = DateTime.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_TimeCollected].Value.ToString());
                    soildata.reciveTime = DateTime.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_ReciveTime].Value.ToString());
                    soildata.DVoltage = Decimal.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_Voltage].Value.ToString());
                    soildata.MessageType = CEnumHelper.UIStrToMesssageType(base.Rows[listUpdatedRows[i]].Cells[CS_MsgType].Value.ToString());
                    soildata.ChannelType = CEnumHelper.UIStrToChannelType(base.Rows[listUpdatedRows[i]].Cells[CS_ChannelType].Value.ToString());
                    soildata.Voltage10 = float.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_V10].Value.ToString());
                    soildata.Moisture10 = float.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_M10].Value.ToString());

                    soildata.Voltage20 = float.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_V20].Value.ToString());
                    soildata.Moisture20 = float.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_M20].Value.ToString());

                    soildata.Voltage40 = float.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_V40].Value.ToString());
                    soildata.Moisture40 = float.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_M40].Value.ToString());

                    //  soildata.Voltage60 = float.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_V60].Value.ToString());
                    // soildata.Voltage60 = float.Parse(base.Rows[listUpdatedRows[i]].Cells[CS_V60].Value.ToString());
                }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                {
                    this.Hide();
                    MessageBox.Show("请输入正确的墒情数据！");
                    this.Show();
                }
                // soildata.Moisture10 = Decimal.Parse();
                m_listUpdated.Add(soildata);
            }
            // 获取删除过的数据
            for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
            {
                //     m_listDeleteVoltage.Add(long.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_VoltageID].Value.ToString()));
                m_listDeleteSoilDatas_StationId.Add(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_StationID].Value.ToString());
                m_listDeleteSoilDatas_StationDate.Add(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_TimeCollected].Value.ToString());
            }
            m_listEditedRows.Clear();   //清空此次记录
            m_listMaskedDeletedRows.Clear();    //清空标记为删除的记录
        }

        //11.12
        private void EH_MI_ExportToExcel_Click(object sender, EventArgs e)
        {
            // 弹出对话框，并导出到Excel文件
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                DataTable dataTable = new DataTable();
                //   dataTable.Columns.Add(CS_VoltageID, typeof(Int64));
                dataTable.Columns.Add(CS_StationID);
                dataTable.Columns.Add(CS_StationName, typeof(string));
                dataTable.Columns.Add(CS_TimeCollected, typeof(DateTime));
                dataTable.Columns.Add(CS_Voltage, typeof(string));
                dataTable.Columns.Add(CS_V10, typeof(string));
                dataTable.Columns.Add(CS_M10, typeof(string));
                dataTable.Columns.Add(CS_V20, typeof(string));
                dataTable.Columns.Add(CS_M20, typeof(string));
                dataTable.Columns.Add(CS_V30, typeof(string));
                dataTable.Columns.Add(CS_M30, typeof(string));
                dataTable.Columns.Add(CS_V40, typeof(string));
                dataTable.Columns.Add(CS_M40, typeof(string));
                dataTable.Columns.Add(CS_V60, typeof(string));
                dataTable.Columns.Add(CS_M60, typeof(string));
                dataTable.Columns.Add(CS_ReciveTime, typeof(DateTime));
                dataTable.Columns.Add(CS_MsgType, typeof(string));
                dataTable.Columns.Add(CS_ChannelType, typeof(string));
                // 逐页读取数据
                for (int i = 0; i < m_iTotalPage; ++i)
                {
                    List<CEntitySoilData> tmpListData = m_proxySoilData.QueryByStationAndTime(m_strStaionId, m_dateTimeStart, m_dateTimeEnd);
                    foreach (CEntitySoilData soilData in tmpListData)
                    {
                        // 赋值到dataTable中去
                        DataRow row = dataTable.NewRow();
                        // row[CS_VoltageID] = voltage.VoltageID;
                        row[CS_StationID] = soilData.StationID;
                        row[CS_StationName] = CDBSoilDataMgr.Instance.GetSoilStationInfoByStationId(soilData.StationID).StationName;
                        row[CS_TimeCollected] = soilData.DataTime;
                        row[CS_Voltage] = soilData.DVoltage;
                        row[CS_V10] = soilData.Voltage10;
                        row[CS_M10] = soilData.Voltage10;
                        row[CS_V20] = soilData.Voltage20;
                        row[CS_M20] = soilData.Voltage20;
                        row[CS_V40] = soilData.Voltage40;
                        row[CS_M40] = soilData.Voltage40;
                        row[CS_V60] = soilData.Voltage60;
                        row[CS_M60] = soilData.Voltage60;
                        row[CS_ReciveTime] = soilData.reciveTime;
                        row[CS_MsgType] = CEnumHelper.MessageTypeToUIStr(soilData.MessageType);
                        row[CS_ChannelType] = CEnumHelper.ChannelTypeToUIStr(soilData.ChannelType);
                        dataTable.Rows.Add(row);
                    }
                }
                // 显示提示框
                CMessageBox box = new CMessageBox() { MessageInfo = "正在导出表格，请稍候" };
                box.ShowDialog(this);
                if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, "电压表"))
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

        private void DataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
            }
        }
        protected override void OnSizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(sender, e);
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
        #endregion 公共方法




    }
}
