using System;
#pragma warning disable CS0105 // “System”的 using 指令以前在此命名空间中出现过
using System;
#pragma warning restore CS0105 // “System”的 using 指令以前在此命名空间中出现过
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
    class MonthMoreStationSoil : CExDataGridView
    {
        #region 静态常量
#pragma warning disable CS0414 // 字段“MonthMoreStationSoil.StationName”已被赋值，但从未使用过它的值
        private static readonly string StationName = "站名";
#pragma warning restore CS0414 // 字段“MonthMoreStationSoil.StationName”已被赋值，但从未使用过它的值
        private static readonly string Water10 = "10cm含水率";
        private static readonly string Water20 = "20cm含水率";
        private static readonly string Water30 = "40cm含水率";
        private static readonly string Mes = "备注";
        #endregion 静态常量

        //private IRainProxy m_proxyRain;
        public MonthMoreStationSoil() : base()
        {
            List<String> HeaderTemp = new List<string>();
            HeaderTemp.Add("iii");
            HeaderTemp.Add("站点");
            HeaderTemp.Add("站名");
            HeaderTemp.Add(Water10);
            HeaderTemp.Add(Water20);
            HeaderTemp.Add(Water30);
            HeaderTemp.Add(Mes);
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
                    listShowRows.Add(GetShowStringList(station, name,date, out state).ToArray());
                    listState.Add(state);
                }
            }
            base.ClearAllRows();
            base.AddRowRange(listShowRows, listState);
            base.UpdateDataToUI();

        }

        private List<string> GetShowStringList(string station,string name, DateTime date, out EDataState state)
        {
            int year = date.Year;
            int month = date.Month;
            int days = DateTime.DaysInMonth(year, month);
            string averResults10 = "";
            string averResults20 = "";
            string averResults30 = "";
            List<string> results = new List<string>();
            float moi10 = 0;
            float moi20 = 0;
            float moi30 = 0;
            float sum10 = 0;
            float sum20 = 0;
            float sum30 = 0;
            int count10 = 0;
            int count20 = 0;
            int count30 = 0;
            for (int i = 1; i <= days; i++)
            {
                DateTime tmp = new DateTime(year, month, i, 0, 0, 0);
                List<CEntitySoilData> temResults = new List<CEntitySoilData>();
                temResults = CDBDataMgr.Instance.GetMSoilsForTable(station, tmp);
                int length = temResults.Count;
                for (int j = 0; j < length; j++)
                {
                    if (temResults[j].Moisture10 != -1)
                    {
                        sum10 = sum10 + float.Parse(temResults[j].Moisture10.ToString());
                        count10 = count10 + 1;
                    }
                    if (temResults[j].Moisture20 != -1)
                    {
                        sum20 = sum20 + float.Parse(temResults[j].Moisture20.ToString());
                        count20 = count20 + 1;
                    }
                    if (temResults[j].Moisture30 != -1)
                    {
                        sum30 = sum30 + float.Parse(temResults[j].Moisture30.ToString());
                        count30 = count30 + 1;
                    }
                }
            }
            moi10 = sum10 / count10;
            moi20 = sum20 / count20;
            moi30 = sum30 / count30;
            if (count10 == 0)
            {
                averResults10 = "--";
            }
            else
            {
                averResults10 = moi10.ToString();
            }
            if (count20 == 0)
            {
                averResults20 = "--";
            }
            else
            {
                averResults20 = moi20.ToString();
            }
            if (count30 == 0)
            {
                averResults30 = "--";
            }
            else
            {
                averResults30 = moi30.ToString();
            }
            results.Add("。。。");
            results.Add(station);
            results.Add(name);
            results.Add(averResults10);
            results.Add(averResults20);
            results.Add(averResults30);
            results.Add("....");
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
            for (int i = 1; i < 5; ++i)
            {
                this.Columns[i].Width = 200; //设定宽度
            }
            this.Columns[5].Width = 500;
        }
        public void ExportToExcel(DateTime dt)
        {
            // 弹出对话框，并导出到Excel文件
            int year = dt.Year;
            int month = dt.Month;
            string name = year + "年" + month + "月";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = name + "墒情表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataTable.Columns.Add("iii");
                dataTable.Columns.Add("站点");
                dataTable.Columns.Add("站名");
                dataTable.Columns.Add(Water10);
                dataTable.Columns.Add(Water20);
                dataTable.Columns.Add(Water30);
                dataTable.Columns.Add(Mes, typeof(string));
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
                if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, "多站月表"))
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
            int days = dt.Day;
            string name = year + "年" + month + "月" + days + "日";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = name + "墒情表";
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
                    //objsheet.PageSetup.PrintTitleRows = "$1:$1";//顶端标题行    
                    objsheet.PageSetup.PaperSize = Microsoft.Office.Interop.Excel.XlPaperSize.xlPaperA4;//A4纸张大小   
                                                                                                        // objsheet.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;//纸张方向.纵向
                    Range range2 = objsheet.Range[objsheet.Cells[1, 1], objsheet.Cells[1, 2]];
                    Range range = objsheet.Range[objsheet.Cells[1, 3], objsheet.Cells[1, 5]];
                    range.Font.Bold = true;
                    range.Font.ColorIndex = 0;
                    range2.Font.Bold = true;
                    range2.Font.ColorIndex = 0;
                    objsheet.Cells[1, 1] = name + "墒情表";
                    int displayColumnsCount = 1;
                    objsheet.Cells[2, displayColumnsCount] = "  ";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "    站号";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "    站名";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "10cm含水率";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "20cm含水率";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "30cm含水率";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "    备注";


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
                                    objsheet.Cells[row + 3, displayColumnsCount] = dgv.Rows[row].Cells[col].Value.ToString().Trim();
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