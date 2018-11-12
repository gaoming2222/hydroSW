using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.DBManager;
using Hydrology.Entity;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Diagnostics;
using Hydrology.Forms;

namespace Hydrology.CControls
{
    /// <summary>
    /// 实时数据的表格控件，包含表头以及内容
    /// </summary>
    class CDataGridViewRTD : CExDataGridView
    {
        #region 静态常量
        public static readonly string CS_StationName = "站名";
        public static readonly string CS_StationId = "站点编号";
        public static readonly string CS_StationType = "站点类型";
        public static readonly string CS_TimeCollected = "采集时间";
        public static readonly string CS_DelayTime = "延迟";
        public static readonly string CS_TodayRain = "今日雨量";
        public static readonly string CS_LastdayRain = "昨日雨量";
        public static readonly string CS_PeriodRain = "时段雨量";
        public static readonly string CS_WaterLevel = "水位";
        public static readonly string CS_WaterFlowActual = "实测流量";
        public static readonly string CS_WaterFlowFindInTable = "相应流量";
        public static readonly string CS_Volatage = "电压";
        public static readonly string CS_Port = "端口";
        public static readonly string CS_MsgType = "报文类型";
        public static readonly string CS_ChannelType = "通讯方式";
        public static readonly string CS_NullUIStr = "---";
        #endregion ///<STATIC_STRING

        #region 数据成员
        private ToolStripMenuItem m_itemWarningFirst;
        private ToolStripMenuItem m_itemNormalFirst;
        private ToolStripMenuItem m_itemRecoverDefault; //恢复默认

        private List<CEntityRealTime> m_listEntityRTD;  //与表格内容一致的RTD内容

        private int time1;
        private int time2;
        #endregion

        public CDataGridViewRTD()
            : base()
        {
            m_listEntityRTD = new List<CEntityRealTime>();
            // 设定标题栏,默认有个隐藏列
            this.Header = new string[] 
            { 
                CS_StationName, CS_StationId, CS_StationType, CS_TimeCollected, CS_DelayTime,
                CS_TodayRain, CS_LastdayRain,CS_PeriodRain, CS_WaterLevel, CS_WaterFlowActual,CS_WaterFlowFindInTable,
                CS_Volatage,CS_Port, CS_MsgType, CS_ChannelType
            };
            // 隐藏延迟列，串口列
            base.HideColomns = new int[] {4,12};
            // 设置一页的数量
            this.PageRowCount = CDBParams.GetInstance().UIPageRowCount;
            string path = string.Empty;
            time1  =  90;
            time2 = 150;
            TimeXmlHelper.ReadFromXML(out time1, out time2);
            //this.PageRowCount = 300;   //  默认一页显示数量

            //DataGridViewNumericUpDownColumn stationId = new DataGridViewNumericUpDownColumn()
            //{
            //    Minimum = 0,
            //    Maximum = 65537,
            //    DecimalPlaces = 0 /*好像是设置小数点后面的位数*/

            //};
            //this.SetColumnEditStyle(1, stationId);

            // 绑定消息，用于按照ID排序
            //this.SortCompare += new DataGridViewSortCompareEventHandler(this.EHSortCompare);
            //this.CellValueNeeded += new DataGridViewCellValueEventHandler(this.EHCellValueNeeded);
            //this.sort

            // 设置数据类型
            //this.Columns[CS_StationId].ValueType = typeof(int);
            // m_dataTable.Columns[CS_StationId].DataType = System.Type.GetType("System.Int32");
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        public void RecalculateHeaderSize()
        {
            this.Columns[3].Width = 180; //删除列宽度为20
        }
        // 添加一条实时记录
        public void AddRTD(CEntityRealTime entity)
        {
            TimeSpan span = entity.TimeReceived - entity.TimeDeviceGained;
            base.AddRow(GetUIShowStringList(entity).ToArray(), GetDataStatus(entity));
            m_listEntityRTD.Add(entity);
        }

        /// <summary>
        /// 更新最新记录，根据stationId的唯一性
        /// </summary>
        /// <param name="entity"></param>
        public bool UpdateRTD(CEntityRealTime entity)
        {
            // 先找到ID所在的行
            m_mutexDataTable.WaitOne();
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                if (m_dataTable.Rows[i][CS_StationId].ToString().Trim() == entity.StrStationID.Trim())
                {
                    // 找到匹配，直接更新并退出
                    // this.Invoke(new Action(() => { m_bindingSource.ResumeBinding(); }), null);
                    //this.DataSource = m_bindingSource;
                     m_mutexDataTable.ReleaseMutex();
                    return base.UpdateRowData(i,
                        GetUIShowStringList(entity).ToArray(),
                        GetDataStatus(entity));
                }

            }//end of for
            //this.DataSource = m_bindingSource;
            for (int i = 0; i < m_listEntityRTD.Count; ++i)
            {
                if (m_listEntityRTD[i].StrStationID.Trim() == entity.StrStationID.Trim())
                {
                    //找到匹配更新
                    m_listEntityRTD[i] = entity;
                    break;
                }
            }
            m_mutexDataTable.ReleaseMutex();
            return false;
        }

