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
    /// 单站平均水位日报表
    /// </summary>
    class MoreWaterDay : CExDataGridView
    {
        #region 静态常量
        private static readonly string day = "站号";
        private static readonly string name = "站名";
        private static readonly string aver = "平均";
        private static readonly string max = "最大值";
        private static readonly string maxTime = "最大时分";
        private static readonly string min = "最小值";
        private static readonly string minTime = "最小时分";
        #endregion 静态常量

        #region 成员变量
#pragma warning disable CS0169 // 从不使用字段“MoreWaterDay.m_proxyWater”
        private IWaterProxy m_proxyWater;
#pragma warning restore CS0169 // 从不使用字段“MoreWaterDay.m_proxyWater”
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

        public MoreWaterDay()
            : base()
        {
            List<string> headerTmp = new List<string>();
            headerTmp.Add("|");
            headerTmp.Add(day);
            headerTmp.Add(name);
            for (int i = 0; i < 24; i++)
            {
                headerTmp.Add(i.ToString());
            }

            headerTmp.Add(aver);
            headerTmp.Add(max);
            headerTmp.Add(maxTime);
            headerTmp.Add(min);
            headerTmp.Add(minTime);
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
            dlg.FileName = name + "水位表";
            dlg.Filter = "Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                // 保存到Excel表格中
                System.Data.DataTable dataTable = new System.Data.DataTable();

                dataTable.Columns.Add("|");
                dataTable.Columns.Add(day);
                for (int i = 0; i < 24; i++)
                {
                    dataTable.Columns.Add(i.ToString(), typeof(string));
                }
                dataTable.Columns.Add(aver);
                dataTable.Columns.Add(max);
                dataTable.Columns.Add(maxTime);
                dataTable.Columns.Add(min);
                dataTable.Columns.Add(minTime);
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
            }
        }

        public void ExportToExcelNew(DataGridView dgv, DateTime dt)
        {
            int year = dt.Year;
            int month = dt.Month;
            int days = dt.Day;
            string name = year + "年" + month + "月" + days + "日";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = name + "水位表";
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
                    Range range = objsheet.Range[objsheet.Cells[1, 9], objsheet.Cells[1, 30]];
                    range.Font.Bold = true;
                    range.Font.ColorIndex = 0;
                    range2.Font.Bold = true;
                    range2.Font.ColorIndex = 0;
                    objsheet.Cells[1, 7] = name + "水位表";
                    int displayColumnsCount = 1;
                    objsheet.Cells[2, displayColumnsCount] = "|";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "站号";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "站名";
                    displayColumnsCount++;
                    for (int i = 1; i <= 24; i++)
                    {
                        if (dgv.Columns[i].Visible == true)
                        {

                            objsheet.Cells[2, displayColumnsCount] = "     " + (i + "时").ToString().Trim();
                            displayColumnsCount++;
                        }
                    }
                    objsheet.Cells[2, displayColumnsCount] = "    平均";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "    最大";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = " 最大时分";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = "    最小";
                    displayColumnsCount++;
                    objsheet.Cells[2, displayColumnsCount] = " 最小时分";
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

        private List<string> GetShowStringList(string stationId, string name, DateTime time, out EDataState state)
        {
            int year = time.Year;
            int month = time.Month;
            int day = time.Day;
            List<string> results = new List<string>();
            results.Add(" ");
            results.Add(stationId);
            results.Add(name);
            string minTime = "";
            string maxTime = "";

            double max = 0;
            double min = 100000;
            double sum = 0;
            double aver = 0;
            DateTime tmp = new DateTime(year, month, day, 0, 0, 0);
            List<CEntityWater> temResults = new List<CEntityWater>();
            temResults = CDBDataMgr.Instance.GetWaterForTable(stationId, tmp);
            int length = temResults.Count;
            for (int i = 0; i < length; i++)
            {
                if (double.Parse(temResults[i].WaterStage.ToString()) > max)
                {
                    max = double.Parse(temResults[i].WaterStage.ToString());
                    maxTime = temResults[i].TimeCollect.Hour + "时" + temResults[i].TimeCollect.Minute + "分";
                }
                if (double.Parse(temResults[i].WaterStage.ToString()) < min)
                {
                    min = double.Parse(temResults[i].WaterStage.ToString());
                    minTime = temResults[i].TimeCollect.Hour + "时" + temResults[i].TimeCollect.Minute + "分";
                }
                sum = sum + double.Parse(temResults[i].WaterStage.ToString());
            }
            if (length != 0)
            {
                aver = sum / length;
            }
            else
            {
                aver = 9999.9999;
            }

            for (int i = 0; i < 24; i++)
            {
                double tmpSum = 0;
                List<string> smallResults = new List<string>();
                for (int j = 0; j < temResults.Count; j++)
                {
                    DateTime temp = temResults[j].TimeCollect;
                    if (temp.Hour == i)
                    {
                        smallResults.Add(temResults[j].WaterStage.ToString());
                    }

                }
                if (smallResults.Count == 0)
                {
                    results.Add("--");
                }
                if (smallResults.Count != 0)
                {
                    for (int j = 0; j < smallResults.Count; j++)
                    {
                        tmpSum = tmpSum + double.Parse(smallResults[j]);
                    }
                    double tmpAver = tmpSum / smallResults.Count;
                    results.Add(tmpAver.ToString("0.00"));
                }
            }
            if (aver != 9999.9999)
            {
                results.Add(aver.ToString("0.00"));
            }
            else
            {
                results.Add("--");
            }

            if (max - 0 < 0.001)
            {
                results.Add("\\");
            }
            else
            {
                results.Add(max.ToString());
            }
            results.Add(maxTime);
            if (min - 9999 > 0.1)
            {
                results.Add("\\");
            }
            else
            {
                results.Add(min.ToString());
            }

            results.Add(minTime);

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
            this.Columns[0].Width = 80;
            this.Columns[1].Width = 45;
            for (int i = 2; i < 26; ++i)
            {
                this.Columns[i].Width = 40; //设定宽度
            }
            for (int i = 26; i < 30; ++i)
            {
                this.Columns[i].Width = 40; //设定宽度
            }
            this.Columns[28].Width = 60;


        }
    }
}