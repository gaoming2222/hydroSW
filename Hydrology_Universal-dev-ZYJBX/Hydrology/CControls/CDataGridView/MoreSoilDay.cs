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
    /// 单站平均水位月报表
    /// </summary>
    class MoreSoilDay : CExDataGridView
    {
        #region 静态常量
        private static readonly string day = "站号";
        private static readonly string name = "站名";
        private static readonly string water10 = "10cm含水率";
        private static readonly string water20 = "20cm含水率";
        private static readonly string water30 = "40cm含水率";
        private static readonly string mes = "备注";
        #endregion 静态常量

        #region 成员变量
#pragma warning disable CS0169 // 从不使用字段“MoreSoilDay.m_proxyWater”
        private IWaterProxy m_proxyWater;
#pragma warning restore CS0169 // 从不使用字段“MoreSoilDay.m_proxyWater”
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

        public MoreSoilDay()
            : base()
        {
            List<string> headerTmp = new List<string>();
            headerTmp.Add(" ");
            headerTmp.Add(day);
            headerTmp.Add(name);
            headerTmp.Add(water10);
            headerTmp.Add(water20);
            headerTmp.Add(water30);
            headerTmp.Add(mes);
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
                    listShowRows.Add(GetShowStringList(station, name,time, out state).ToArray());
                    listState.Add(state);
                }
            }
            base.ClearAllRows();
            base.AddRowRange(listShowRows, listState);
            base.UpdateDataToUI();

        }
        public void ExportToExcel(DateTime dt)
        {
            int year = dt.Year;
            int month = dt.Month;
            int days = dt.Day;
            string name = year + "年" + month + "月" + days + "日";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = name + "墒情表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataTable.Columns.Add(" ");
                dataTable.Columns.Add(day);
                dataTable.Columns.Add(water10);
                dataTable.Columns.Add(water20);
                dataTable.Columns.Add(water30);
                dataTable.Columns.Add(mes, typeof(string));
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
                if (CExcelExport.ExportToExcelWrapper(dataTable, dlg.FileName, name + "墒情表"))
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
                    objsheet.Cells[2, displayColumnsCount] = "40cm含水率";
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

        private List<string> GetShowStringList(string stationId,string name, DateTime time, out EDataState state)
        {
            List<string> results = new List<string>();
            results.Add(" ");
            results.Add(stationId);
            results.Add(name);
            int year = time.Year;
            int month = time.Month;
            int days = time.Day;
            DateTime temp = new DateTime(year, month, days, 0, 0, 0);
            float moi10 = 0;
            float moi20 = 0;
            float moi30 = 0;
            float sum10 = 0;
            float sum20 = 0;
            float sum30 = 0;
            int count10 = 0;
            int count20 = 0;
            int count30 = 0;
            List<CEntitySoilData> tmpResults = new List<CEntitySoilData>();
            tmpResults = CDBDataMgr.Instance.GetMSoilsForTable(stationId, temp);
            int length = tmpResults.Count;
            if (length == 0)
            {
                results.Add("//");
                results.Add("//");
                results.Add("//");
                results.Add(" ");
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    if (tmpResults[i].Moisture10 != -1)
                    {
                        sum10 = sum10 + float.Parse(tmpResults[i].Moisture10.ToString());
                        count10 = count10 + 1;
                    }
                    if (tmpResults[i].Moisture20 != -1)
                    {
                        sum20 = sum20 + float.Parse(tmpResults[i].Moisture20.ToString());
                        count20 = count20 + 1;
                    }
                    if (tmpResults[i].Moisture30 != -1)
                    {
                        sum30 = sum30 + float.Parse(tmpResults[i].Moisture30.ToString());
                        count30 = count30 + 1;
                    }
                }
                if (count10 != 0)
                {
                    moi10 = sum10 / count10;
                    results.Add(moi10.ToString("0.00"));
                }
                else
                {
                    results.Add("--");
                }

                if (count20 != 0)
                {
                    moi20 = sum20 / count20;
                    results.Add(moi20.ToString("0.00"));
                }
                else
                {
                    results.Add("--");
                }

                if (count30 != 0)
                {
                    moi30 = sum30 / count30;
                    results.Add(moi30.ToString("0.00"));
                }
                else
                {
                    results.Add("--");
                }
                results.Add(" ");
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

            for (int i = 0; i < 5; ++i)
            {
                this.Columns[i].Width = 200; //设定宽度
            }
            this.Columns[5].Width = 500;
        }
    }
}