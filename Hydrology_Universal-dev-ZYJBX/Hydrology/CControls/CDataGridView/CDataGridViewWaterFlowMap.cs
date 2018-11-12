using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using System.Windows.Forms;
using System.Diagnostics;
using Hydrology.DBManager.Interface;
using Hydrology.DataMgr;

namespace Hydrology.CControls
{
    /// <summary>
    /// 用户自定义水位流量(库容)关系表控件
    /// </summary>
    class CDataGridViewWaterFlowMap : CExDataGridView
    {
        #region 结构体定义
        public struct SStatus
        {
            public List<int> listEditedRows { get; set; }
            public List<int> listMarkDeleteRows { get; set; }
            public List<long> listDoDeletes { get; set; }
        }
        public struct SModifiedData
        {
            public List<CEntityWaterFlowMap> listUpdated { get; set; }
            public List<CEntityWaterFlowMap> listAdded { get; set; }
            public List<long> listDeleted { get; set; }
        }
        #endregion 结构体定义

        #region 静态常量
        // public static readonly string CS_RecordId = "记录ID"; //隐藏列，不显示
        public static readonly string CS_Delete = "删除";
        public static readonly string CS_StationId = "站号";
        public static readonly string CS_BGTM = "起止时间";
        public static readonly string CS_PTNO = "点序号";
        public static readonly string CS_ZR = "水位";
        public static readonly string CS_Q1 = "流量1(库容)";
        public static readonly string CS_Q2 = "流量2(库容)";
        public static readonly string CS_Q3 = "流量3(库容)";
        public static readonly string CS_Q4 = "流量4(库容)";
        public static readonly string CS_Q5 = "流量5(库容)";
        public static readonly string CS_Q6 = "流量6(库容)";
        public static readonly string CS_currQ = "当前使用";
        public static readonly string CS_TimeFormat = "yyy-MM-dd HH:mm:ss";
        #endregion 静态常量

        #region 数据成员
        private List<CEntityWaterFlowMap> m_listData; // 所有数据，为外部赋值，并且直接有内部修改外部的值，类似于指针
        private string m_strStationId;  //当前站点的StationId;
        private List<long> m_listDoDeletedRows; //标记为删除并且点了删除按钮的列，无法恢复
        private List<CEntityWaterFlowMap> m_listDeleteByUser;     //删除
        private IWaterFlowMapProxy m_proxyWaterFlowMap; //数据库接口对象
        #endregion 数据成员

        #region 公共方法

        public CDataGridViewWaterFlowMap()
            : base()
        {
            m_listDoDeletedRows = new List<long>();
            m_listDeleteByUser = new List<CEntityWaterFlowMap>();
            m_proxyWaterFlowMap = CDBDataMgr.Instance.GetWaterFlowMapProxy();
        }