        /// <summary>
        /// 刷新实时数据超时的状态值，超过1个半小时，为黄色，超过2个半小时为红色
        /// </summary>
        /// <returns></returns>
        public bool RefreshRTDTimeOutStatus()
        {
            base.m_mutexDataTable.WaitOne();
            List<int> updateRowIndex = new List<int>();
            List<EDataState> updateStatus = new List<EDataState>();
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                DateTime time = DateTime.Parse(m_dataTable.Rows[i][CS_TimeCollected].ToString());
                EDataState state = GetDataStatus(time);
                if (state == EDataState.EError || state == EDataState.EWarning)
                {
                    // 只有显示红色或者黄色才更新，否则不变
                    updateRowIndex.Add(i);
                    updateStatus.Add(state);
                }
            }
            base.m_mutexDataTable.ReleaseMutex();
            return base.UpdateRowRangeStatus(updateRowIndex, updateStatus);
        }

        /// <summary>
        /// 根据表格的内容生成实时数据列表
        /// </summary>
        /// <returns></returns>
        public List<CEntityRealTime> GetRTDList()
        {
            List<CEntityRealTime> result = new List<CEntityRealTime>(m_listEntityRTD.ToArray());
            return result;
        }

        #region OVERRIDE
        // 初始化菜单项
        protected override void InitContextMenu()
        {
            base.InitContextMenu();

            // 添加警告优先，正常优先，以及筛选器选项
            m_itemWarningFirst = new ToolStripMenuItem() { Text = "警告优先" };
            m_itemNormalFirst = new ToolStripMenuItem() { Text = "正常优先" };
            m_itemRecoverDefault = new ToolStripMenuItem() { Text = "恢复默认" };
            // ToolStripMenuItem itemFilter = new ToolStripMenuItem() { Text = "筛选条件..." };

            ToolStripSeparator seperator = new ToolStripSeparator();
            m_contextMenu.Items.Add(seperator);
            m_contextMenu.Items.Add(m_itemWarningFirst);
            m_contextMenu.Items.Add(m_itemNormalFirst);
            m_contextMenu.Items.Add(m_itemRecoverDefault);
            seperator = new ToolStripSeparator();
            //m_contextMenu.Items.Add(seperator);
            //m_contextMenu.Items.Add(itemFilter);

            // 绑定消息
            m_itemWarningFirst.Click += EHWarningFirst;
            m_itemNormalFirst.Click += EHNormalFirst;
            m_itemRecoverDefault.Click += EHRecoverDefault;
        }
        // 重写下一页事件
        protected override void OnMenuNextPage(object sender, EventArgs e)
        {
            //base.OnMenuNextPage(sender, e);
            //MessageBox.Show("next page in RTD");
            //base.m_iCurrentPage += 1;
            //由于内存分页，并不需要将iCurrentPage加1
            base.OnMenuNextPage(sender, e);
            RefreshRowVisiable();
            //this.Update();
        }
        // 重写上一页事件
        protected override void OnMenuPreviousPage(object sender, EventArgs e)
        {
            //base.m_iCurrentPage -= 1;
            base.OnMenuPreviousPage(sender, e);
            RefreshRowVisiable();
        }
        /// <summary>
        /// 重写首页事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMenuFirstPage(object sender, EventArgs e)
        {
            base.OnMenuFirstPage(sender, e);
            RefreshRowVisiable();
        }

