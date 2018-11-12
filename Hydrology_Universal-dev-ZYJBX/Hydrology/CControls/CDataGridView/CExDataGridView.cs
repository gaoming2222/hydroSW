using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Hydrology.Entity;
using Hydrology.DataMgr;

namespace Hydrology.CControls
{
    // 自定义表格控件
    class CExDataGridView : DataGridView
    {
        #region 事件定义

        /// <summary>
        /// 页码发生改变
        /// </summary>
        public event EventHandler<CEventSingleArgs<int>> PageNumberChanged;

        /// <summary>
        /// 数据已经准备好了事件
        /// </summary>
        public event EventHandler<CEventDBUIDataReadyArgs> DataReady;

        #endregion ///<事件定义

        #region 数据成员

        // 数据内容
        protected DataTable m_dataTable;

        // 数据内容备份
        protected DataTable m_dataTableBak;

        // 数据内容绑定封装
        protected BindingSource m_bindingSource;

        // 添加数据互斥量，保证线程安全
        protected /*static*/ Mutex m_mutexDataTable;

        /// <summary>
        /// 更新界面UI的互斥量
        /// </summary>
        protected Mutex m_mutexRefreshUI;

        // 隐藏列
        protected int[] m_arrayHiddenCols;

        // 表头
        public string[] m_arrayStrHeader;

        // 定时器
        private System.Timers.Timer m_timer = null;

        // 该行数据状态，用来初始化显示颜色
        public enum EDataState { ENormal, EWarning, EError, EDeleted, EPink/*用于实时数据中越界的颜色*/};
        public static readonly string S_C_State = "state";

        // 右键菜单
        protected ContextMenuStrip m_contextMenu;
        protected ToolStripMenuItem m_menuItemFirstPage;
        protected ToolStripMenuItem m_menuItemLastPage;
        protected ToolStripMenuItem m_menuItemPreviousPage;
        protected ToolStripMenuItem m_menuItemNextPage;

        // 分页相关

        protected int m_iCurrentPage = 1;   //当前页码,从1开始
        protected int m_iTotalPage = 0;     //总共页码
        protected int m_iPageRowCount = 100;    //一页的数量,默认是100
        protected int m_iTotalRowCount = 0;     // 总共的行数

        // 上一次刷新数据的时间
        private Nullable<DateTime> m_dateTimePreRefreshTime;

        private static readonly int S_C_TimeInterval = 60 * 1000;    //10s刷新一次,单位为ms

        // 被编辑的行的索引，从0开始
        protected List<int> m_listEditedRows;

        // 被标记为删除的行的索引，从0开始
        protected List<int> m_listMaskedDeletedRows;

        // 可编辑的列的索引
        protected Dictionary<int, DataGridViewColumn> m_mapColumnEdit;

        // 上一次单击的单元格
        protected Point m_preDataGridCellPos;

        #endregion ///<数据成员

