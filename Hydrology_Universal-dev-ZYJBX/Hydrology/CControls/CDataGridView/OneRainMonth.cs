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

namespace Hydrology.CControls
{
    /// <summary>
    /// 单站平均水位月报表
    /// </summary>
    class OneRainMonth : CExDataGridView
    {
        #region 静态常量
        private static readonly string aver = "日雨量";
        private static double allSum;

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

        public OneRainMonth()
            : base()
        {
            List<string> headerTmp = new List<string>();
            headerTmp.Add("时间");
            for (int i = 9; i < 24; i++)
            {
                headerTmp.Add(i.ToString() + "时");
            }
            for (int j = 0; j < 9; j++)
            {
                headerTmp.Add(j.ToString() + "时");
            }
            headerTmp.Add(aver);
            this.Header = headerTmp.ToArray();
            this.ReCalculateSize();
            this.BPartionPageEnable = false; //不启用分页功能

            // 初始化数据源
            m_proxyRain = CDBDataMgr.Instance.GetRainProxy();
            AutoSizeColumn(this);
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

        public void SetFilter(string StationID, DateTime time)
        {
            allSum = 0;
            List<string[]> listShowRows = new List<string[]>();

            int year = time.Year;
            int month = time.Month;
            int days = DateTime.DaysInMonth(year, month);
            List<EDataState> listState = new List<EDataState>();
            for (int i = 1; i < days + 1; i++)
            {
                DateTime tmp = new DateTime(year, month, i, 9, 0, 0);
                //rainsRequied = CDBDataMgr.Instance.GetRainsForTable(StationID, tmp);
                EDataState state;
                //if(i == days)
                //{
                //    listShowRows.Add(AddAllSum(allSum, out state).ToArray());
                //}
                listShowRows.Add(GetShowStringList(StationID, tmp, i, out state).ToArray());
                listState.Add(state);
            }
            EDataState state1;
            listShowRows.Add(AddAllSum(allSum, out state1).ToArray());
            listState.Add(state1);

            base.ClearAllRows();
            base.AddRowRange(listShowRows, listState);
            base.UpdateDataToUI();
        }

        /// <summary>
        /// 导出到Excel表格中
        /// </summary>


        #region 帮助方法
        private List<string> AddAllSum(double allSum, out EDataState state)
        {
            List<string> result = new List<string>();
            result.Add("月雨量");
            for (int i = 0; i < 24; i++)
            {
                result.Add(" ");
            }
            result.Add(allSum.ToString());
            double rate = 0.5;
            state = GetState(rate);
            return result;

        }
        private List<string> GetShowStringList(string stationID, DateTime time, int day, out EDataState state)
        {
            List<CEntityRain> tmpResults = new List<CEntityRain>();
            List<string> results = new List<string>();
            results.Add(day + "日");
            //j=-double aver = 0;
            double sum = 0;
            tmpResults = CDBDataMgr.Instance.GetRainsForTable(stationID, time);
            int len = tmpResults.Count;
            for (int i = 0; i < len; i++)
            {
                if (tmpResults[i].PeriodRain.ToString() != "" && tmpResults[i].PeriodRain != -9999)
                {
                    double rain = double.Parse(tmpResults[i].PeriodRain.ToString());
                    sum = sum + rain;
                }
            }
            allSum = allSum + sum;
            if (len == 24)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (tmpResults[i].PeriodRain.ToString() != "" && tmpResults[i].PeriodRain != -9999)
                    {
                        double flag = double.Parse(tmpResults[i].PeriodRain.ToString());
                        if (flag - 0 > 0.01)
                        {
                            results.Add(tmpResults[i].PeriodRain.ToString());
                        }
                        else
                        {
                            results.Add(" ");
                        }
                    }
                    else
                    {
                        results.Add("-- ");
                    }
                }
            }
            else
            {
                List<int> hours = new List<int>();
                int array = 0;
                for (int j = 0; j < len; j++)
                {
                    DateTime dt = tmpResults[j].TimeCollect;
                    int d = dt.Hour;
                    hours.Add(d);
                }
                for (int k = 9; k < 24; k++)
                {
                    if (hours.Contains(k))
                    {
                        if (tmpResults[array].PeriodRain.ToString() != "" && tmpResults[array].PeriodRain != -9999)
                        {
                            double flag = double.Parse(tmpResults[array].PeriodRain.ToString());
                            if (flag - 0 > 0.01)
                            {
                                results.Add(tmpResults[array].PeriodRain.ToString());
                            }
                            else
                            {
                                results.Add(" ");
                            }
                        }
                        else
                        {
                            results.Add("--");
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
                        if (tmpResults[k].PeriodRain.ToString() != "" && tmpResults[k].PeriodRain != -9999)
                        {
                            double flag = double.Parse(tmpResults[array].PeriodRain.ToString());
                            if (flag - 0 > 0.01)
                            {
                                results.Add(tmpResults[array].PeriodRain.ToString());
                            }
                            else
                            {
                                results.Add(" ");
                            }
                        }
                        else
                        {
                            results.Add("--");
                        }
                        array = array + 1;
                    }
                    else
                    {
                        results.Add("--");
                    }
                }
            }
            double rate = 0.5;
            state = GetState(rate);

            results.Add(sum.ToString());
            return results;
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
            this.Columns[0].Width = 50;
            for (int i = 1; i < 25; ++i)
            {
                this.Columns[i].Width = 45; //设定宽度
            }
            this.Columns[25].Width = 50;
        }
        public void ExportToExcel(DateTime dt, string stationName)
        {
            // 弹出对话框，并导出到Excel文件
            int year = dt.Year;
            int month = dt.Month;
            string name = stationName + "_" + year + "年" + month + "月";
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
                dataTable.Columns.Add("站点");
                for (int i = 9; i < 25; ++i)
                {
                    dataTable.Columns.Add(i.ToString(), typeof(string));
                }
                for (int i = 1; i < 9; ++i)
                {
                    dataTable.Columns.Add(i.ToString(), typeof(string));
                }


                dataTable.Columns.Add(aver, typeof(string));
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
            string name = stationName + "_" + year + "年" + month + "月";
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
                    objsheet.Cells[2, displayColumnsCount] = "时间";
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

        #endregion 帮助方法
    }
}
