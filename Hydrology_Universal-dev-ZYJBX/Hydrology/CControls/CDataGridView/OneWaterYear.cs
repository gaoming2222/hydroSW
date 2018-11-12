using Hydrology.DataMgr;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using Hydrology.Forms;
using Hydrology.Utils;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Hydrology.CControls.CDataGridView
{
    class OneWaterYear : CExDataGridView
    {
        #region 静态常量
        private static string Stamonth = "月份";
        public static int flag = 1;
        #endregion 静态常量

        #region 成员变量
        private IWaterProxy m_proxyWater;
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

        public OneWaterYear()
            : base()
        {
            List<string> headerTmp = new List<string>();
            headerTmp.Add(Stamonth);
            for (int i = 1; i <= 31; i++)
            {
                headerTmp.Add(i.ToString() + "日");
            }

            this.Header = headerTmp.ToArray();
            this.ReCalculateSize();

            this.BPartionPageEnable = false; //不启用分页功能
            // 初始化数据源
            m_proxyWater = CDBDataMgr.Instance.GetWaterProxy();
        }
        public void SetFilter(string StationID, DateTime time)
        {
            //添加代码
            int year = time.Year;
            int month = time.Month;
            int days = DateTime.DaysInMonth(year, month);
            List<string[]> listShowRows = new List<string[]>();
            List<EDataState> listState = new List<EDataState>();
            for (int i = 1; i <= 12; i++)
            {
                flag = i;
                DateTime tmp = new DateTime(year, i, 1, 0, 0, 0);
                EDataState state;
                listShowRows.Add(GetShowStringList(StationID, tmp, out state).ToArray());
                listState.Add(state);
            }

            base.ClearAllRows();
            base.AddRowRange(listShowRows, listState);
            base.UpdateDataToUI();

        }
        private List<string> GetShowStringList(string station, DateTime time, out EDataState state)
        {
            int year = time.Year;
            int month = time.Month;
            int days = DateTime.DaysInMonth(year, month);
            List<string> results = new List<string>();
            results.Add(flag + "月");
            //List<string> tmpResults = new List<string>();
            List<CEntityWater> temResults = new List<CEntityWater>();
            temResults = CDBDataMgr.Instance.getWaterForYearTable(station, time);
            for (int j = 1; j <= days; j++)
            {
                decimal tmpSum = 0;
                int count = 0;

                for (int k = 0; k < temResults.Count; k++)
                {
                    if (temResults[k].TimeCollect.Day == j)
                    {
                        if (temResults[k].WaterStage != -9999)
                        {
                            tmpSum = tmpSum + temResults[k].WaterStage;
                            count = count + 1;
                        }
                    }

                }
                if (count != 0)
                {
                    results.Add((tmpSum / count).ToString("0.00"));
                }
                else
                {
                    results.Add("--");
                }


            }
            for (int l = results.Count; l < 32; l++)
            {
                results.Add("--");
            }
            double rate = 0.5;
            state = GetState(rate);
            return results;
        }
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
            for (int i = 0; i < 32; ++i)
            {
                this.Columns[i].Width = 40; //设定宽度
            }
        }

        public void ExportToExcel(DateTime dt, string stationName)
        {
            // 弹出对话框，并导出到Excel文件
            int year = dt.Year;
            int month = dt.Month;
            string name = stationName + "_" + year + "年";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = name + "水位表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            PrintDocument pd = new PrintDocument();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataTable.Columns.Add(Stamonth);
                for (int i = 1; i < 32; i++)
                {
                    dataTable.Columns.Add(i.ToString() + "日");
                }

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
                if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, name + "水位表"))
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
            dlg.FileName = name + "水位表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                int rowscount = dgv.Rows.Count;
                int colscount = dgv.Columns.Count;
                if (rowscount <= 0)
                {
                    MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (colscount <= 0)
                {
                    MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (rowscount > 65536)
                {
                    MessageBox.Show("数据记录数太多(最多不能超过65536条)，不能保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (colscount > 255)
                {
                    MessageBox.Show("数据记录行数太多，不能保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                try
                {
                    //申明对象 
                    objExcel = new Microsoft.Office.Interop.Excel.Application();
                    objWorkbook = objExcel.Workbooks.Add(Missing.Value);
                    objsheet = (Microsoft.Office.Interop.Excel.Worksheet)objWorkbook.ActiveSheet;
                    //设置EXCEL不可见 
                    objExcel.Visible = false;
                    objsheet.PageSetup.PaperSize = Microsoft.Office.Interop.Excel.XlPaperSize.xlPaperA4;//A4纸张大小   
                    objsheet.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;//纸张方向.纵向
                    Range range2 = objsheet.Range[objsheet.Cells[1, 1], objsheet.Cells[1, 8]];
                    Range range = objsheet.Range[objsheet.Cells[1, 9], objsheet.Cells[1, 26]];
                    range.Font.Bold = true;
                    // range.Font.
                    //设置字体颜色
                    range.Font.ColorIndex = 0;
                    //设置颜色背景
                    range.Interior.ColorIndex = 15;

                    range2.Font.Bold = true;
                    // range.Font.
                    //设置字体颜色
                    range2.Font.ColorIndex = 0;
                    //设置颜色背景
                    range2.Interior.ColorIndex = 15;
                    objsheet.Cells[1, 7] = name + "水位表";
                    //  ((Microsoft.Office.Interop.Excel.Range)objsheet.Cells[1, 1]).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                    //向Excel中写入表格的表头
                    int displayColumnsCount = 1;
                    objsheet.Cells[2, displayColumnsCount] = "站点/日期";
                    displayColumnsCount++;
                    for (int i = 1; i <= 31; i++)
                    {
                        if (dgv.Columns[i].Visible == true)
                        {

                            objsheet.Cells[2, displayColumnsCount] = "     " + i + "日";
                            displayColumnsCount++;
                        }
                    }

                    //objsheet.Cells[2, displayColumnsCount] = "日雨量";
                    //设置进度条 
                    //tempProgressBar.Refresh(); 
                    //tempProgressBar.Visible   =   true; 
                    //tempProgressBar.Minimum=1; 
                    //tempProgressBar.Maximum=dgv.RowCount; 
                    //tempProgressBar.Step=1; 
                    //向Excel中逐行逐列写入表格中的数据 
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
                    //隐藏进度条 
                    //tempProgressBar.Visible   =   false; 
                    //保存文件 
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
                MessageBox.Show(dlg.FileName + "/n/n导出完毕! ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