        public CExDataGridView()
            : base()
        {
            // 默认启用分页模式
            m_bPartionPageEnable = true;
            // 设置属性
            this.EditMode = DataGridViewEditMode.EditProgrammatically; //程序启用编辑，默认只有双击才能编辑
            this.AutoGenerateColumns = false;
            this.DoubleBuffered = true;  // 设置双缓存，加速界面刷新
            //this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            //this.DefaultCellStyle = 
            //this.VirtualMode = true;
            // 会先调用父类DataGridView的构造函数
            m_dataTable = new DataTable();
            m_dataTableBak = new DataTable();
            m_bindingSource = new BindingSource();
            m_bindingSource.DataSource = m_dataTable;
            //绑定数据源
            this.DataSource = m_bindingSource;

            // 初始化右键菜单
            InitContextMenu();

            // 绑定消息
            // 绑定隐藏状态列的消息
            this.DataBindingComplete += this.ExDataBindingComplete;
            this.CellFormatting += this.ExCellFormatting;           //颜色和序号显示
            //this.CellValueChanged += this.EHValueChanged;           //单元格内容发生改变
            this.CellEndEdit += this.EHValueChanged;
            this.SizeChanged += new EventHandler(OnSizeChanged);
            //this.CellValueNeeded += this.EHCellValueNeeded;

            //初始化互斥量
            m_mutexDataTable = new Mutex();
            m_mutexRefreshUI = new Mutex();

            //初始化定时器
            m_timer = new System.Timers.Timer();
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(EHTimer);
            m_timer.Interval = S_C_TimeInterval;
            m_timer.Enabled = false;

            m_listEditedRows = new List<int>();
            m_mapColumnEdit = new Dictionary<int, DataGridViewColumn>();
            m_preDataGridCellPos = new Point(0, 0);
            m_listMaskedDeletedRows = new List<int>();

            // 开启水平滚动轴
            //this.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            //this.HorizontalScrollBar.Visible = true;
            //this.HorizontalScrollBar.VisibleChanged += new EventHandler(EHHorizontalScrollBarVisible);
            //this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }


        protected virtual void OnSizeChanged(object sender, EventArgs e)
        {

        }

        #region 属性
        // 隐藏列的索引，可以隐藏多个列
        public int[] HideColomns
        {
            get
            {
                return m_arrayHiddenCols;
            }
            set
            {
                m_arrayHiddenCols = value;
                int iMax = this.Header.Length - 1;
                for (int i = 0; i < this.m_arrayHiddenCols.Length; ++i)
                {
                    if (m_arrayHiddenCols[i] <= iMax)
                    {
                        //如果在合理范围之内,隐藏某些列
                        this.Columns[m_arrayHiddenCols[i]].Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// 设置标题信息,表头
        /// </summary>
        public string[] Header
        {
            get { return m_arrayStrHeader; }
            set
            {
                //  清空原来的头
                // m_dataTable.Columns.Clear();
                m_dataTable = new DataTable();
                m_dataTableBak = new DataTable();
                this.Columns.Clear();
                for (int i = 0; i < value.Length; ++i)
                {
                    m_dataTable.Columns.Add(value[i]);
                    m_dataTableBak.Columns.Add(value[i]);
                    this.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell())
                    {
                        HeaderText = value[i],
                        DataPropertyName = value[i],
                        Name = value[i]
                    });
                }
                // 加入状态列
                m_dataTable.Columns.Add(CExDataGridView.S_C_State); //加入状态列
                m_dataTableBak.Columns.Add(CExDataGridView.S_C_State);
                this.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell())
                {
                    HeaderText = CExDataGridView.S_C_State,
                    DataPropertyName = CExDataGridView.S_C_State,
                    Name = CExDataGridView.S_C_State
                });
                m_bindingSource.DataSource = m_dataTable;
                this.DataSource = m_bindingSource;
                m_arrayStrHeader = value;
            }
        }

        public int PageRowCount
        {
            get { return m_iPageRowCount; }
            set
            {
                m_iPageRowCount = value;
            }
        }

        // 如果没有设置总页数，那么按照现行最大的页数进行自动计算
        public int TotalPageCount
        {
            get { return m_iTotalPage; }
            set { m_iTotalPage = value; }
        }

        // 总行数
        public int TotalRowCount
        {
            get { return m_iTotalRowCount; }
            set { m_iTotalRowCount = value; }
        }

        // 是否启用分页功能，默认启用
        private bool m_bPartionPageEnable;
        public bool BPartionPageEnable
        {
            get { return m_bPartionPageEnable; }
            set { m_bPartionPageEnable = value; }
        }

        /// <summary>
        /// 获取当前页码
        /// </summary>
        public int CurrentPageIndex
        {
            get { return m_iCurrentPage; }
        }
        #endregion 属性

        /// <summary>
        /// 添加新的一行数据，保证和头部一致，否则失败 ,bWriteToBak是否需要写入备份表，默认是需要写入的
        /// </summary>
        /// <param name="newRow"></param>
        /// <param name="state"></param>
        /// <param name="bWriteToBak"></param>
        /// <returns></returns>
        public bool AddRow(string[] newRow, EDataState state = EDataState.ENormal, bool bWriteToBak = true)
        {
            try
            {
                if (newRow.Length != Header.Length)
                {
                    return false;
                }
                m_mutexDataTable.WaitOne();
                if (this.IsHandleCreated)
                {
                    // 跨线程调用
                    this.Invoke(new Action(() =>
                    {
                        this.Enabled = false; //防止在更新的时候点击界面崩溃
                        string[] tmpRow = new string[newRow.Length + 1];
                        newRow.CopyTo(tmpRow, 0);
                        tmpRow[newRow.Length] = ((Int32)state).ToString(); //保存状态位
                        m_bindingSource.SuspendBinding();
                        m_bindingSource.RaiseListChangedEvents = false;
                        //m_dataTable.EndLoadData();
                        m_dataTable.Rows.Add(tmpRow);
                        if (bWriteToBak)
                        {
                            m_dataTableBak.Rows.Add(tmpRow);
                        }
                        //m_dataTable.BeginLoadData();
                        m_bindingSource.RaiseListChangedEvents = true;
                        m_bindingSource.ResumeBinding();

                        //this.Rows[m_dataTable.Rows.Count - 1].Selected = true;
                        //this.FirstDisplayedScrollingRowIndex = m_dataTable.Rows.Count - 1;

                        this.Enabled = true;
                    }));
                }
                else
                {
                    // 窗体还没有创建，直接更新
                    string[] tmpRow = new string[newRow.Length + 1];
                    newRow.CopyTo(tmpRow, 0);
                    tmpRow[newRow.Length] = ((Int32)state).ToString(); //保存状态位
                    m_dataTable.Rows.Add(tmpRow);

                    //this.Rows[m_dataTable.Rows.Count - 1].Selected = true;
                    //this.FirstDisplayedScrollingRowIndex = m_dataTable.Rows.Count - 1;
                }
                m_mutexDataTable.ReleaseMutex();
                TryToUpdateData();

            }
            catch (System.InvalidOperationException e)
            {
                Debug.WriteLine("###Catch Exception:{0}", e.ToString());
            }
            catch (System.ArgumentException e)
            {
                Debug.WriteLine("###Catch Exception:{0}", e.ToString());
            }
            catch (System.InvalidCastException e)
            {
                Debug.WriteLine("###Catch Exception:{0}", e.ToString());
            }
            catch (System.Data.ConstraintException e)
            {
                Debug.WriteLine("###Catch Exception:{0}", e.ToString());
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("###Catch Exception:{0}", ex.ToString());
            }
            finally
            {
            }
            return true;
        }

        public void TimeSelect()
        {
            try
            {
                DataView dv = new DataView();
                dv = m_dataTable.DefaultView;
                dv.RowFilter = "TimeCollect Like '%:00:00'";
                m_bindingSource.DataSource = dv;
            }
            catch (Exception e)
            {
                Debug.WriteLine("" + e.ToString());
            }
        }


        /// <summary>
        ///  添加多行数据，保证和头部一致，否则失败
        /// </summary>
        /// <param name="newRows">新行的集合</param>
        /// <param name="states">每一行数据对应的状态值</param>
        /// <returns>添加数据是否成功</returns>
        public bool AddRowRange(List<string[]> newRows, List<EDataState> states, bool bWriteToBak = true)
        {
            //判断是否为空，并且行数和状态对应
            if (newRows.Count == 0 || newRows.Count != states.Count)
            {
                return false;
            }
            m_mutexDataTable.WaitOne();
            if (this.IsHandleCreated)
            {
                // 窗体已经创建，考虑多线程
                this.Invoke(new Action(() =>
                {
                    Enabled = false;
                    m_bindingSource.SuspendBinding();
                    m_bindingSource.RaiseListChangedEvents = false;
                    for (int i = 0; i < newRows.Count; ++i)
                    {
                        string[] newRow = newRows[i];
                        if (newRow.Length != Header.Length)
                        {
                            Debug.WriteLine("AddRowRange 长度不一致");
                            break;
                        }
                        string[] tmpRow = new string[newRow.Length + 1];
                        newRow.CopyTo(tmpRow, 0);
                        tmpRow[newRow.Length] = ((Int32)states[i]).ToString(); //保存状态位

                        m_dataTable.Rows.Add(tmpRow);
                        if (bWriteToBak)
                        {
                            m_dataTableBak.Rows.Add(tmpRow);
                        }
                    }
                    m_bindingSource.RaiseListChangedEvents = true;
                    m_bindingSource.ResumeBinding();
                    Enabled = true;
                }));
            }
            else
            {
                // 窗体没有创建,直接更新
                for (int i = 0; i < newRows.Count; ++i)
                {
                    string[] newRow = newRows[i];
                    if (newRow.Length != Header.Length)
                    {
                        Debug.WriteLine("AddRowRange 长度不一致");
                        break;
                    }
                    string[] tmpRow = new string[newRow.Length + 1];
                    newRow.CopyTo(tmpRow, 0);
                    tmpRow[newRow.Length] = ((Int32)states[i]).ToString(); //保存状态位
                    m_dataTable.Rows.Add(tmpRow);
                    if (bWriteToBak)
                    {
                        m_dataTableBak.Rows.Add(tmpRow);
                    }
                }
            }
            m_mutexDataTable.ReleaseMutex(); //释放锁
            TryToUpdateData();
            //this.Invoke((Action)delegate { Enabled = true; });
            return true;
        }

        // 设置列的编辑样式，只有设置了编辑样式的列才可以编辑,index从0开始
        public bool SetColumnEditStyle(int columnIndex, DataGridViewColumn column)
        {
            if (columnIndex >= 0 && columnIndex < m_arrayStrHeader.Length)
            {
                column.HeaderText = m_arrayStrHeader[columnIndex];
                column.DataPropertyName = m_arrayStrHeader[columnIndex];
                column.Name = m_arrayStrHeader[columnIndex];
                this.Columns.RemoveAt(columnIndex);
                this.Columns.Insert(columnIndex, column);
                m_mapColumnEdit.Add(columnIndex, column);
                return true;
            }
            return false;
        }

        // 立即将最新的数据显示到界面
        public void UpdateDataToUI()
        {
            //this.BeginInvoke(new Action<bool>(m_bindingSource.ResetBindings), false);
            try
            {
                m_mutexRefreshUI.WaitOne();
                if (this.IsHandleCreated)
                {
                    this.Invoke((Action)delegate
                    {
                        CurrencyManager cm = (CurrencyManager)this.BindingContext[m_bindingSource];
                        if (cm != null)
                        {
                            cm.Refresh();
                        }
                        m_dateTimePreRefreshTime = DateTime.Now;
                    });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("电压超限，绘图失败！");
            }
            finally
            {
                m_mutexRefreshUI.ReleaseMutex();
            }

        }

        // 将数据保存到数据库或者实体中,包括增加修改和删除的的各种数据
        public virtual bool DoSave()
        {
            return true;
        }

        /// <summary>
        /// 清空所有行的数据
        /// </summary>
        public void ClearAllRows(bool bWriteToBak = true)
        {
            try
            {
                m_mutexDataTable.WaitOne();
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(() =>
                    {
                        CurrencyManager cm = (CurrencyManager)this.BindingContext[m_bindingSource];
                        cm.SuspendBinding(); //挂起数据绑定
                        m_dataTable.Rows.Clear();
                        if (bWriteToBak)
                        {
                            m_dataTableBak.Rows.Clear();
                        }
                        TryToUpdateData();
                        // 恢复数据绑定
                        cm.ResumeBinding();
                    }));
                }
                else
                {
                    m_dataTable.Rows.Clear();
                }
                m_mutexDataTable.ReleaseMutex();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


        }

        // 更新某一行数据
        public bool UpdateRowData(int rowIndex, string[] newRow, EDataState state)
        {
            try
            {
                m_mutexDataTable.WaitOne();
                if (rowIndex < 0 || rowIndex >= m_dataTable.Rows.Count /*|| !this.IsHandleCreated*/)
                {
                    m_mutexDataTable.ReleaseMutex();
                    return false;
                }
                if (this.IsHandleCreated)
                {
                    // 窗体已经创建，考虑多线程
                    this.Invoke(new Action(() =>
                    {
                        Enabled = false;
                        string[] tmpRow = new string[newRow.Length + 1];
                        newRow.CopyTo(tmpRow, 0);
                        tmpRow[newRow.Length] = ((Int32)state).ToString(); //保存状态位
                        m_bindingSource.SuspendBinding();
                        m_bindingSource.RaiseListChangedEvents = false;
                        for (int i = 0; i < tmpRow.Length; ++i)
                        {
                            m_dataTable.Rows[rowIndex][i] = tmpRow[i];
                        }
                        m_bindingSource.RaiseListChangedEvents = true;
                        m_bindingSource.ResumeBinding();
                        Enabled = true;
                    }));

                }
                else
                {
                    // 窗体还没有创建
                    string[] tmpRow = new string[newRow.Length + 1];
                    newRow.CopyTo(tmpRow, 0);
                    tmpRow[newRow.Length] = ((Int32)state).ToString(); //保存状态位
                    for (int i = 0; i < tmpRow.Length; ++i)
                    {
                        m_dataTable.Rows[rowIndex][i] = tmpRow[i];
                    }
                }
                //m_bindingSource.ResumeBinding();
                m_mutexDataTable.ReleaseMutex();
                TryToUpdateData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                //this.Invoke((Action)delegate { Enabled = true; });
            }
            return true;
        }
        /// <summary>
        /// 更新系列数据
        /// </summary>
        /// <returns></returns>
        public bool UpdateRowRange(List<int> listRowIndex, List<string[]> listNewRow, List<EDataState> listState)
        {
            try
            {
                // 判断参数个数是否匹配
                if (listRowIndex.Count != listNewRow.Count || listRowIndex.Count != listState.Count)
                {
                    // 参数不匹配
                    return false;
                }
                int rowIndex = 0;
                string[] newRow;
                EDataState state = EDataState.ENormal;
                m_mutexDataTable.WaitOne();
                // 判断索引是否合法
                for (int i = 0; i < listRowIndex.Count; ++i)
                {
                    rowIndex = listRowIndex[i];
                    if (rowIndex < 0 || rowIndex >= m_dataTable.Rows.Count /*|| !this.IsHandleCreated*/)
                    {
                        Debug.WriteLine("UpdateRowRange 索引越界,退出更新");
                        m_mutexDataTable.ReleaseMutex();
                        return false;
                    }
                }
                if (this.IsHandleCreated)
                {
                    // 窗体已经创建，考虑多线程
                    this.Invoke(new Action(() =>
                    {
                        Enabled = false;
                        m_bindingSource.SuspendBinding();
                        for (int i = 0; i < listRowIndex.Count; ++i)
                        {
                            rowIndex = listRowIndex[i];
                            newRow = listNewRow[i];
                            state = listState[i];
                            string[] tmpRow = new string[newRow.Length + 1];
                            newRow.CopyTo(tmpRow, 0);
                            tmpRow[newRow.Length] = ((Int32)state).ToString(); //保存状态位
                            m_bindingSource.RaiseListChangedEvents = false;
                            for (int j = 0; j < tmpRow.Length; ++i)
                            {
                                m_dataTable.Rows[rowIndex][j] = tmpRow[j];
                            }
                        }
                        m_bindingSource.RaiseListChangedEvents = true;
                        m_bindingSource.ResumeBinding();
                        Enabled = true;
                    }));

                }
                else
                {
                    // 窗体还没有创建
                    for (int i = 0; i < listRowIndex.Count; ++i)
                    {
                        rowIndex = listRowIndex[i];
                        newRow = listNewRow[i];
                        state = listState[i];
                        string[] tmpRow = new string[newRow.Length + 1];
                        newRow.CopyTo(tmpRow, 0);
                        tmpRow[newRow.Length] = ((Int32)state).ToString(); //保存状态位
                        m_bindingSource.RaiseListChangedEvents = false;
                        for (int j = 0; j < tmpRow.Length; ++i)
                        {
                            m_dataTable.Rows[rowIndex][j] = tmpRow[j];
                        }
                    }
                }
                //m_bindingSource.ResumeBinding();
                m_mutexDataTable.ReleaseMutex();
                TryToUpdateData();

            }
            catch (Exception ex)
            {
                m_mutexDataTable.ReleaseMutex();
                Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新系列数据的状态值
        /// </summary>
        /// <returns></returns>
        public bool UpdateRowRangeStatus(List<int> listRowIndex, List<EDataState> listState)
        {
            try
            {
                // 判断参数个数是否匹配
                if (listRowIndex.Count != listState.Count)
                {
                    // 参数不匹配
                    return false;
                }
                int rowIndex = 0;
                EDataState state = EDataState.ENormal;
                m_mutexDataTable.WaitOne();
                // 判断索引是否合法
                for (int i = 0; i < listRowIndex.Count; ++i)
                {
                    rowIndex = listRowIndex[i];
                    if (rowIndex < 0 || rowIndex >= m_dataTable.Rows.Count /*|| !this.IsHandleCreated*/)
                    {
                        Debug.WriteLine("UpdateRowStatus 索引越界,退出更新");
                        m_mutexDataTable.ReleaseMutex();
                        return false;
                    }
                }
                if (this.IsHandleCreated)
                {
                    // 窗体已经创建，考虑多线程
                    this.Invoke(new Action(() =>
                    {
                        Enabled = false;
                        m_bindingSource.SuspendBinding();
                        for (int i = 0; i < listRowIndex.Count; ++i)
                        {
                            rowIndex = listRowIndex[i];
                            state = listState[i];
                            m_dataTable.Rows[rowIndex][S_C_State] = ((Int32)state).ToString(); //保存状态位
                        }
                        m_bindingSource.RaiseListChangedEvents = true;
                        m_bindingSource.ResumeBinding();
                        Enabled = true;
                    }));

                }
                else
                {
                    // 窗体还没有创建
                    for (int i = 0; i < listRowIndex.Count; ++i)
                    {
                        rowIndex = listRowIndex[i];
                        state = listState[i];
                        m_dataTable.Rows[rowIndex][S_C_State] = ((Int32)state).ToString(); //保存状态位
                    }
                }
                //m_bindingSource.ResumeBinding();
                m_mutexDataTable.ReleaseMutex();
                TryToUpdateData();

            }
            catch (Exception ex)
            {
                m_mutexDataTable.ReleaseMutex();
                Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        // 标记某一行为被删除的数据
        public bool MarkRowDeletedOrNot(int rowIndex, bool bDeleted)
        {
            if (rowIndex < 0 || rowIndex >= this.RowCount)
            {
                return false;
            }
            m_mutexDataTable.WaitOne();

            this.Invoke(new Action(() => { m_bindingSource.SuspendBinding(); }), null);
            m_bindingSource.RaiseListChangedEvents = false;
            if (bDeleted)
            {
                m_dataTable.Rows[rowIndex][m_arrayStrHeader.Length] = ((Int32)EDataState.EDeleted).ToString();
                m_listMaskedDeletedRows.Add(rowIndex); //添加到删除的集合里面
            }
            else
            {
                // 恢复正常
                m_dataTable.Rows[rowIndex][m_arrayStrHeader.Length] = ((Int32)EDataState.ENormal).ToString();
                m_listMaskedDeletedRows.Remove(rowIndex);
            }
            m_bindingSource.RaiseListChangedEvents = true;
            this.Invoke(new Action(() => { m_bindingSource.ResumeBinding(); }), null);
            //m_bindingSource.ResumeBinding();
            TryToUpdateData();
            m_mutexDataTable.ReleaseMutex();
            return true;
        }

        // 删除某些行的数据
        public bool DeleteRowData(int rowIndex, bool bWriteToBak = false)
        {
            if (rowIndex < 0 || rowIndex >= this.RowCount)
            {
                return false;
            }
            m_mutexDataTable.WaitOne();
            this.Invoke(new Action(() => { m_bindingSource.SuspendBinding(); }), null);
            m_bindingSource.RaiseListChangedEvents = false;
            m_dataTable.Rows.RemoveAt(rowIndex);
            // 更新m_listDeletedRows
            for (int i = 0; i < m_listMaskedDeletedRows.Count; ++i)
            {
                if (m_listMaskedDeletedRows[i] > rowIndex)
                {
                    m_listMaskedDeletedRows[i] = m_listMaskedDeletedRows[i] - 1;
                }
            }
            m_bindingSource.RaiseListChangedEvents = true;
            this.Invoke(new Action(() => { m_bindingSource.ResumeBinding(); }), null);
            //m_bindingSource.ResumeBinding();
            TryToUpdateData();
            m_mutexDataTable.ReleaseMutex();
            return true;
        }

        /// <summary>
        /// 判断数据是否改变了
        /// </summary>
        /// <returns></returns>
        public bool IsDataChanged()
        {
            if (m_dataTableBak.Rows.Count != m_dataTable.Rows.Count)
            {
                // 行数不同，返回失败
                return true;
            }
            // 此时行数相同
            int rowcount = m_dataTableBak.Rows.Count;
            int columncount = m_dataTableBak.Columns.Count;
            for (int rowindex = 0; rowindex < rowcount; ++rowindex)
            {
                for (int colindex = 0; colindex < columncount; ++colindex)
                {
                    if (m_dataTable.Rows[rowindex][colindex].ToString() != m_dataTableBak.Rows[rowindex][colindex].ToString())
                    {
                        // 如果不相等，放回true
                        return true;
                    }
                }
            }
            return false;
        }

        #region 事件委托

        protected virtual void ExCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!m_mutexDataTable.WaitOne(0))
            {
                // 表示当前正在后台更新数据，暂时不更新界面消息
                return;
            }
            //m_mutex.WaitOne();
            if (e.RowIndex < 0)
            {
                return;
            }
            try
            {
                //Debug.WriteLine("cell format...{0}-{1}",e.RowIndex,e.ColumnIndex);
                DataGridViewRow CurrentRow = this.Rows[e.RowIndex];
                CurrentRow.HeaderCell.Value = Convert.ToString(e.RowIndex + 1 + (m_iCurrentPage - 1) * m_iPageRowCount);//显示行号，也可以设置成显示其他信息
                // 此时保证枚举前面没有 手动改变枚举的值
                CExDataGridView.EDataState state =
                    (CExDataGridView.EDataState)Int32.Parse(CurrentRow.Cells[m_arrayStrHeader.Length].Value.ToString());
                switch (state)
                {
                    case EDataState.ENormal:
                        {
                            if (e.RowIndex % 2 == 1)
                            {
                                // 浅绿
                                this.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(248, 255, 255);
                            }
                        }
                        break;
                    case EDataState.EWarning:
                        {
                            // 警告
                            this.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Yellow;
                        }
                        break;
                    case EDataState.EError:
                        {
                            // 错误
                            this.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                            //this.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new System.Drawing.Font("Arial", 10);
                        }
                        break;
                    case EDataState.EDeleted:
                        {
                            // 被标记为删除
                            this.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Gray;
                            //this.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new System.Drawing.Font("Arial", 10);
                        }
                        break;
                    case EDataState.EPink:
                        {
                            // 实时数据中标记为越界显示
                            this.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 128, 255);
                        }
                        break;
                }//switch
                // 释放互斥量，表示后台可以更新数据了
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                m_mutexDataTable.ReleaseMutex();
            }

        }

        protected virtual void ExDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // 计算总页数
            //m_iTotalPage = (int)Math.Ceiling( (this.Rows.Count / (double)m_iPageRowCount) );
            if (m_iTotalPage > 1 && m_iCurrentPage < m_iTotalPage)
            {
                m_menuItemNextPage.Enabled = true;
                m_menuItemLastPage.Enabled = true;
            }
            //Debug.WriteLine("###totalpage:{0}", m_iTotalPage);
            //m_dataGridView.Columns[5].Visible = false;
            if (null != m_arrayHiddenCols)
            {
                int iMax = this.Header.Length - 1;
                for (int i = 0; i < this.m_arrayHiddenCols.Length; ++i)
                {
                    if (m_arrayHiddenCols[i] <= iMax)
                    {
                        //如果在合理范围之内,隐藏某些列
                        this.Columns[m_arrayHiddenCols[i]].Visible = false;
                    }
                }
            }
            // 隐藏最后一列，既状态列
            if (this.m_dataTable.Columns.Count > 0)
            {
                this.Columns[this.m_dataTable.Columns.Count - 1].Visible = false;
            }

            // 发出消息
            if (null != DataReady)
            {
                DataReady(this, new CEventDBUIDataReadyArgs()
                {
                    CurrentPageIndex = m_iCurrentPage,
                    TotalPageCount = m_iTotalPage,
                    TotalRowCount = m_iTotalRowCount
                });
            }

        }

        protected virtual void EHTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            //定时器事件，由于初始化的时候是5秒刷新一次，所以，这里直接刷新界面，并停止定时器
            m_timer.Stop();
            try
            {
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke(new Action<bool>(m_bindingSource.ResetBindings), false);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message.ToString());
            }
            m_dateTimePreRefreshTime = DateTime.Now;
        }

