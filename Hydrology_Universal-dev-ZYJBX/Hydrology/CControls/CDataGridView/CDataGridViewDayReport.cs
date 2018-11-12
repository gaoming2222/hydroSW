using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using Hydrology.DataMgr;
using System.Windows.Forms;
using System.Data;
using Hydrology.Forms;
using Hydrology.Utils;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace Hydrology.CControls
{
    /// <summary>
    /// 日报表的DataGridView
    /// </summary> 
    class CDataGridViewDayReport : CExDataGridView
    {
        #region 静态常量
        private static readonly string CS_StationID = "站号";
        private static readonly string CS_StationName = "站名";
        private static readonly string CS_TheoreticalCount = "应收记录";
        private static readonly string CS_ActualCount = "实收记录";
        private static readonly string CS_CommunicationRate = "畅通率(%)";
        private static readonly string CS_GSM = "GSM畅通率";
        private static readonly string CS_GPRS = "GPRS畅通率";
        private static readonly string CS_RecordRight = "√";
        private static readonly string CS_RecordError = "×";
        #endregion  静态常量

        #region 成员变量
        // private ICommunicationRateProxy m_proxyCommnunicationRate; //数据库查询对象
        //private double m_bRedMax = 0.95; //低于m_bRedMax红色
        //private double m_bYellowMax = 0.94; // 介于0.25和0.5之间，黄色，其余都是绿色

        private int m_TotalCount = 0;
        private int m_MoreThan95 = 0;
        public int TotalCount
        {
            get { return m_TotalCount; }
        }
        public int MoreThan95Count
        {
            get { return m_MoreThan95; }
        }
        public int LessThan95Count
        {
            get { return m_TotalCount - m_MoreThan95; }
        }

        #endregion 成员变量

        #region 公共方法
        public CDataGridViewDayReport()
            : base()
        {
            List<string> headerTmp = new List<string>();
            headerTmp.Add(CS_StationID);
            headerTmp.Add(CS_StationName);
            for (int i = 9; i < 24; ++i)
            {
                headerTmp.Add(i.ToString());
            }
            for (int i = 0; i < 9; ++i)
            {
                headerTmp.Add(i.ToString());
            }
            headerTmp.Add(CS_TheoreticalCount);
            headerTmp.Add(CS_ActualCount);
            headerTmp.Add(CS_CommunicationRate);
            headerTmp.Add(CS_GSM);
            headerTmp.Add(CS_GPRS);
            this.Header = headerTmp.ToArray();
            this.ReCalculateSize();
            this.BPartionPageEnable = false; //不启用分页功能

            // 初始化数据源
            // m_proxyCommnunicationRate = CDBDataMgr.Instance.GetCommunicationRateProxy();
        }

        public void ReCalculateSize()
        {
            //this.Rows[1].Height = 60;
            this.Columns[0].Width = 45;
            this.Columns[1].Width = 45;
            for (int i = 2; i < 26; ++i)
            {
                this.Columns[i].Width = 35; //设定宽度
            }
            for (int i = 26; i < 28; i++)
            {
                this.Columns[i].Width = 60;
            }
            for (int i = 28; i < 31; i++)
            {
                this.Columns[i].Width = 100;
            }
        }

        /// <summary>
        /// 根据分中心查询所有站点，查询某一天的畅通率，如果subcenterName为null，则查询所有测站
        /// </summary>
        /// <param name="subcenterName">分中心的名字，一定要正确</param>
        /// <param name="time"></param>
        public void SetFilter(string subcenterName, DateTime time)
        {
            //  清空统计数据
            m_MoreThan95 = 0;
            m_TotalCount = 0;
            // 清空之前的数据
            List<CEntityStation> stations = new List<CEntityStation>();
            if (null == subcenterName)
            {
                // 统计所有站点的畅通率
                stations = CDBDataMgr.Instance.GetAllStation();
            }
            else
            {
                // 统计某个分中心的畅通率
                List<CEntityStation> stationsAll = CDBDataMgr.Instance.GetAllStation();
                CEntitySubCenter subcenter = CDBDataMgr.Instance.GetSubCenterByName(subcenterName);
                for (int i = 0; i < stationsAll.Count; ++i)
                {
                    if (stationsAll[i].SubCenterID == subcenter.SubCenterID)
                    {
                        // 该测站在分中心内，添加到要查询的列表中
                        stations.Add(stationsAll[i]);
                    }
                }
            }
            // 统计列表stations中所有的站点
            List<string[]> listShowRows = new List<string[]>();
            List<EDataState> listState = new List<EDataState>();
            for (int i = 0; i < stations.Count; ++i)
            {
                EDataState state;
                listShowRows.Add(GetShowStringList(stations[i], time, out state).ToArray());
                listState.Add(state);
            }
            // 更新界面
            base.ClearAllRows();
            base.AddRowRange(listShowRows, listState);
            base.UpdateDataToUI();
        }

        /// <summary>
        /// 导出到Excel表格中
        /// </summary>
        public void ExportToExcel(DateTime dt)
        {
            // 弹出对话框，并导出到Excel文件
            SaveFileDialog dlg = new SaveFileDialog();
            string name = dt.Year + "年" + dt.Month + "月" + dt.Day + "日畅通率表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            dlg.FileName = name;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataTable.Columns.Add(CS_StationID, typeof(string));
                dataTable.Columns.Add(CS_StationName, typeof(string));
                for (int i = 9; i < 25; ++i)
                {
                    dataTable.Columns.Add(i.ToString(), typeof(string));
                }
                for (int i = 1; i < 9; ++i)
                {
                    dataTable.Columns.Add(i.ToString(), typeof(string));
                }

                dataTable.Columns.Add(CS_TheoreticalCount, typeof(Int32));
                dataTable.Columns.Add(CS_ActualCount, typeof(Int32));
                dataTable.Columns.Add(CS_CommunicationRate, typeof(string));
                dataTable.Columns.Add(CS_GSM);
                dataTable.Columns.Add(CS_GPRS);
                // 逐行读取数据
                int iRowCount = m_dataTable.Rows.Count;
                for (int i = 0; i < iRowCount; ++i)
                {
                    // 赋值到dataTable中去
                    DataRow row = dataTable.NewRow();
                    // 多余的一列是state
                    for (int j = 0; j < m_dataTable.Columns.Count - 1; ++j)
                    {
                        row[j] = m_dataTable.Rows[i][j];
                    }
                    dataTable.Rows.Add(row);
                }
                // 显示提示框
                CMessageBox box = new CMessageBox() { MessageInfo = "正在导出表格，请稍候" };
                box.ShowDialog(this);
                if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, name))
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

        public void ExportToExcelNew(DataGridView dgv, DateTime dt)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            string name = dt.Year + "年" + dt.Month + "月" + dt.Day + "日畅通率表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            dlg.FileName = name;
            dlg.Filter = "Excel文件(*.xlsx)|*.xlsx|Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                int rowscount = dgv.Rows.Count;
                int colscount = dgv.Columns.Count;
                //行数必须大于0 
                if (rowscount <= 0)
                {
                    MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //列数必须大于0 
                if (colscount <= 0)
                {
                    MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //行数不可以大于65536 
                if (rowscount > 65536)
                {
                    MessageBox.Show("数据记录数太多(最多不能超过65536条)，不能保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                FileInfo file = new FileInfo(dlg.FileName);
                if (file.Exists)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message, "删除失败 ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                CMessageBox box = new CMessageBox();
                box.MessageInfo = "正在导出报表";
                box.ShowDialog(this);
                this.Enabled = false;
                Microsoft.Office.Interop.Excel.Application objExcel = null;
                Microsoft.Office.Interop.Excel.Workbook objWorkbook = null;
                Microsoft.Office.Interop.Excel.Worksheet objsheet = null;
                ProgressBar tempProgressBar = new ProgressBar();
                try
                {
                    //申明对象 
                    objExcel = new Microsoft.Office.Interop.Excel.Application();
                    objWorkbook = objExcel.Workbooks.Add(Missing.Value);
                    objsheet = (Microsoft.Office.Interop.Excel.Worksheet)objWorkbook.ActiveSheet;
                    //设置EXCEL不可见 
                    objExcel.Visible = false;
                    objsheet.DisplayAutomaticPageBreaks = true;//显示分页线    
                    objsheet.PageSetup.CenterFooter = "第 &P 页，共 &N 页";
                    objsheet.PageSetup.CenterHorizontally = true;//水平居中   
                    objsheet.PageSetup.PrintTitleRows = "$1:$1";//顶端标题行    
                    objsheet.PageSetup.PaperSize = Microsoft.Office.Interop.Excel.XlPaperSize.xlPaperA4;//A4纸张大小   
                    objsheet.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;//纸张方向.纵向
                    Range range2 = objsheet.Range[objsheet.Cells[1, 1], objsheet.Cells[1, 8]];
                    Range range = objsheet.Range[objsheet.Cells[1, 9], objsheet.Cells[1, 31]];
                    range.Font.Bold = true;
                    range.Font.ColorIndex = 0;
                    range2.Font.Bold = true;
                    range2.Font.ColorIndex = 0;
                    objsheet.Cells[1, 7] = name;
                    int displayColumnsCount = 1;
                    objsheet.Cells[2, displayColumnsCount] = "    站号";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "    站名";
                    displayColumnsCount++;
                    for (int i = 9; i <= 23; i++)
                    {
                        if (dgv.Columns[i].Visible == true)
                        {

                            objsheet.Cells[2, displayColumnsCount] = (i + "时").ToString().Trim();
                            displayColumnsCount++;
                        }
                    }
                    for (int i = 0; i <= 8; i++)
                    {
                        if (dgv.Columns[i].Visible == true)
                        {

                            objsheet.Cells[2, displayColumnsCount] = (i + "时").ToString().Trim();
                            displayColumnsCount++;
                        }
                    }
                    objsheet.Cells[2, displayColumnsCount] = " 应收记录";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = " 实收记录";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "畅通率(%)";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "GSM畅通率";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "GPRS畅通率";
                    displayColumnsCount++;
                    for (int row = 0; row <= dgv.RowCount; row++)
                    {
                        //tempProgressBar.PerformStep();
                        displayColumnsCount = 1;
                        for (int col = 0; col < colscount; col++)
                        {
                            if (dgv.Columns[col].Visible == true)
                            {
                                try
                                {
                                    objsheet.Cells[row + 3, displayColumnsCount] = "'" + dgv.Rows[row].Cells[col].Value.ToString().Trim();
                                    displayColumnsCount++;
                                }
                                catch (Exception)
                                {

                                }

                            }
                        }
                    }
                    objWorkbook.SaveAs(dlg.FileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                            Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, Missing.Value, Missing.Value, Missing.Value,
                            Missing.Value, Missing.Value);
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message, "警告 ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                finally
                {
                    //关闭Excel应用 
                    if (objWorkbook != null) objWorkbook.Close(Missing.Value, Missing.Value, Missing.Value);
                    if (objExcel.Workbooks != null) objExcel.Workbooks.Close();
                    if (objExcel != null) objExcel.Quit();

                    objsheet = null;
                    objWorkbook = null;
                    objExcel = null;
                }
                this.Enabled = true;
                box.CloseDialog();
                MessageBox.Show(dlg.FileName + "导出完毕! ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }
        #endregion 公共方法

        #region 帮助方法
        private List<string> GetShowStringList(CEntityStation station, DateTime time, out EDataState state)
        {
            // 一天从9点钟开始，到第二天8点结束
            int year = time.Year;
            int month = time.Month;
            int day = time.Day;
            DateTime dt = new DateTime(year, month, day, 9, 0, 0);
            List<DateTime> results = new List<DateTime>();
            List<CEntityVoltage> tmpResults = new List<CEntityVoltage>();
            List<CEntityVoltage> GSMResults = new List<CEntityVoltage>();
            List<CEntityVoltage> GPRSResults = new List<CEntityVoltage>();
            //DateTime timeStart = new DateTime(time.Year, time.Month, time.Day, 9, 0, 0);
            //DateTime timeEnd = new DateTime(time.Year, time.Month, time.Day, 8, 0, 0);
            //timeEnd = timeEnd.AddDays(1);
            //results = m_proxyCommnunicationRate.QueryRecordByStationIdAndPeriod(station.StationID,
            //    timeStart, timeEnd);
            tmpResults = CDBDataMgr.Instance.GetVoltageForRateTable(station, dt);
            //for (int k = 0; k < tmpResults.Count; k++)
            //{
            //    int n = int.Parse(tmpResults[k].type);
            //    if (n == 3)
            //    {
            //        GPRSResults.Add(tmpResults[k]);
            //    }
            //    if (n == 6)
            //    {
            //        GSMResults.Add(tmpResults[k]);
            //    }
            //}
            //int GPRSNum = GPRSResults.Count;
            //int GSMNum = GSMResults.Count;
            List<int> hourlist = new List<int>();
            for (int i = 0; i < tmpResults.Count; ++i)
            {
                if (!hourlist.Contains(tmpResults[i].TimeCollect.Hour))
                {
                    int n = int.Parse(tmpResults[i].type);
                    if (n == 3)
                    {
                        GPRSResults.Add(tmpResults[i]);
                    }
                    if (n == 6)
                    {
                        GSMResults.Add(tmpResults[i]);
                    }
                    hourlist.Add(tmpResults[i].TimeCollect.Hour);
                }
            }
            List<string> resultUI = new List<string>();
            resultUI.Add(station.StationID);
            resultUI.Add(station.StationName);
            // 24小时是00点
            for (int i = 9; i < 24; ++i)
            {
                if (hourlist.Contains(i))
                {
                    resultUI.Add(CS_RecordRight);
                }
                else
                {
                    resultUI.Add(CS_RecordError);
                }
            }
            for (int i = 0; i < 9; ++i)
            {
                if (hourlist.Contains(i))
                {
                    resultUI.Add(CS_RecordRight);
                }
                else
                {
                    resultUI.Add(CS_RecordError);
                }
            }
            int GPRSNum = GPRSResults.Count;
            int GSMNum = GSMResults.Count;
            int reportInterval = Int16.Parse(CDBDataMgr.Instance.GetIntervalForRateTable(station));
            resultUI.Add(reportInterval.ToString()); //应收24条 需要从表中获取
            resultUI.Add(hourlist.Count.ToString()); //实收的条数
            double GSMRate = GSMNum / (double)reportInterval * 100;
            double GPRSRate = GPRSNum / (double)reportInterval * 100;
            double rate = hourlist.Count / (double)reportInterval;
            state = GetState(rate); //计算颜色
            CalcState(rate);
            rate = rate * 100; //百分比
            resultUI.Add(rate.ToString("0.00") + "%"); //保留两位小数
            resultUI.Add(GSMRate.ToString("0.00") + "%");
            resultUI.Add(GPRSRate.ToString("0.00") + "%");
            return resultUI;
        }

        private EDataState GetState(double rate)
        {
            //EDataState state = EDataState.ENormal;
            //if (rate < m_bRedMax)
            //{
            //    // 红色
            //    state = CExDataGridView.EDataState.EError;
            //}
            //else if (rate <= m_bYellowMax)
            //{
            //    // 黄色
            //    state = EDataState.EWarning;
            //}
            //return state;

            //全部显示为白色，2014年04月19日改
            return EDataState.ENormal;
        }
        private void CalcState(double rate)
        {
            if (rate >= 0.95)
            {
                m_MoreThan95 += 1;
            }
            m_TotalCount += 1;
        }
        #endregion 帮助方法

        #region 重写
        protected override void OnSizeChanged(object sender, EventArgs e)
        {
            //ReCalculateSize();
        }
        #endregion 重写
    }
}