        /// <summary>
        /// 外部调用可控制宽度，嘿嘿
        /// </summary>
        /// <param name="bEnable"></param>
        public void SetEditMode(bool bEnable)
        {
            if (bEnable)
            {
                // 设定标题栏,默认有个隐藏列
                this.Header = new string[] 
                { 
                   CS_StationId,CS_BGTM,CS_PTNO,CS_ZR,CS_Q1,CS_Q2,CS_Q3,CS_Q4,CS_Q5,CS_Q6,CS_currQ
                };
                //this.HideColomns = new int[]
                //{
                //    12 /*CS_RecordId*/
                //};

                //开启编辑模式,设置可编辑列
                DataGridViewCheckBoxColumn deleteCol = new DataGridViewCheckBoxColumn();
                base.SetColumnEditStyle(0, deleteCol);

                ////// 设置站号编辑列
                //DataGridViewNumericUpDownColumn stationIdCol = new DataGridViewNumericUpDownColumn();
                //// stationId.Maximum = Decimal.MaxValue;
                //base.SetColumnEditStyle(1, stationIdCol);


                ////// 设置站号编辑列
                //DataGridViewNumericUpDownColumn PTNOCol = new DataGridViewNumericUpDownColumn();
                //// stationId.Maximum = Decimal.MaxValue;
                //base.SetColumnEditStyle(3, PTNOCol);

                // 设置水位编辑列
                DataGridViewNumericUpDownColumn waterCol = new DataGridViewNumericUpDownColumn();
                waterCol.Maximum = Decimal.MaxValue;
                waterCol.Minimum = Decimal.MinValue;
                waterCol.DecimalPlaces = 2;
                base.SetColumnEditStyle(4, waterCol);

                //// 设置流量1编辑列
                DataGridViewNumericUpDownColumn Q1Col = new DataGridViewNumericUpDownColumn();
                Q1Col.Maximum = Decimal.MaxValue;
                Q1Col.Minimum = Decimal.MinValue;
                Q1Col.DecimalPlaces = 3;
                base.SetColumnEditStyle(5, Q1Col);

                //// 设置流量2编辑列
                DataGridViewNumericUpDownColumn Q2Col = new DataGridViewNumericUpDownColumn();
                Q2Col.Maximum = Decimal.MaxValue;
                Q2Col.Minimum = Decimal.MinValue;
                Q2Col.DecimalPlaces = 3;
                base.SetColumnEditStyle(6, Q2Col);

                //// 设置流量3编辑列
                DataGridViewNumericUpDownColumn Q3Col = new DataGridViewNumericUpDownColumn();
                Q3Col.Maximum = Decimal.MaxValue;
                Q3Col.Minimum = Decimal.MinValue;
                Q3Col.DecimalPlaces = 3;
                base.SetColumnEditStyle(7, Q3Col);

                //// 设置流量4编辑列
                DataGridViewNumericUpDownColumn Q4Col = new DataGridViewNumericUpDownColumn();
                Q4Col.Maximum = Decimal.MaxValue;
                Q4Col.Minimum = Decimal.MinValue;
                Q4Col.DecimalPlaces = 3;
                base.SetColumnEditStyle(8, Q4Col);

                //// 设置流量5编辑列
                DataGridViewNumericUpDownColumn Q5Col = new DataGridViewNumericUpDownColumn();
                Q5Col.Maximum = Decimal.MaxValue;
                Q5Col.Minimum = Decimal.MinValue;
                Q5Col.DecimalPlaces = 3;
                base.SetColumnEditStyle(9, Q5Col);

                //// 设置流量6编辑列
                DataGridViewNumericUpDownColumn Q6Col = new DataGridViewNumericUpDownColumn();
                Q6Col.Maximum = Decimal.MaxValue;
                Q6Col.Minimum = Decimal.MinValue;
                Q6Col.DecimalPlaces = 3;
                base.SetColumnEditStyle(10, Q6Col);


                var cmb_currQ = new DataGridViewComboBoxColumn();
                cmb_currQ.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

                cmb_currQ.Items.Add("1");
                cmb_currQ.Items.Add("2");
                cmb_currQ.Items.Add("3");
                cmb_currQ.Items.Add("4");
                cmb_currQ.Items.Add("5");
                cmb_currQ.Items.Add("6");
                base.SetColumnEditStyle(11, cmb_currQ);

                // 设置删除列的宽度
                this.Columns[0].Width = 15; //删除列宽度为20
                this.Columns[2].Width = 25; //删除列宽度为25
                this.Columns[11].Width = 25; //删除列宽度为25
            }
            else
            {
                // 只读模式未编写
                // 设定标题栏,默认有个隐藏列
                this.Header = new string[] 
                { 
                   CS_StationId,CS_BGTM,CS_PTNO,CS_ZR,CS_Q1,CS_Q2,CS_Q3,CS_Q4,CS_Q5,CS_Q6,CS_currQ
                };
                //this.Columns[0].Width = 25; //删除列宽度为25
                //this.Columns[1].Width = 120; //删除列宽度为25
                //this.Columns[2].Width = 15; //删除列宽度为25
                //this.Columns[3].Width = 15; //删除列宽度为25
                //this.Columns[4].Width = 40; //删除列宽度为25
                //this.Columns[5].Width = 40; //删除列宽度为25
                //this.Columns[6].Width = 40; //删除列宽度为25
                //this.Columns[7].Width = 40; //删除列宽度为25
                //this.Columns[8].Width = 40; //删除列宽度为25
                //this.Columns[9].Width = 40; //删除列宽度为25
                //this.Columns[10].Width = 25; //删除列宽度为25
                AutoSizeColumn(this);
                this.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                //this.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

        }

        /// <summary>
        /// 使DataGridView的列自适应宽度
        /// </summary>
        /// <param name="dgViewFiles"></param>
        private void AutoSizeColumn(DataGridView dgViewFiles)
        {
            int width = 0;
            //使列自使用宽度
            //对于DataGridView的每一个列都调整
            for (int i = 0; i < dgViewFiles.Columns.Count; i++)
            {
                //将每一列都调整为自动适应模式
                dgViewFiles.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
                //记录整个DataGridView的宽度
                width += dgViewFiles.Columns[i].Width;
            }
            //判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，
            //则将DataGridView的列自动调整模式设置为显示的列即可，
            //如果是小于原来设定的宽度，将模式改为填充。
            if (width > dgViewFiles.Size.Width)
            {
                dgViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            else
            {
                dgViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            //冻结某列 从左开始 0，1，2
        }


        /// <summary>
        /// 只能显示某个站点的水位流量数据，不能同时显示,这是首先需要调用的一个方法
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="listMaps"></param>
        public void InitDatas(string stationId, List<CEntityWaterFlowMap> listMaps)
        {
            try
            {
                ClearAllState(); //清空内部状态，虚拟方法
                m_strStationId = stationId;
                m_listData = listMaps;
                // 当然CEntityWaterFlowMap里面也是有站点的名字的
                SetWaterFlowMap(listMaps);
                UpdateDataToUI();
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        public void Revert(string stationid)
        {
            try
            {
                // 清空所有状态
                this.ClearAllState();
                this.Hide();
                if (m_proxyWaterFlowMap != null && stationid != "")
                {
                    this.SetWaterFlowMap(m_proxyWaterFlowMap.QueryMapsByStationId(stationid));
                    UpdateDataToUI();
                }
                this.Show();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public bool LoadData_1()
        {
            try
            {
                if (m_proxyWaterFlowMap != null)
                {
                    //  this.SetWaterFlowMap(CDBDataMgr.Instance.GetAllStationData());
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        public void AddNewRecord()
        {
            // ID为-1的表示为新增的记录，此时的站点ID为-1
            base.AddRow(new string[]
            {
                "False","","","",   
                "","","","",
                "","","",""
            }, EDataState.ENormal);
            base.ClearSelection();
            // 设定为编辑过的行，便于在获取值的时候直接取得
            base.m_listEditedRows.Add(m_dataTable.Rows.Count - 1);
            base.UpdateDataToUI();
            base.CurrentCell = base.Rows[base.Rows.Count - 1].Cells[1];
        }

        public bool GetAllModifiedData(out SModifiedData data)
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            data = new SModifiedData();
            if (!AssertDataValid())
            {
                // 数据不合法
                return false;
            }
            List<CEntityWaterFlowMap> listUpdated = null;
            List<CEntityWaterFlowMap> listAdded = null;
            List<long> listDeleted = null;

            if (GetUpdatedData(out listUpdated, out listAdded) &&
                GetDeletedData(out listDeleted))
            {
                // 只有都为true的情况下，才认为此次修改有效

                GenerateCombinationData(listUpdated, listAdded, m_listDoDeletedRows);
                data.listUpdated = listUpdated;
                data.listDeleted = listDeleted;
                data.listAdded = listAdded;
                //data.listCombination = m_listData;
                return true;
            }
            return false;
            //return true;
        }

        /// <summary>
        /// 保存列表状态，此方法要在调用GetAllModifiedData之前，否则都被清空了
        /// </summary>
        /// <returns></returns>
        public bool SaveStatus(out SStatus status)
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            status = new SStatus();
            status.listEditedRows = new List<int>(m_listEditedRows.ToArray());
            status.listMarkDeleteRows = new List<int>(m_listMaskedDeletedRows.ToArray());
            status.listDoDeletes = new List<long>(m_listDoDeletedRows.ToArray());
            return true;
        }

        public bool RestoreStatus(SStatus status)
        {
            // 恢复状态
            m_listEditedRows = new List<int>(status.listEditedRows.ToArray());
            m_listDoDeletedRows = new List<long>(status.listDoDeletes.ToArray());
            m_listMaskedDeletedRows = new List<int>(status.listMarkDeleteRows.ToArray());
            //this.SuspendLayout();
            foreach (int i in status.listMarkDeleteRows)
            {
                // 标记为删除
                base.UpdateRowData(i, new string[] 
                {
                    "True",
                    m_listData[i].StationID.ToString(),
                    m_listData[i].BGTM.ToString(),
                    m_listData[i].PTNO.ToString(),
                    m_listData[i].ZR.ToString("0.00"),
                    m_listData[i].Q1.ToString("0.000"),
                    m_listData[i].Q2.ToString("0.000"),
                    m_listData[i].Q3.ToString("0.000"),
                    m_listData[i].Q4.ToString("0.000"),
                    m_listData[i].Q5.ToString("0.000"),
                    m_listData[i].Q6.ToString("0.000"),
                    m_listData[i].currQ.ToString("0.000"),
                   // m_listData[i].RecordId.ToString()

                }, EDataState.EDeleted);
            }
            //this.ResumeLayout(false);
            return true;
        }

        public void DoDelete()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            if (base.m_listMaskedDeletedRows.Count <= 0)
            {
                MessageBox.Show("没有标记为删除的列，无需删除");
                return;
            }
            try
            {
                // 获取数据
                for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
                {
                    //// -1表示新添加的用户
                    //long recordId = long.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_StationId].Value.ToString());
                    string stationid = base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_StationId].Value.ToString();
                    DateTime bgtm = DateTime.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_BGTM].Value.ToString());
                    int ptno = int.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_PTNO].Value.ToString());
                    CEntityWaterFlowMap tmp = new CEntityWaterFlowMap();
                    tmp.StationID = stationid;
                    tmp.BGTM = bgtm;
                    tmp.PTNO = ptno;
                    // 并且从编辑项中减去这一行
                    m_listEditedRows.Remove(m_listMaskedDeletedRows[i]);
                    m_listDeleteByUser.Add(tmp);
                    //if (recordId != -1)
                    //{
                    //    m_listDoDeletedRows.Add(recordId);
                    //}
                }
                // 将某些行标记为不可见
                for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
                {
                    //base.Rows[i].Visible = false;
                    base.DeleteRowData(m_listMaskedDeletedRows[i]);
                }

                m_proxyWaterFlowMap.DeleteRange(m_listDeleteByUser);
                //   MessageBox.Show("删除成功！");
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
            {
                MessageBox.Show("删除失败！");
            }
            m_listMaskedDeletedRows.Clear(); //清空
            base.UpdateDataToUI();
            MessageBox.Show("删除成功！");
        }

        #endregion 公共方法

        #region 重写
        // 重写Cell值改变事件
        protected override void EHValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0)
                {
                    if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
                    {
                        // 删除列
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
                    else
                    {
                        // 非删除列
                        base.EHValueChanged(sender, e);
                    }
                    base.UpdateDataToUI();
                }
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
            }
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

        // 清空所有状态
        protected override void ClearAllState()
        {
            base.ClearAllState();
            m_listDoDeletedRows.Clear();
        }

        #endregion 重写

        #region 帮助方法
        /// <summary>
        /// 设置表格内容为指定参数内容
        /// </summary>
        /// <param name="listMaps"></param>
        private void SetWaterFlowMap(List<CEntityWaterFlowMap> listMaps)
        {
            try
            {
                base.ClearAllRows();
                string q1 = "--", q2 = "--", q3 = "--", q4 = "--", q5 = "--", q6 = "--";
                for (int i = 0; i < listMaps.Count; ++i)
                {
                    if(listMaps[i].Q1 >= 0)
                    {
                        q1 = listMaps[i].Q1.ToString("0.000");
                    }
                    if (listMaps[i].Q2 >= 0)
                    {
                        q2 = listMaps[i].Q2.ToString("0.000");
                    }
                    if (listMaps[i].Q3 >= 0)
                    {
                        q3 = listMaps[i].Q3.ToString("0.000");
                    }
                    if (listMaps[i].Q4 >= 0)
                    {
                        q4 = listMaps[i].Q4.ToString("0.000");
                    }
                    if (listMaps[i].Q5 >= 0)
                    {
                        q5 = listMaps[i].Q5.ToString("0.000");
                    }
                    if (listMaps[i].Q6 >= 0)
                    {
                        q6 = listMaps[i].Q6.ToString("0.000");
                    }
                    base.AddRow(new string[] 
                {
                    //"False",
                    listMaps[i].StationID.ToString(),
                    listMaps[i].BGTM.ToString(),
                    listMaps[i].PTNO.ToString(),
                    listMaps[i].ZR.ToString("0.00"),
                    q1,
                    q2,
                    q3,
                    q4,
                    q5,
                    q6,
                    listMaps[i].currQ.ToString(),
                  //  listMaps[i].RecordId.ToString()
                }, EDataState.ENormal);
                }
                UpdateDataToUI();
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch (Exception e) { }
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
        }

        /// <summary>
        /// 获取更新的和新增的数据记录
        /// </summary>
        /// <param name="listUpdated"></param>
        /// <param name="listAdded"></param>
        /// <returns></returns>
        private bool GetUpdatedData(out List<CEntityWaterFlowMap> listUpdated,
            out List<CEntityWaterFlowMap> listAdded)
        {
            // 标记为删除的就不需要添加的修改或者添加的记录中了
            listUpdated = new List<CEntityWaterFlowMap>();
            listAdded = new List<CEntityWaterFlowMap>();
            try
            {
                this.Invoke((Action)delegate { Enabled = false; });
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
                    CEntityWaterFlowMap wfmap = new CEntityWaterFlowMap();
                    wfmap.StationID = base.Rows[m_listEditedRows[i]].Cells[CS_StationId].Value.ToString();
                    wfmap.BGTM = DateTime.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_BGTM].Value.ToString());
                    wfmap.PTNO = int.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_PTNO].Value.ToString());
                    wfmap.ZR = Decimal.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_ZR].Value.ToString());
                    wfmap.Q1 = Decimal.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_Q1].Value.ToString());
                    wfmap.Q2 = Decimal.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_Q2].Value.ToString());
                    wfmap.Q3 = Decimal.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_Q3].Value.ToString());
                    wfmap.Q4 = Decimal.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_Q4].Value.ToString());
                    wfmap.Q5 = Decimal.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_Q5].Value.ToString());
                    wfmap.Q6 = Decimal.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_Q6].Value.ToString());
                    wfmap.currQ = Decimal.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_currQ].Value.ToString());
                    // wfmap.StationID = m_strStationId; //使用默认的站点ID
                    //if (wfmap.RecordId == -1)
                    //{
                    // 添加的新记录
                    // listAdded.Add(wfmap);
                    //}
                    //else
                    //{
                    //    // 更新以前的记录
                    listUpdated.Add(wfmap);
                    //}

                }
                m_listEditedRows.Clear();   //清空此次记录，标记为所有的都为未修改
                return true;
            }
            catch (System.Exception ex)
            {
                // 获取失败
                Debug.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                this.Invoke((Action)delegate { Enabled = true; });
            }

        }

        /// <summary>
        /// 获取标记为删除的数据记录
        /// </summary>
        /// <param name="listDeleted"></param>
        /// <returns></returns>
        private bool GetDeletedData(out List<long> listDeleted)
        {
            listDeleted = new List<long>(m_listDoDeletedRows.ToArray());
            try
            {
                // 添加完全标记为删除的列
                this.Invoke((Action)delegate { Enabled = false; });
                for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
                {
                    // -1是新增加的用户
                    long recordId = long.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_StationId].Value.ToString());
                    if (recordId != -1)
                    {
                        listDeleted.Add(recordId);
                    }
                }
                m_listMaskedDeletedRows.Clear();   //清空此次记录
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                this.Invoke((Action)delegate { Enabled = true; });
            }

        }

        private bool GenerateCombinationData(List<CEntityWaterFlowMap> listUpdated,
            List<CEntityWaterFlowMap> listAdded, List<long> listDoDeletedRows)
        {
            try
            {
                // 根据所做的修改，生成修改后的数据体
                listUpdated = new List<CEntityWaterFlowMap>(listUpdated.ToArray());
                listDoDeletedRows = new List<long>(m_listDoDeletedRows.ToArray());
                for (int i = 0; i < m_listData.Count; )
                {
                    // 先对于删除列，删除列中的元素
                    int findindex = listDoDeletedRows.IndexOf(long.Parse(m_listData[i].StationID));
                    if (-1 != findindex)
                    {
                        // 会不会导致i的变化呢？此时i不变？？？
                        m_listData.RemoveAt(i);
                        listDoDeletedRows.RemoveAt(findindex);
                    }
                    else
                    {
                        //对于修改过的数据
                        for (int j = 0; j < listUpdated.Count; ++j)
                        {
                            if (listUpdated[j].RecordId == m_listData[i].RecordId)
                            {
                                // 找到匹配，更新数值，并且继续
                                m_listData[i] = listUpdated[j];
                                listUpdated.RemoveAt(j); //删除多余的元素
                                break;
                            }
                        }
                        i += 1;
                    }
                }
                // 再把添加过的元素加到数组末尾
                m_listData.AddRange(listAdded);
                return true;
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
            {
                return true;
            }
        }

        private bool AssertDataValid()
        {
            // 判断用户输入是否合法
            try
            {
                this.Invoke((Action)delegate { Enabled = false; });
                List<int> listEditRows = new List<int>();
                foreach (int item in base.m_listEditedRows)
                {
                    if (!m_listMaskedDeletedRows.Contains(item))
                    {
                        listEditRows.Add(item);
                    }
                }
                for (int i = 0; i < listEditRows.Count; ++i)
                {
                    if (base.Rows[m_listEditedRows[i]].Cells[CS_Q1].Value.ToString().Equals(""))
                    {
                        MessageBox.Show("流量1不能为空");
                        return false;
                    }
                    //if (base.Rows[m_listEditedRows[i]].Cells[CS_Q2].Value.ToString().Equals(""))
                    //{
                    //    MessageBox.Show("流量2不能为空");
                    //    return false;
                    //}
                    //if (base.Rows[m_listEditedRows[i]].Cells[CS_Q3].Value.ToString().Equals(""))
                    //{
                    //    MessageBox.Show("流量3不能为空");
                    //    return false;
                    //}
                    //if (base.Rows[m_listEditedRows[i]].Cells[CS_Q4].Value.ToString().Equals(""))
                    //{
                    //    MessageBox.Show("流量4不能为空");
                    //    return false;
                    //}
                    //if (base.Rows[m_listEditedRows[i]].Cells[CS_Q5].Value.ToString().Equals(""))
                    //{
                    //    MessageBox.Show("流量5不能为空");
                    //    return false;
                    //}
                    //if (base.Rows[m_listEditedRows[i]].Cells[CS_Q6].Value.ToString().Equals(""))
                    //{
                    //    MessageBox.Show("流量6不能为空");
                    //    return false;
                    //}
                    //if (base.Rows[m_listEditedRows[i]].Cells[CS_ZR].Value.ToString().Equals(""))
                    //{
                    //    MessageBox.Show("水位不能为空");
                    //    return false;
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                this.Invoke((Action)delegate { Enabled = true; });
            }
        }

        #endregion 帮助方法



    }//end of class
}
