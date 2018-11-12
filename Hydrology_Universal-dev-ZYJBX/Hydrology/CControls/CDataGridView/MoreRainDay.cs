using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using Hydrology.Forms;
using Hydrology.Utils;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace Hydrology.CControls
{
    /// <summary>
    /// 单站平均雨量日报表
    /// </summary>
    class MoreRainDay : CExDataGridView
    {
        #region 静态常
        private static readonly string day = "站名";
        private static readonly string stationid = "站号";
        private static readonly string dayRains = "日雨量";
        #endregion 静态常量

        #region 成员变量
#pragma warning disable CS0169 // 从不使用字段“MoreRainDay.m_proxyWater”
        private IWaterProxy m_proxyWater;
#pragma warning restore CS0169 // 从不使用字段“MoreRainDay.m_proxyWater”
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

        public MoreRainDay()
            : base()
        {
            List<string> headerTmp = new List<string>();
            headerTmp.Add("|");
            headerTmp.Add(day);
            headerTmp.Add(stationid);
            for (int i = 9; i < 24; i++)
            {
                headerTmp.Add(i.ToString());
            }
            for (int i = 0; i < 9; i++)
            {
                headerTmp.Add(i.ToString());
            }
            headerTmp.Add(dayRains);
            this.Header = headerTmp.ToArray();
            this.ReCalculateSize();
            this.BPartionPageEnable = false; //不启用分页功能
            // 初始化数据源
            //m_proxyWater = CDBDataMgr.Instance.GetWaterProxy();
        }
        public void SetFilter(List<string> stations, DateTime time)
        {
            //添加代码
            List<string[]> listShowRows = new List<string[]>();
            List<EDataState> listState = new List<EDataState>();
            for (int i = 0; i < stations.Count; i++)
            {
                string[] stationsArray = stations[i].Split('|');
                if (stationsArray.Length == 2)
                {
                    string station = stationsArray[0];
                    string name = stationsArray[1];
                    EDataState state;
                    listShowRows.Add(GetShowStringList(station, name, time, out state).ToArray());
                    listState.Add(state);
                }
            }
            base.ClearAllRows();
            base.AddRowRange(listShowRows, listState);
            base.UpdateDataToUI();

        }
        public void ExportToExcel(DateTime dt)
        {
            //添加代码
            int year = dt.Year;
            int month = dt.Month;
            int days = dt.Day;
            string name = year + "年" + month + "月" + days + "日";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = name + "雨量表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataTable.Columns.Add("|");
                dataTable.Columns.Add(day);
                for (int i = 9; i < 24; i++)
                {
                    dataTable.Columns.Add(i.ToString());
                }
                for (int i = 0; i < 9; i++)
                {
                    dataTable.Columns.Add(i.ToString());
                }
                dataTable.Columns.Add(dayRains);
                // 逐行读取数据
                int iRowCount = m_dataTable.Rows.Count;
                DataRow row0 = dataTable.NewRow();

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
                if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, name + "雨量表"))
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
            }
        }

        public void ExportToExcelNew(DataGridView dgv, DateTime dt)
        {
            int year = dt.Year;
            int month = dt.Month;
            int days = dt.Day;
            string name = year + "年" + month + "月" + days + "日";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = name + "雨量表";
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
                    Range range = objsheet.Range[objsheet.Cells[1, 9], objsheet.Cells[1, 26]];
                    range.Font.Bold = true;
                    range.Font.ColorIndex = 0;
                    range2.Font.Bold = true;
                    range2.Font.ColorIndex = 0;
                    objsheet.Cells[1, 7] = name + "雨量表";
                    int displayColumnsCount = 1;
                    objsheet.Cells[2, displayColumnsCount] = "|";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "站号";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "站名";
                    displayColumnsCount++;
                    for (int i = 9; i <= 23; i++)
                    {
                        if (dgv.Columns[i].Visible == true)
                        {

                            objsheet.Cells[2, displayColumnsCount] = "     " + (i + "时").ToString().Trim();
                            displayColumnsCount++;
                        }
                    }
                    for (int i = 0; i <= 8; i++)
                    {
                        if (dgv.Columns[i].Visible == true)
                        {

                            objsheet.Cells[2, displayColumnsCount] = "      " + (i + "时").ToString().Trim();
                            displayColumnsCount++;
                        }
                    }
                    objsheet.Cells[2, displayColumnsCount] = "    日雨量";
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

        private List<string> GetShowStringList(string station, string name, DateTime time, out EDataState state)
        {
            List<string> results = new List<string>();
            double sum = 0;
            results.Add("|");
            results.Add(station);
            results.Add(name);
            List<CEntityRain> temResults = new List<CEntityRain>();
            int year = time.Year;
            int month = time.Month;
            int day = time.Day;
            DateTime tmp = new DateTime(year, month, day, 9, 0, 0);
            temResults = CDBDataMgr.Instance.GetMRaainsForTable(station, tmp);
            int length = temResults.Count;
            for (int i = 0; i < length; i++)
            {
                double rain = double.Parse(temResults[i].PeriodRain.ToString());
                sum = sum + rain;
            }
            if (length == 24)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (temResults[i].PeriodRain.ToString() == "0.0")
                    {
                        results.Add(" ");
                    }
                    else
                    {
                        results.Add(temResults[i].PeriodRain.ToString());
                    }

                }
            }
            else
            {
                List<int> hours = new List<int>();
                int array = 0;
                for (int j = 0; j < length; j++)
                {
                    DateTime dt = temResults[j].TimeCollect;
                    int d = dt.Hour;
                    hours.Add(d);
                }
                for (int k = 9; k < 24; k++)
                {
                    if (hours.Contains(k))
                    {
                        if (temResults[array].PeriodRain.ToString() == "0.0")
                        {
                            results.Add(" ");
                        }
                        else
                        {
                            results.Add(temResults[array].PeriodRain.ToString());
                        }

                        array = array + 1;
                    }
                    else
                    {
                        results.Add("--");
                    }
                }
                for (int k = 0; k < 9; k++)
                {
                    if (hours.Contains(k))
                    {
                        if (temResults[array].PeriodRain.ToString() == "0.0")
                        {
                            results.Add(" ");
                        }
                        else
                        {
                            results.Add(temResults[array].PeriodRain.ToString());
                        }

                        array = array + 1;
                    }
                    else
                    {
                        results.Add("--");
                    }
                }
            }
            results.Add(sum.ToString());
            double rate = 0.5;
            state = GetState(rate);
            return results;
        }
        //private List<string> GetShowStringList(string station, DateTime time, out EDataState state)
        //{
        //    List<string> results = new List<string>();
        //    double sum = 0;
        //    results.Add("|");
        //    results.Add(station);
        //    List<CEntityRain> temResults = new List<CEntityRain>();
        //    int year = time.Year;
        //    int month = time.Month;
        //    int day = time.Day;
        //    DateTime tmp = new DateTime(year, month, day, 9, 0, 0);
        //    temResults = CDBDataMgr.Instance.GetMRaainsForTable(station, tmp);
        //    int length = temResults.Count;

        //    for (int i = 0; i < length; i++)
        //    {
        //        if (temResults[i].PeriodRain.ToString() != "" && temResults[i].PeriodRain != -9999)
        //        {
        //            double rain = double.Parse(temResults[i].PeriodRain.ToString());
        //            sum = sum + rain;
        //        }
        //    }
        //    if (length == 24)
        //    {
        //        for (int i = 0; i < 24; i++)
        //        {
        //            if (temResults[i].PeriodRain.ToString() == "" || temResults[i].PeriodRain == -9999)
        //            {
        //                results.Add(" ");
        //            }
        //            else
        //            {
        //                results.Add(temResults[i].PeriodRain.ToString());
        //            }

        //        }
        //    }
        //    else
        //    {
        //        List<int> hours = new List<int>();
        //        int array = 0;
        //        for (int j = 0; j < length; j++)
        //        {
        //            DateTime dt = temResults[j].TimeCollect;
        //            int d = dt.Hour;
        //            hours.Add(d);
        //        }
        //        for (int k = 9; k < 24; k++)
        //        {
        //            if (hours.Contains(k))
        //            {
        //                if (temResults[array].PeriodRain.ToString() == "" || temResults[array].PeriodRain == -9999)
        //                {
        //                    results.Add(" ");
        //                }
        //                else
        //                {
        //                    results.Add(temResults[array].PeriodRain.ToString());
        //                }

        //                array = array + 1;
        //            }
        //            else
        //            {
        //                results.Add("--");
        //            }
        //        }
        //        for (int k = 0; k < 9; k++)
        //        {
        //            if (hours.Contains(k))
        //            {
        //                if (temResults[array].PeriodRain.ToString() == "" || temResults[array].PeriodRain == -9999)
        //                {
        //                    results.Add(" ");
        //                }
        //                else
        //                {
        //                    results.Add(temResults[array].PeriodRain.ToString());
        //                }

        //                array = array + 1;
        //            }
        //            else
        //            {
        //                results.Add("--");
        //            }
        //        }
        //    }
        //    results.Add(sum.ToString());
        //    double rate = 0.5;
        //    state = GetState(rate);
        //    return results;
        //}
        private EDataState GetState(double rate)
        {
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
        public void ReCalculateSize()
        {
            this.Columns[0].Width = 80;
            this.Columns[1].Width = 80;
            for (int i = 2; i < 26; ++i)
            {
                this.Columns[i].Width = 45; //设定宽度
            }
            //this.Columns[26].Width = 80;

        }
    }
}