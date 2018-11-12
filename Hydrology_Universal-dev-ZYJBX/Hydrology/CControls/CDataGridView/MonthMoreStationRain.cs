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
    /// 多站月报表
    /// </summary>
    class MonthMoreStationRain : CExDataGridView
    {
        #region 静态常量
        private static readonly string StationName = "站号";
        private static readonly string StationName1 = "站名";
        private static readonly string AllRains = "月雨量";
        #endregion 静态常量

        //private IRainProxy m_proxyRain;
        public MonthMoreStationRain() : base()
        {
            List<String> HeaderTemp = new List<string>();
            HeaderTemp.Add("|");
            HeaderTemp.Add(StationName);
            HeaderTemp.Add(StationName1);
            for (int i = 1; i < 32; i++)
            {
                HeaderTemp.Add(i.ToString() + "日");
            }
            HeaderTemp.Add(AllRains);
            this.Header = HeaderTemp.ToArray();
            this.ReCalculateSize();
            this.BPartionPageEnable = false; //不启用分页功能
        }

        public void SetFilterTest(List<string> stations, DateTime date)
        {
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
                    listShowRows.Add(GetShowStringList(station, name, date, out state).ToArray());
                    listState.Add(state);
                }
            }
            base.ClearAllRows();
            base.AddRowRange(listShowRows, listState);
            base.UpdateDataToUI();

        }

        private List<string> GetShowStringList(string station, string name, DateTime date, out EDataState state)
        {
            int year = date.Year;
            int month = date.Month;
            int days = DateTime.DaysInMonth(year, month);
            List<string> averResults = new List<string>();
            List<string> results = new List<string>();
            for (int i = 1; i <= days; i++)
            {
                DateTime tmp = new DateTime(year, month, i, 9, 0, 0);
                List<CEntityRain> temResults = new List<CEntityRain>();
                temResults = CDBDataMgr.Instance.GetMRaainsForTable(station, tmp);
                int length = temResults.Count;
                double sum = 0;
                for (int j = 0; j < length; j++)
                {
                    if (temResults[j].PeriodRain >= 0)
                    {
                        double temp = double.Parse(temResults[j].PeriodRain.ToString());
                        sum = sum + temp;
                    }

                }
                if (length == 0)
                {
                    averResults.Add("--");
                }
                else
                {
                    averResults.Add(sum.ToString());
                }
            }
            results.Add("|");
            results.Add(station);
            results.Add(name);
            int cha = 31 - averResults.Count;
            double all = 0;
            for (int i = 0; i < averResults.Count; i++)
            {
                if (averResults[i] == "0")
                {
                    results.Add(" ");
                }
                else
                {
                    results.Add(averResults[i]);
                    if (averResults[i] != "--")
                    {
                        all = all + double.Parse(averResults[i]);
                    }
                }

            }
            for (int i = 0; i < cha; i++)
            {
                results.Add("\\");
            }
            results.Add(all.ToString());
            double rate = 0.5;
            state = GetState(rate);
            return results;
        }
        private EDataState GetState(double rate)
        {
            return EDataState.ENormal;
        }
        public void ReCalculateSize()
        {
            this.Columns[0].Width = 80;
            this.Columns[1].Width = 40;
            for (int i = 2; i < 34; ++i)
            {
                this.Columns[i].Width = 40; //设定宽度
            }

        }
        public void ExportToExcel(DateTime dt)
        {
            // 弹出对话框，并导出到Excel文件
            int year = dt.Year;
            int month = dt.Month;
            string name = year + "年" + month + "月";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = name + "雨量月表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataTable.Columns.Add("|");
                dataTable.Columns.Add(StationName);
                dataTable.Columns.Add(StationName1);

                for (int i = 1; i < 32; ++i)
                {
                    dataTable.Columns.Add(i.ToString() + "日", typeof(string));
                }

                dataTable.Columns.Add(AllRains, typeof(string));
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
                if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, name + "雨量月表"))
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
            int year = dt.Year;
            int month = dt.Month;
            string name = year + "年" + month + "月";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = name + "雨量月表";
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
