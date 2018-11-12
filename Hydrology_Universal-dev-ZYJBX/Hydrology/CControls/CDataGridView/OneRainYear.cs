using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using Hydrology.Forms;
using Hydrology.Utils;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace Hydrology.CControls.CDataGridView
{
    class OneRainYear : CExDataGridView
    {
        #region 静态常量
        private static readonly string staMonth = "月份";
        private static readonly string aver = "月雨量";
        private static int flag = 0;
        private static double MonthSum = 0;
        #endregion 静态常量

        #region 成员变量
        private IRainProxy m_proxyRain;//数据库查询对象
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

        public OneRainYear()
            : base()
        {
            List<string> headerTmp = new List<string>();
            headerTmp.Add(staMonth);
            for (int i = 1; i <= 31; i++)
            {
                headerTmp.Add(i.ToString() + "日");
            }
            headerTmp.Add(aver);
            this.Header = headerTmp.ToArray();
            this.ReCalculateSize();
            this.BPartionPageEnable = false; //不启用分页功能
            // 初始化数据源
            m_proxyRain = CDBDataMgr.Instance.GetRainProxy();

        }

        public void SetFilter(string StationID, DateTime time)
        {
            List<string[]> listShowRows = new List<string[]>();
            int year = time.Year;
            int month = time.Month;
            int days = DateTime.DaysInMonth(year, month);
            List<EDataState> listState = new List<EDataState>();
            for (int i = 1; i < 13; i++)
            {
                flag = i;
                MonthSum = 0;
                DateTime tmp = new DateTime(year, i, 1, 9, 0, 0);
                EDataState state;
                listShowRows.Add(GetShowStringList(StationID, tmp, out state).ToArray());
                listState.Add(state);
            }
            base.ClearAllRows();
            base.AddRowRange(listShowRows, listState);
            base.UpdateDataToUI();
        }

        private List<string> GetShowStringList(string stationID, DateTime time, out EDataState state)
        {
            List<CEntityRain> tmpResults = new List<CEntityRain>();
            List<string> results = new List<string>();
            results.Add(flag + "月");
            tmpResults = CDBDataMgr.Instance.GetRainsForYearTable(stationID, time);
            for (int i = 1; i <= 31; i++)
            {
                DateTime time1 = time.AddHours(23);
                double sum = 0;
                int count = 0;
                //for (int k = 110; k < tmpResults.Count; k++)
                for (int k = 0; k < tmpResults.Count; k++)
                {
                    if (tmpResults[k].TimeCollect >= time && tmpResults[k].TimeCollect <= time1 && tmpResults[k].TimeCollect.Minute == 0)
                    {
                        if (tmpResults[k].PeriodRain != -9999)
                        {
                            count = count + 1;
                            //sum = double.Parse(sum + (tmpResults[k].PeriodRain.ToString()));
                            sum = sum + double.Parse(tmpResults[k].PeriodRain.ToString());

                        }
                    }
                }
                if (count == 0)
                {
                    results.Add("--");
                }
                else
                {
                    if (sum != 0)
                    {
                        results.Add((sum).ToString("0.0"));
                    }
                    else
                    {
                        results.Add((sum).ToString(" "));
                    }

                    MonthSum = MonthSum + sum;
                }
                time = time.AddDays(1);
            }
            results.Add(MonthSum.ToString());
            double rate = 0.5;
            state = GetState(rate);
            return results;
        }

        public void ReCalculateSize()
        {

            for (int i = 0; i < 32; ++i)
            {
                this.Columns[i].Width = 40; //设定宽度
            }
            this.Columns[32].Width = 60;
        }

        private EDataState GetState(double rate)
        {
            return EDataState.ENormal;
        }

        public void ExportToExcel(DateTime dt, string stationName)
        {
            // 弹出对话框，并导出到Excel文件
            int year = dt.Year;
            int month = dt.Month;
            string name = stationName + "_" + year + "年";
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.FileName = name + "雨量表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            PrintDialog plg = new PrintDialog();

            PrinterSettings pss = new System.Drawing.Printing.PrinterSettings();
            pss.DefaultPageSettings.Landscape = true;
            plg.PrinterSettings = pss;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataTable.Columns.Add(staMonth);
                for (int i = 1; i <= 31; ++i)
                {
                    dataTable.Columns.Add(i.ToString() + "日");
                }
                dataTable.Columns.Add(aver);
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
                if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, name + "降水量月表"))
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

        public void ExportToExcelNew(DataGridView dgv, DateTime dt, string stationName)
        {
            int year = dt.Year;
            int month = dt.Month;
            string name = stationName + "_" + year + "年";
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
                    objsheet.Cells[2, displayColumnsCount] = "站点/小时";
                    displayColumnsCount++;
                    for (int i = 1; i <= 31; i++)
                    {
                        if (dgv.Columns[i].Visible == true)
                        {

                            objsheet.Cells[2, displayColumnsCount] = "     " + (i + "日").ToString().Trim();
                            displayColumnsCount++;
                        }
                    }

                    objsheet.Cells[2, displayColumnsCount] = "    月雨量";
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

    }
}