        /// <summary>
        /// 重写尾页事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMenuLastPage(object sender, EventArgs e)
        {
            base.OnMenuLastPage(sender, e);
            RefreshRowVisiable();
        }
        // 重写绑定完后的数据响应时间
        protected override void ExDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // 计算总页数
            base.m_iTotalPage = (int)Math.Ceiling((this.Rows.Count / (double)m_iPageRowCount));
            // 刷新显示的页码
            RefreshRowVisiable();
            // 隐藏状态列,并计算总页数
            base.ExDataBindingComplete(sender, e);
        }

        // 重写格式化数据
        protected override void ExCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 绘制颜色
            base.ExCellFormatting(sender, e);
            // 更改行号，也可以设置成显示其他信息
            DataGridViewRow CurrentRow = this.Rows[e.RowIndex];
            CurrentRow.HeaderCell.Value = Convert.ToString(e.RowIndex + 1);
        }

        protected override void OnSizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(sender, e);
            this.Columns[3].Width = 125;
        }
        #endregion

        #region 事件处理
        private void EHNormalFirst(object sender, EventArgs e)
        {
            //正常优先
            m_itemNormalFirst.Checked = true;
            m_itemWarningFirst.Checked = false;
            m_itemRecoverDefault.Checked = false;
            if (this.Columns.Count > 0)
            {
                // 排序
                this.Sort(this.Columns[S_C_State], System.ComponentModel.ListSortDirection.Ascending);
            }
        }
        private void EHWarningFirst(object sender, EventArgs e)
        {
            //警告优先
            m_itemNormalFirst.Checked = false;
            m_itemWarningFirst.Checked = true;
            m_itemRecoverDefault.Checked = false;
            if (this.Columns.Count > 0)
            {
                // 排序
                this.Sort(this.Columns[S_C_State], System.ComponentModel.ListSortDirection.Descending);
            }
        }
        private void EHRecoverDefault(object sender, EventArgs e)
        {
            //恢复默认
            m_itemNormalFirst.Checked = false;
            m_itemWarningFirst.Checked = false;
            m_itemRecoverDefault.Checked = true;
            if (this.Columns.Count > 0)
            {
                // 排序
                this.Sort(this.Columns[CS_StationId], System.ComponentModel.ListSortDirection.Ascending);
                //DataRow[] rows = base.m_dataTable.Select(string.Format("{0} LIKE '*'", CS_StationId), string.Format("Convert({0},'System.Int32') ASC", CS_StationId));
            }
        }
        //private void EHSortCompare(object sender, DataGridViewSortCompareEventArgs e)
        //{
        //     // 用来处理站点排序
        //    if (e.Column.HeaderText == CS_StationId)
        //    {
        //        e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
        //        e.Handled = true;
        //    }
        //}

        //private void EHCellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        //{
        //    if (e.ColumnIndex == this.Columns[CS_StationId].Index)
        //    {
        //        int parsedValue = 0;
        //        if (!int.TryParse(e.Value.ToString(), out parsedValue))
        //        {
        //            if (int.TryParse(this[e.ColumnIndex, e.RowIndex].Value.ToString(), out parsedValue))
        //                e.Value = parsedValue;
        //            else
        //                e.Value = 0;
        //        }
        //    }
        //}
        #endregion  事件处理

        #region 帮助方法
        // 刷新当前行显示的记录
        private void RefreshRowVisiable()
        {
            // 20170402
            this.Hide();
            if (BindingContext == null)
            {
                return;
            }
            CurrencyManager cm = (CurrencyManager)BindingContext[this.DataSource];
            cm.SuspendBinding(); //挂起数据绑定
            // 控制显示行数,内存中分页
            int startRow = (base.m_iCurrentPage - 1) * base.m_iPageRowCount;
            int maxRow = Math.Min(startRow + base.m_iPageRowCount, this.Rows.Count);
            for (int i = 0; i < this.Rows.Count; ++i)
            {
                if (i >= startRow && i < maxRow)
                {
                    // 显示该条记录
                    this.Rows[i].Visible = true;
                }
                else
                {
                    // 隐藏这条记录
                    this.Rows[i].Visible = false;
                }
            }
            // 恢复数据绑定
            cm.ResumeBinding();
            //20170402
            this.Show();
        }

        // 计算当前数值的状态信息
        private CExDataGridView.EDataState GetDataStatus(CEntityRealTime entity)
        {
            // 看是否设定最大值，以及最小值，还有变化值之类的东东啦
//             CExDataGridView.EDataState state = CExDataGridView.EDataState.ENormal;
//             TimeSpan span = entity.TimeReceived - entity.TimeDeviceGained;
//             int offset = Math.Abs(span.Minutes);
//             if (offset > 5 && offset < 10)
//             {
//                 state = CExDataGridView.EDataState.EWarning;
//             }
//             else if (offset > 10)
//             {
//                 state = CExDataGridView.EDataState.EError;
//             }
//             return state;
            CExDataGridView.EDataState state = GetDataStatus(entity.TimeDeviceGained);
            if (state == EDataState.ENormal)
            {
                // 只有没超时的时候，才会显示实时数据的状态
                switch (entity.ERTDState)
                {
                    case ERTDDataState.EError: state = CExDataGridView.EDataState.EPink; break;
                    case ERTDDataState.ENormal: state = CExDataGridView.EDataState.ENormal; break;
                    case ERTDDataState.EWarning: state = CExDataGridView.EDataState.EWarning; break;
                }
            }
            return state;
        }

        /// <summary>
        /// 根据实时数据的时间计算数据的显示状态,一个半小时以内为黄色，2个半小时以上为红色，其余为正常
        /// </summary>
        /// <param name="timeCollect"></param>
        /// <returns></returns>
        private CExDataGridView.EDataState GetDataStatus(DateTime timeCollect)
        {

            // 只有正常的情况下，才计算延迟
            TimeSpan span = DateTime.Now - timeCollect;
            double offset = Math.Abs(span.TotalMinutes);
            //读取XML文件对应的时间
         //   if (offset > 90 && offset < 150)
            if (offset > time1 && offset < time2)
            {
                // 黄色显示
                return CExDataGridView.EDataState.EWarning;
            }
           // else if (offset > 150)
            else if (offset > time2)
            {
                // 红色显示
                return CExDataGridView.EDataState.EError;
            }
            return CExDataGridView.EDataState.ENormal;
        }

        private List<string> GetUIShowStringList(CEntityRealTime entity)
        {
            List<string> result = new List<string>();
            TimeSpan span = entity.TimeReceived - entity.TimeDeviceGained;
            result.Add(entity.StrStationName);
            result.Add(entity.StrStationID.ToString());
            result.Add(CEnumHelper.StationTypeToUIStr(entity.EIStationType));
            result.Add(entity.TimeDeviceGained.ToString());
            result.Add(string.Format("{0}m{1}s", span.Hours * 60 + span.Minutes, span.Seconds));
            if (entity.DDayRainFall.HasValue)
            {
                result.Add(entity.DDayRainFall.Value.ToString("0.00"));/*今日雨量,计算*/
            }
            else
            {
                result.Add(CS_NullUIStr);
            }
            if (entity.LastDayRainFall.HasValue)
            {
                result.Add(entity.LastDayRainFall.Value.ToString("0.00"));/*昨日雨量,计算*/
            }
            else
            {
                result.Add(CS_NullUIStr);
            }
            if (entity.DPeriodRain.HasValue)
            {
                if (entity.DPeriodRain.Value >= 0)
                {
                    result.Add(entity.DPeriodRain.Value.ToString("0.00"));/*时段雨量,计算*/
                }
                else
                {
                    result.Add("0.00");
                }
            }
            else
            {
                result.Add(CS_NullUIStr);
            }
            if (entity.DWaterYield.HasValue)
            {
                if (entity.DDayRainFall != (decimal)-20000)
                {
                    result.Add(entity.DWaterYield.Value.ToString("0.00"));/*水位*/
                }
                else
                {
                    result.Add(CS_NullUIStr);
                }
            }
            else
            {
                result.Add(CS_NullUIStr);
            }
            if (entity.DWaterFlowActual.HasValue)
            {
                result.Add(entity.DWaterFlowActual.Value.ToString("0.00"));/*实测流量*/
            }
            else
            {
                result.Add(CS_NullUIStr);
            }
            if (entity.DWaterFlowFindInTable.HasValue)
            {
                result.Add(entity.DWaterFlowFindInTable.Value.ToString("0.00"));/*相应流量*/
            }
            else
            {
                result.Add(CS_NullUIStr);/*相应流量*/
            }
            

            result.Add(entity.Dvoltage.ToString());/*电压*/
            result.Add(entity.StrPort);
            result.Add(CEnumHelper.MessageTypeToUIStr(entity.EIMessageType));
            result.Add(CEnumHelper.ChannelTypeToUIStr(entity.EIChannelType));
            return result;
        }

        // 重写双击事件
        protected override void OnCellMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {

                    if (base.m_arrayStrHeader[e.ColumnIndex] == CS_StationId || base.m_arrayStrHeader[e.ColumnIndex] == CS_StationName)
                    {

                        //获取当前单元格的行号和列号
                        int rowIndex = e.RowIndex;
                        string stationid = this.Rows[rowIndex].Cells[CS_StationId].Value.ToString();
                        string stationname = this.Rows[rowIndex].Cells[CS_StationName].Value.ToString();
                        CStationDataMgrForm form = new CStationDataMgrForm()
                        {
                            Editable = false
                        }; 
                        if (form != null)
                        {
                            form.cmbStation.Text = string.Format("({0,-4}|{1})", stationid, stationname);
                            form.ShowDialog();
                        }
                    }
                }
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
               
            }
        }


        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            try
            {
                // base.OnCellClick(e);
                //this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
#pragma warning disable CS0168 // 声明了变量“e1”，但从未使用过
            catch(Exception e1)
#pragma warning restore CS0168 // 声明了变量“e1”，但从未使用过
            {

            }
            
        }
        #endregion ///<HELP_METHOD

        internal class TimeXmlHelper
        {
            public static bool WriteToXML(int time1,int time2)
            {
                try
                {
                    if (!Directory.Exists("Config"))
                    {
                        Directory.CreateDirectory("Config");
                    }
                    XElement xElement =
                        new XElement("time",
                            new XElement("time1",time1),
                            new XElement("time2",time2)
                        );
                    XmlWriterSettings settings = new XmlWriterSettings()
                    {
                        Encoding = new UTF8Encoding(false),
                        Indent = true
                    };
                    XmlWriter xw = XmlWriter.Create("Config/time.xml", settings);
                    xElement.Save(xw);
                    xw.Flush();
                    xw.Close();
                    return true;
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    return false;
                }
            }

            public static void ReadFromXML( out int time1,out int time2)
            {
                string filename = "Config/time.xml";
               // path = string.Empty;
                time1 = 90;
                time2 = 150;
                if (File.Exists(filename))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filename);
                    var root = xmlDoc.DocumentElement;

                    XmlNode singleChild = xmlDoc.SelectSingleNode("time/time1");
                    time1  = int.Parse(singleChild.InnerText);

                    singleChild = xmlDoc.SelectSingleNode("time/time2");
                    time2 = int.Parse(singleChild.InnerText);

                    //singleChild = xmlDoc.SelectSingleNode("time/source");
                    //path = singleChild.InnerText.ToString();
                }
                else
                {
                    TimeXmlHelper.WriteToXML(time1,time2);
                    ReadFromXML( out time1,out time2);
                }
            }
        }
    }
}