        /// <summary>
        /// 某个Cell的值发生改变，单元格完成编辑事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EHValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 单元格的内容发生变化,去除重复
            if (!m_listEditedRows.Contains(e.RowIndex))
            {
                m_listEditedRows.Add(e.RowIndex);
            }
            // 取消编辑模式
            //             this.Columns.RemoveAt(e.ColumnIndex);
            //             this.Columns.Insert(e.ColumnIndex, new DataGridViewColumn(new DataGridViewTextBoxCell())
            //                     {
            //                         HeaderText = m_arrayStrHeader[e.ColumnIndex],
            //                         DataPropertyName = m_arrayStrHeader[e.ColumnIndex],
            //                         Name = m_arrayStrHeader[e.ColumnIndex]
            //                     });
        }

        //         protected virtual void EHCellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        //         {
        //             e.Value = e.Value;
        //         }

        /// <summary>
        /// 水平滚动轴变化事件，用于始终开启水平滚动条，真不知道有什么用，无聊至极
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EHHorizontalScrollBarVisible(object sender, EventArgs e)
        {
            if (!HorizontalScrollBar.Visible)
            {
                // 如果不可见，则手动改为可见
                //    int height = HorizontalScrollBar.Height;
                //    HorizontalScrollBar.Location =
                //      new Point(1,ClientRectangle.Height - height);
                //    HorizontalScrollBar.Size =
                //      new Size( ClientRectangle.Width - 1 - this.HorizontalScrollBar.Width,height);
                //    HorizontalScrollBar.Show();
                //    Debug.WriteLine(string.Format("pos:x:{0},y:{1}, width:{2} height:{3}", HorizontalScrollBar.Location.X, HorizontalScrollBar.Location.Y,
                //        HorizontalScrollBar.Size.Width, HorizontalScrollBar.Size.Height));
            }
        }

        #endregion EVENTHANDLER

        #region 重写
        protected override void OnMouseClick(MouseEventArgs e)
        {
            try
            {
                base.OnMouseClick(e);
                if (e.Button == MouseButtons.Right)
                {
                    //右键单击，弹出菜单
                    if (m_bPartionPageEnable)
                    {
                        m_contextMenu.Show(this, e.Location.X, e.Location.Y);
                    }
                }
            }
            catch (Exception ex)
            {
                CSystemInfoMgr.Instance.AddInfo("单击出错 " + ex.ToString());
            }
        }

        protected override void OnCellMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            try
            {
                // 双击启用编辑
                base.OnCellMouseDoubleClick(e);
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && m_mapColumnEdit.ContainsKey(e.ColumnIndex))
                {
                    // 设置编辑模式
                    try
                    {
                        m_mutexDataTable.WaitOne();
                        this.BeginEdit(true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        m_mutexDataTable.ReleaseMutex();
                    }
                }
            }
            catch (Exception ex) { CSystemInfoMgr.Instance.AddInfo("双击出错 " + ex.ToString()); }
            //this.Columns[e.ColumnIndex] = (DataGridViewColumn)m_mapColumnEdit[e.ColumnIndex].Clone();

            // 接受不了事件
            //                 NumericUpDown test = new NumericUpDown();
            //                 test.Parent = this;
            //                 Rectangle r = this.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            //                 test.Location = new Point(r.X, r.Y);
            //                 test.Width = r.Width;
            //                 test.Height = r.Height;
            //                 test.Show(); 


        }

        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return;
                }
                base.OnCellClick(e);
                if (m_preDataGridCellPos.X == e.RowIndex
                    && m_preDataGridCellPos.Y == e.ColumnIndex
                    && m_mapColumnEdit.ContainsKey(e.ColumnIndex))
                {
                    // 开启编辑
                    m_mutexDataTable.WaitOne();
                    this.BeginEdit(true);
                    m_mutexDataTable.ReleaseMutex();
                }
                m_preDataGridCellPos.X = e.RowIndex;
                m_preDataGridCellPos.Y = e.ColumnIndex;
            }
            catch (Exception ex) { CSystemInfoMgr.Instance.AddInfo("点击出错 " + ex.ToString()); }
        }

        #endregion 重写

        #region 虚函数
        // 初始化右键菜单
        protected virtual void InitContextMenu()
        {
            m_contextMenu = new ContextMenuStrip();

            /* ToolStripMenuItem*/
            m_menuItemFirstPage = new ToolStripMenuItem() { Text = "首页" };
            /*ToolStripMenuItem*/
            m_menuItemLastPage = new ToolStripMenuItem() { Text = "尾页" };
            /*ToolStripMenuItem*/
            m_menuItemPreviousPage = new ToolStripMenuItem() { Text = "上一页" };
            /*ToolStripMenuItem*/
            m_menuItemNextPage = new ToolStripMenuItem() { Text = "下一页" };
            m_menuItemPreviousPage.Enabled = false;
            m_menuItemNextPage.Enabled = false;
            m_menuItemFirstPage.Enabled = false;
            m_menuItemLastPage.Enabled = false;

            ToolStripSeparator seperator = new ToolStripSeparator();
            m_contextMenu.Items.Add(m_menuItemNextPage);
            m_contextMenu.Items.Add(m_menuItemPreviousPage);
            m_contextMenu.Items.Add(seperator);
            seperator = new ToolStripSeparator();
            m_contextMenu.Items.Add(m_menuItemFirstPage);
            m_contextMenu.Items.Add(m_menuItemLastPage);

            // 绑定消息
            m_menuItemNextPage.Click += OnMenuNextPage;
            m_menuItemPreviousPage.Click += OnMenuPreviousPage;
            m_menuItemFirstPage.Click += new EventHandler(OnMenuFirstPage);
            m_menuItemLastPage.Click += new EventHandler(OnMenuLastPage);
        }

        /// <summary>
        /// 尾页菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMenuLastPage(object sender, EventArgs e)
        {
            m_iCurrentPage = m_iTotalPage;
            m_menuItemLastPage.Enabled = false; //尾页不能用
            m_menuItemFirstPage.Enabled = true; //首页可用
            m_menuItemNextPage.Enabled = false; //下一页不可用
            m_menuItemPreviousPage.Enabled = true;//上一页可用
            this.BeginInvoke(new Action<bool>(m_bindingSource.ResetBindings), false);
            if (PageNumberChanged != null)
            {
                PageNumberChanged(this, new CEventSingleArgs<int>(m_iCurrentPage));
            }
        }

        /// <summary>
        /// 首页菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMenuFirstPage(object sender, EventArgs e)
        {
            m_iCurrentPage = 1;
            m_menuItemLastPage.Enabled = true;//尾页可以用
            m_menuItemFirstPage.Enabled = false; //首页不可用
            m_menuItemNextPage.Enabled = true; //下一页可以用
            m_menuItemPreviousPage.Enabled = false; //上一页不能用
            this.BeginInvoke(new Action<bool>(m_bindingSource.ResetBindings), false);
            if (PageNumberChanged != null)
            {
                PageNumberChanged(this, new CEventSingleArgs<int>(m_iCurrentPage));
            }
        }

        /// <summary>
        /// 下一页的事件响应消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMenuNextPage(object sender, EventArgs e)
        {
            m_iCurrentPage += 1;
            if (m_iCurrentPage >= m_iTotalPage)
            {
                m_menuItemNextPage.Enabled = false;
                m_menuItemLastPage.Enabled = false; //尾页不可用
            }
            if (m_iCurrentPage > 1)
            {
                m_menuItemPreviousPage.Enabled = true;
                m_menuItemFirstPage.Enabled = true; //首页可用
            }
            this.BeginInvoke(new Action<bool>(m_bindingSource.ResetBindings), false);
            if (PageNumberChanged != null)
            {
                PageNumberChanged(this, new CEventSingleArgs<int>(m_iCurrentPage));
            }
        }

        /// <summary>
        /// 上一页的事件响应消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMenuPreviousPage(object sender, EventArgs e)
        {
            m_iCurrentPage -= 1;
            if (m_iCurrentPage <= 1)
            {
                m_menuItemPreviousPage.Enabled = false;
                m_menuItemFirstPage.Enabled = false;
            }
            if (m_iCurrentPage < m_iTotalPage)
            {
                m_menuItemNextPage.Enabled = true;
                m_menuItemLastPage.Enabled = true; //尾页可用
            }
            this.BeginInvoke(new Action<bool>(m_bindingSource.ResetBindings), false);
            if (PageNumberChanged != null)
            {
                PageNumberChanged(this, new CEventSingleArgs<int>(m_iCurrentPage));
            }
        }

        /// <summary>
        /// 清空后面操作的所有信息
        /// </summary>
        protected virtual void ClearAllState()
        {
            m_listEditedRows.Clear();
            m_listMaskedDeletedRows.Clear();
        }

        #endregion ///<虚函数

        #region 帮助方法

        // 尝试更新数据，如果时间间隔过小，就启动定时器
        private void TryToUpdateData()
        {
            try
            {
                if (!m_mutexRefreshUI.WaitOne(0))
                {
                    // 如果没有拿到互斥量，直接返回
                    return;
                }
                // 难道互斥量，更新
                if (!m_dateTimePreRefreshTime.HasValue)
                {
                    // 第一次，刷新
                    if (this.IsHandleCreated)
                    {
                        this.Invoke((Action)delegate
                        {
                            CurrencyManager cm = (CurrencyManager)this.BindingContext[m_bindingSource];
                            if (cm != null)
                            {
                                cm.Refresh();
                            }
                            //this.BeginInvoke(new Action<bool>(m_bindingSource.ResetBindings), false);
                            m_dateTimePreRefreshTime = DateTime.Now;

                        });
                    }
                }
                else
                {
                    TimeSpan span = new TimeSpan();
                    span = DateTime.Now - m_dateTimePreRefreshTime.Value;
                    if (span.TotalMilliseconds > S_C_TimeInterval)
                    {
                        //判断时间是否大于5秒,否则刷新
                        if (this.IsHandleCreated)
                        {
                            m_dateTimePreRefreshTime = DateTime.Now;
                            this.Invoke((Action)delegate
                            {
                                CurrencyManager cm = (CurrencyManager)this.BindingContext[m_bindingSource];
                                if (cm != null)
                                {
                                    cm.Refresh();
                                }
                                //this.BeginInvoke(new Action<bool>(m_bindingSource.ResetBindings), false);
                            });
                        }
                    }
                    else
                    {
                        // 开启一个定时器，如果5秒钟之内没有收到数据，立即更新
                        m_timer.Start();
                    }
                }//end of not the first time
                m_mutexRefreshUI.ReleaseMutex();
            }
            catch (System.Exception)
            {

            }

        }

        // 定位某一行为最先可见
        protected virtual void FocusOnRow(int rowindex, bool bSelected = true)
        {
            if (rowindex < this.Rows.Count)
            {
                if (bSelected)
                {
                    // 如果设置为选中的话
                    this.Rows[rowindex].Selected = true;
                }
                this.FirstDisplayedScrollingRowIndex = rowindex;
            }
        }
        #endregion ///<帮助方法
    }
}
